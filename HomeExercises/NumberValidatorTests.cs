using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {
        [TestCase(-1, 2, true, "precision must be a positive number",
             TestName = "When precision is negative")]
        [TestCase(1, -2, true, "scale must be a non-negative number less or equal than precision",
             TestName = "When scale is negative")]
        [TestCase(2, 2, true, "scale must be a non-negative number less or equal than precision",
             TestName = "When scale is bigger than precision")]
        public void NumberValidator_ThrowArgumentException(int precision, int scale, bool onlyPositive, string message)
        {
            new Action(() => new NumberValidator(precision, scale, onlyPositive))
                .ShouldThrow<ArgumentException>()
                .WithMessage(message);
        }

        [TestCase(1, 0, false, "scale can be 0", TestName = "When scale is 0")]
        [TestCase(1, 0, true, "scale can be 0", TestName = "When scale is 0")]
        public void NumberValidator_NotThrowArgumentException(int precision, int scale, bool onlyPositive,
            string message)
        {
            new Action(() => new NumberValidator(precision, scale, onlyPositive))
                .ShouldNotThrow<ArgumentException>();
        }

        [TestCase(3, 2, true, "-0.0", TestName = "WhenNegativeNumberOnPositiveOnlyValidator")]
        // precision test
        [TestCase(3, 2, false, "+0.00", TestName = "WhenLengthOfIntPartWithPlusGreaterThanPrecision")]
        [TestCase(3, 2, false, "-0.00", TestName = "WhenLengthOfIntPartWithMinusGreaterThanPrecision")]
        [TestCase(3, 2, false, "00.00", TestName = "WhenSignsNumberGreaterThanPrecision")]
        [TestCase(3, 2, false, "0000", TestName = "WhenNumberCountGreaterThanPrecision")]
        // scale test
        [TestCase(4, 2, false, "0.000", TestName = "WhenNumberOfDecimalPlacesGreaterThanScale")]
        [TestCase(4, 0, false, "0.0", TestName = "WhenNumberOfDecimalPlacesGreaterThanScale")]
        // precision and scale test
        [TestCase(4, 2, false, "10.000", TestName = "WhenBothIntPartAndFractPartDoNotSatisfy")]
        [TestCase(4, 2, false, "+0.000", TestName = "WhenBothIntPartWithPlusAndFractPartDoNotSatisfy")]
        [TestCase(4, 2, false, "-0.000", TestName = "WhenBothIntPartWithMinusAndFractPartDoNotSatisfy")]
        // format test
        [TestCase(3, 2, false, "0.0a", TestName = "WhenValueNotANumber")]
        [TestCase(3, 2, false, ".01", TestName = "WhenOnlyFractionalPartExists")]
        [TestCase(3, 2, false, "1.", TestName = "WhenOnlyIntegerPartExists")]
        [TestCase(3, 2, false, "-", TestName = "WhenValueNotANumber")]
        [TestCase(3, 2, false, "", TestName = "WhenValueIsEmptyString")]
        [TestCase(3, 2, false, null, TestName = "WhenValueIsNull")]
        public void IsValidNumber_Fails(int precision, int scale, bool positiveOnly, string value)
        {
            var validator = new NumberValidator(precision, scale, positiveOnly);
            validator.IsValidNumber(value).Should().BeFalse();
        }

        [TestCase(2, 0, false, "-0", TestName = "WhenZeroWithMinus")] //???
        [TestCase(2, 0, true, "+1", TestName = "WhenPositiveNumber")]
        [TestCase(2, 0, false, "-1", TestName = "WhenNegativeNumber")]
        [TestCase(100501, 100500, false, "-1", TestName = "WhenScaleBiggerThanDecimalPlacesNumber")]
        [TestCase(3, 2, false, "111", TestName = "WhenNumberWithoutSign")]
        [TestCase(3, 2, false, "0.00", TestName = "WhenIntAndFracParts")]
        [TestCase(4, 2, true, "+0.00", TestName = "WhenIntAndFracPartsWithPlus")]
        [TestCase(4, 2, false, "-0.00", TestName = "WhenIntAndFracPartsWithMinus")]
        public void IsValidNumber_Passes(int precision, int scale, bool positiveOnly, string value)
        {
            var validator = new NumberValidator(precision, scale, positiveOnly);
            validator.IsValidNumber(value).Should().BeTrue();
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