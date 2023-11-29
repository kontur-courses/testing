using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        private const string INVALID_PRECISION_MESSAGE = "precision must be a positive number";
        private const string INVALID_SCALE_MESSAGE = "scale must be a non-negative number less than precision";

        [TestCase(-1, 2, true, TestName = "NumberValidator Invalid Case: Negative Precision Throws")]
        [TestCase(0, 2, true, TestName = "NumberValidator Invalid Case: Zero Precision Throws")]
        public void TestNumberValidatorNonPositivePrecisionThrows(int precision, int scale, bool onlyPositive)
        {
            Action numberValidator = () => new NumberValidator(precision, scale, onlyPositive);
            numberValidator.Should().Throw<ArgumentException>().WithMessage(INVALID_PRECISION_MESSAGE);
        }

        [TestCase(4, -1, true, TestName = "NumberValidator Invalid Case: Negative Scale Throws")]
        public void TestNumberValidatorNegativeScaleThrows(int precision, int scale, bool onlyPositive)
        {
            Action numberValidator = () => new NumberValidator(precision, scale, onlyPositive);
            numberValidator.Should().Throw<ArgumentException>().WithMessage(INVALID_SCALE_MESSAGE);
        }

        [TestCase(3, 3, true, TestName = "NumberValidator Invalid Case: Scale Equal Precision Throws")]
        [TestCase(3, 4, true, TestName = "NumberValidator Invalid Case: Scale Greater Precision Throws")]
        public void TestNumberValidatorScaleGreaterOrEqualPrecisionThrows(int precision, int scale, bool onlyPositive)
        {
            Action numberValidator = () => new NumberValidator(precision, scale, onlyPositive);
            numberValidator.Should().Throw<ArgumentException>().WithMessage(INVALID_SCALE_MESSAGE);
        }

        [TestCase(2, 0, true, TestName = "NumberValidator: Valid Cases")]
        [TestCase(2, 1, true, TestName = "NumberValidator: Valid Cases")]
        [TestCase(2, 0, false, TestName = "NumberValidator: Valid Cases")]
        [TestCase(2, 1, false, TestName = "NumberValidator: Valid Cases")]
        public void TestNumberValidatorValidCases(int precision, int scale, bool onlyPositive)
        {
            Action numberValidator = () => new NumberValidator(precision, scale, onlyPositive);
            numberValidator.Should().NotThrow<ArgumentException>();
        }

        [TestCase(4, 2, true, "", TestName = "IsValidNumber Invalid Case: Value Is Null or Empty")]
        [TestCase(4, 2, true, null, TestName = "IsValidNumber Invalid Case: Value Is Null or Empty")]
        public void TestIsValidNumberValueIsNullOrEmpty(int precision, int scale, bool onlyPositive, string value)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);
            numberValidator.IsValidNumber(value).Should().BeFalse($"{value} should be null or empty");
        }

        [TestCase(4, 2, true, "a.sd", TestName = "IsValidNumber Invalid Case: Value Is Not a Number")]
        [TestCase(4, 2, true, "0.a", TestName = "IsValidNumber Invalid Case: Value Is Not a Number")]
        [TestCase(4, 2, true, "a.0", TestName = "IsValidNumber Invalid Case: Value Is Not a Number")]
        [TestCase(4, 2, true, " ", TestName = "IsValidNumber Invalid Case: Value Is Not a Number")]
        public void TestIsValidNumberValueIsNotNumber(int precision, int scale, bool onlyPositive, string value)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);
            numberValidator.IsValidNumber(value).Should().BeFalse($"{value} should not be a number");
        }

        [TestCase(4, 2, true, "12345.12", TestName = "IsValidNumber Invalid Case: Value Length Greater Precision")]
        public void TestIsValidNumberValueLengthGreaterPrecision(int precision, int scale, bool onlyPositive, string value)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);
            numberValidator.IsValidNumber(value).Should().BeFalse($"{value} length should be greater {precision}");
        }

        [TestCase(4, 2, true, "1.123", TestName = "IsValidNumber Invalid Case: Frac Length Greater Scale")]
        public void TestIsValidNumberFracLengthGreaterScale(int precision, int scale, bool onlyPositive, string value)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);
            numberValidator.IsValidNumber(value).Should().BeFalse($"{value} frac part should be greater {scale}");
        }

        [TestCase(4, 2, true, "-1.1", TestName = "IsValidNumber Invalid Case: Value Is Negative While OnlyPositive Is True")]
        public void TestIsValidNumberValueIsNegativeWhileOnlyPositiveIsTrue(int precision, int scale, bool onlyPositive, string value)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);
            numberValidator.IsValidNumber(value).Should().BeFalse($"{value} should be negative while OnlyPositive is {onlyPositive}");
        }

        [TestCase(4, 2, true, "12.12", TestName = "IsValidNumber: Valid Cases")]
        [TestCase(4, 2, true, "1.1", TestName = "IsValidNumber: Valid Cases")]
        [TestCase(4, 2, true, "1,1", TestName = "IsValidNumber: Valid Cases")]
        [TestCase(4, 2, false, "12.12", TestName = "IsValidNumber: Valid Cases")]
        [TestCase(4, 2, false, "-1.12", TestName = "IsValidNumber: Valid Cases")]
        public void TestIsValidNumberValidCases(int precision, int scale, bool onlyPositive, string value)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);
            numberValidator.IsValidNumber(value).Should().BeTrue($"{value} should be a valid number");
        }
    }

    public class NumberValidator
    {
        private readonly Regex numberRegex;
        private readonly bool onlyPositive;
        private readonly int precision;
        private readonly int scale;

        public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
        {
            this.precision = precision;
            this.scale = scale;
            this.onlyPositive = onlyPositive;
            if (precision <= 0)
                throw new ArgumentException("precision must be a positive number");
            if (scale < 0 || scale >= precision)
                throw new ArgumentException("scale must be a non-negative number less than precision");
            numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
        }

        public bool IsValidNumber(string value)
        {
            // Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
            // описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
            // Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
            // целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
            // Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

            if (string.IsNullOrEmpty(value))
                return false;

            var match = numberRegex.Match(value);
            if (!match.Success)
                return false;

            // Знак и целая часть
            var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
            // Дробная часть
            var fracPart = match.Groups[4].Value.Length;

            if (intPart + fracPart > precision || fracPart > scale)
                return false;

            if (onlyPositive && match.Groups[1].Value == "-")
                return false;
            return true;
        }
    }
}