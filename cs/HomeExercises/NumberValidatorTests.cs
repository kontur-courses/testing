using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {
        [Test]
        [TestCase(0, 1, TestName = "OnZeroPrecision")]
        [TestCase(4, -1, TestName = "OnNegativeScale")]
        [TestCase(-2, 1, TestName = "OnNegativePrecision")]
        [TestCase(4, 4, TestName = "OnScaleSameAsPrecision")]
        [TestCase(4, 5, TestName = "OnScaleBiggerThanPrecision")]
        public void Ctor_ThrowArgumentException_WhenIncorrectScaleOrPrecision(int precision, int scale)
        {
            Action action = () => new NumberValidator(precision, scale);
            action.Should().Throw<ArgumentException>();
        }

        public class IsValidNumber_Tests
        {
            [TestCase(null, TestName = "String_IsNull")]
            [TestCase("", TestName = "String_IsEmpty")]
            [TestCase("str", TestName = "String_ContainsLetters")]
            [TestCase("12r", TestName = "Number_ContainsLetters")]
            [TestCase("12;3", TestName = "String_ContainsWrongSeparator")]
            [TestCase("-+1", TestName = "Number_IsTwoSigned")]
            [TestCase("   ", TestName = "String_IsWhiteSpaces")]
            public void NonZeroPrecision_NonZeroScale_OnlyPositive_ReturnFalse_When(string value)
            {
                new NumberValidator(10, 5)
                    .IsValidNumber(value)
                    .Should()
                    .BeFalse();
            }

            [TestCase("0", TestName = "WhenNumber_IsZero")]
            [TestCase("+0", TestName = "WhenZero_ContainsPlus")]
            [TestCase("+1.2", TestName = "WhenNumber_IsPositive")]
            [TestCase("1", TestName = "WhenLengthOfNumber__ThatLessThanPrecision")]
            [TestCase("123", TestName = "WhenLengthOfNumber__ThatEqualsPrecision")]
            [TestCase("12.0", TestName = "WhenLengthOfNumber_EqualsPrecision_AndPointSeparator_IsNotCountingInLength")]
            [TestCase("12,0", TestName = "WhenLengthOfNumber_EqualsPrecision_AndCommaSeparator_IsNotCountingInLength")]
            [TestCase("0.0", TestName = "WhenNumber_ContainsZerosInIntegerAndDecimalParts")]
            [TestCase("1.23", TestName = "WhenScaleEquals_ToDecimalPartLength")]
            [TestCase("1.2", TestName = "WhenScaleLess_ThanDecimalPartLength")]

            public void NonZeroPrecision_NonZeroScale_NotOnlyPositive_ReturnTrue(string value)
            {
                var numberValidator = new NumberValidator(3, 2, true);
                numberValidator
	                .IsValidNumber(value)
	                .Should()
	                .BeTrue();
            }

            [TestCase("1234", false, TestName = "WhenLengthOfNumber_BiggerThanPrecision")]
            [TestCase("123.", false, TestName = "WhenDecimalPart_Empty_AndNumberContainsSeparator")]
            [TestCase("+123", false, TestName = "WhenLengthOfNumber_EqualsPrecision_ButNumberContainsPlus")]
            public void NonZeroPrecision_ZeroScale_NotOnlyPositive_ReturnFalse(string value, bool expectedResult)
            {
                var numberValidator = new NumberValidator(3);
                numberValidator
                    .IsValidNumber(value)
                    .Should()
                    .BeFalse();
            }

            [TestCase("-1.2", TestName = "Number_IsNegative")]
            [TestCase("-0", TestName = "Number_IsZeroWithNegativeSign")]
            [TestCase("1.234", TestName = "ScaleLess_ThanDecimalPartLength")]
            [TestCase(".12", TestName = "IntegerPart_Empty_AndNumberContainsSeparator")]
            [TestCase("-123", TestName = "LengthOfNumber_EqualsPrecision_ButNumberContainsMinus")]
            public void NonZeroPrecision_NonZeroScale_OnlyPositive_ReturnFalse(string value)
            {
                var numberValidator = new NumberValidator(3, 2, true);
                numberValidator
                    .IsValidNumber(value)
                    .Should()
                    .BeFalse();
            }
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
                throw new ArgumentException("precision must be a non-negative number less or equal than precision");
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