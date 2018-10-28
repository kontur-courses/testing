using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;


namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [TestCase(17, 2, true, "1.23", TestName = "IfValueIsRealNumber")]
        [TestCase(17, 2, true, "0", TestName = "IfValueIsZero")]
        [TestCase(17, 2, false, "-0", TestName = "IfValueIsNegativeZero")]
        [TestCase(4, 2, true, "+1.23", TestName = "IfValueIsRealPositiveNumber")]
        [TestCase(4, 2, false, "-1.23", TestName = "IfValueIsRealNegativeNumber")]
        public void IsValidNumber_ShouldBeTrue_IfNumberFormatIsValid(int precision, int scale, bool onlyPositive, string inputValue)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(inputValue).Should().BeTrue();
        }


        [TestCase(3, 2, true, "00.00", TestName = "IfValue_IntegerAndFractal_LengthGreaterThanPrecision")]
        [TestCase(4, 2, true, "-0.00", TestName = "IfOnlyPositiveNotFalse")]
        [TestCase(17, 2, true, "0.000", TestName = "IfScaleExceededLimit")]
        [TestCase(3, 2, true, "-1.23", TestName = "IfPrecisionExceedLimitWithNegativeSign")]
        [TestCase(3, 2, true, "a.sd", TestName = "IfValueIsNotANumber")]
        [TestCase(1, 0, true, null, TestName = "IfValueIsNull")]
        [TestCase(1, 0, true, "", TestName = "IfValueIsEmptyString")]
        [TestCase(3, 2, false, ".00", TestName = "IfPointIsNotLeadByDigit")]
        [TestCase(2, 1, false, ",0", TestName = "IfCommaIsNotLeadByDigit")]
        [TestCase(3, 2, false, "-1.", TestName = "IfPointIsNotFollowedByDigit")]
        [TestCase(2, 1, false, "0,", TestName = "IfCommaIsNotFollowedByDigit")]
        [TestCase(20, 19, false, "-", TestName = "OnlySignWithoutDigits")]
        [TestCase(20, 19, false, "+", TestName = "OnlySignWithoutDigits")]
        public void IsValidNumber_ShouldBeFalse_IfNumberFormatIsNotValid(int precision, int scale, bool onlyPositive, string inputValue)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(inputValue).Should().BeFalse();
        }


        [TestCase(2, -1, "scale must be a non-negative number less than precision", TestName = "ScaleIsNegative")]
        [TestCase(-1, 2, "precision must be a positive number", TestName = "PrecisionIsNegative")]
        [TestCase(1, 2, "scale must be a non-negative number less than precision", TestName ="ScaleIsGreaterThanPrecision")]
        public void ThrowsArgumentExceptionWhen(int precision, int scale, string message)
        {
            Action action = () => new NumberValidator(precision, scale);
            action.ShouldThrow<ArgumentException>();
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
                throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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