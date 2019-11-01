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
        [TestCase(0, 1, TestName = "Precision_IsZero")]
        [TestCase(4, -1, TestName = "Scale_IsNegative")]
        [TestCase(-2, 1, TestName = "Precision_IsNegative")]
        [TestCase(4, 4, TestName = "Scale_Equals_Precision")]
        [TestCase(4, 5, TestName = "Scale_Bigger_ThanPrecision")]
        public void Ctor_WIthIncorrectScaleOrPrecision_ThrowArgumentException_When(int precision, int scale)
        {
            Action action = () => new NumberValidator(precision, scale);
            action.Should().Throw<ArgumentException>();
        }

        public class IsValidNumber_Tests
        {
            [TestCase(null, TestName = "Number_IsNull")]
            [TestCase("", TestName = "Number_IsEmpty")]
            [TestCase("-+1", TestName = "Number_IsTwoSigned")]
            [TestCase(" ", TestName = "Number_IsWhiteSpaces")]
            [TestCase("sr", TestName = "Number_ContainsLetters")]
            [TestCase("1r", TestName = "Number_ContainsLetters")]
            [TestCase("1;3", TestName = "Number_ContainsWrongSeparator")]
            [TestCase("1234", TestName = "LengthOfNumber_BiggerThanPrecision")]
            [TestCase("123.", TestName = "DecimalPart_Empty_AndNumberContainsSeparator")]
            [TestCase("+123", TestName = "LengthOfNumber_EqualsPrecision_ButNumberContainsPlus")]
            public void NonZeroPrecision_NonZeroScale_NotOnlyPositive_ReturnFalse_When(string value)
            {
	            var numberValidator = new NumberValidator(3, 2);
	            numberValidator.IsValidNumber(value)
                    .Should()
                    .BeFalse();
            }

            [TestCase("0", TestName = "Number_IsZero")]
            [TestCase("+0", TestName = "Zero_ContainsPlus")]
            [TestCase("+1.2", TestName = "Number_IsPositive")]
            [TestCase("0.0", TestName = "Number_ContainsZerosInIntegerAndDecimalParts")]
            [TestCase("1.23", TestName = "DecimalPartLength_Equals_Scale")]
            [TestCase("1.2", TestName = "DecimalPartLength_Bigger_Scale")]
            [TestCase("1", TestName = "LengthOfNumber_Less_Precision")]
            [TestCase("123", TestName = "LengthOfNumber_Equals_Precision")]
            [TestCase("12.0", TestName = "LengthOfNumber_Equals_Precision_AndPointSeparator_IsNotCountingInLength")]
            [TestCase("12,0", TestName = "LengthOfNumber_Equals_Precision_AndCommaSeparator_IsNotCountingInLength")]
            public void NonZeroPrecision_NonZeroScale_NotOnlyPositive_ReturnTrue_When(string value)
            {
                var numberValidator = new NumberValidator(3, 2, true);
                numberValidator
                    .IsValidNumber(value)
                    .Should()
                    .BeTrue();
            }

            [TestCase("-1.2", TestName = "Number_IsNegative")]
            [TestCase("-0", TestName = "Number_IsZeroWithNegativeSign")]
            [TestCase("1.234", TestName = "DecimalPartLength_Bigger_Scale")]
            [TestCase(".12", TestName = "IntegerPart_Empty_AndNumberContainsSeparator")]
            [TestCase("-123", TestName = "LengthOfNumber_Equals_Precision_ButNumberContainsMinus")]
            public void NonZeroPrecision_NonZeroScale_OnlyPositive_ReturnFalse_When(string value)
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