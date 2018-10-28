using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;


namespace HomeExercises
{
    public class NumberValidatorTests
    {
        
        [TestCase(17, 2, true, "0", TestName = "ValueIsZero")]
        [TestCase(17, 2, false, "-0", TestName = "ValueIsNegativeZero")]
        [TestCase(17, 2, false, "+0", TestName = "ValueIsPositiveZero")]
        [TestCase(17, 2, true, "0.0", TestName = "ValidZeroWithFraction")]
        [TestCase(17, 2, true, "0,0", TestName = "ValidZeroWithFractionSeparatedWithComma")]
        [TestCase(17, 2, true, "1.23", TestName = "ValueIsRealNumber")]
        [TestCase(4, 2, true, "+1.23", TestName = "ValueIsRealPositiveNumber")]
        [TestCase(4, 2, false, "-1.23", TestName = "ValueIsRealNegativeNumber")]
        [TestCase(17, 2, true, "1234567890", TestName = "ValueIsValidWithAllDigits")]
        public void IsValidNumber_ShouldBeTrue_IfNumberFormatIsValid(int precision, int scale, bool onlyPositive, string inputValue)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(inputValue).Should().BeTrue();
        }


        [TestCase(3, 2, true, "00.00", TestName = "Value_IntegerAndFractal_LengthGreaterThanPrecision")]
        [TestCase(4, 2, true, "-0.00", TestName = "OnlyPositiveNotFalse")]
        [TestCase(17, 2, true, "0.000", TestName = "ScaleExceededLimit")]
        [TestCase(3, 2, true, "-1.23", TestName = "PrecisionExceedLimitWithNegativeSign")]
        [TestCase(3, 2, true, "a.sd", TestName = "ValueIsNotANumber")]
        [TestCase(1, 0, true, null, TestName = "ValueIsNull")]
        [TestCase(1, 0, true, "", TestName = "ValueIsEmptyString")]
        [TestCase(3, 2, false, ".00", TestName = "PointIsNotLeadByDigit")]
        [TestCase(2, 1, false, ",0", TestName = "CommaIsNotLeadByDigit")]
        [TestCase(3, 2, false, "-1.", TestName = "PointIsNotFollowedByDigit")]
        [TestCase(2, 1, false, "0,", TestName = "CommaIsNotFollowedByDigit")]
        [TestCase(20, 19, false, "-", TestName = "OnlySignWithoutDigits")]
        [TestCase(20, 19, false, "+", TestName = "OnlySignWithoutDigits")]
        public void IsValidNumber_ShouldBeFalse_IfNumberFormatIsNotValid(int precision, int scale, bool onlyPositive, string inputValue)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(inputValue).Should().BeFalse();
        }


        [TestCase(-1, 2, "precision must be a positive number", TestName = "PrecisionIsNegative")]
        [TestCase(2, -1, "scale must be a non-negative number less or equal than precision", TestName = "ScaleIsNegative")]
        [TestCase(1, 2, "scale must be a non-negative number less or equal than precision", TestName ="ScaleIsGreaterThanPrecision")]
        public void ThrowsArgumentExceptionWhen(int precision, int scale, string message)
        {
            Action action = () => new NumberValidator(precision, scale);
            action.ShouldThrow<ArgumentException>().WithMessage(message);
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