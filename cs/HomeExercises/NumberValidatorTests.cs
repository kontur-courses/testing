using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        public class NumberValidator_Constructor_Tests
        {
            [Test]

            [TestCase(4, 1, false, TestName = "NotThrowAnything_AtPositivePrecision_PositiveScale_OnlyPositiveFalse")]
            [TestCase(4, 0, false, TestName = "NotThrowAnything_AtPositivePrecision_ZeroScale_OnlyPositiveFalse")]
            [TestCase(4, 0, true, TestName = "NotThrowAnything_AtPositivePrecision_ZeroScale_OnlyPositiveTrue")]
            public void Constructor_WithThreeCorrectArguments(int precision, int scale, bool onlyPositive)
            {
                Action action = () => new NumberValidator(precision, scale, onlyPositive);
                action.ShouldNotThrow();
            }

            [TestCase(4, 1)]
            public void Constructor_NotThrowAnything_WithTwoCorrectArguments(int precision, int scale)
            {
                Action action = () => new NumberValidator(precision, scale);
                action.ShouldNotThrow();
            }

            [TestCase(4)]
            public void Constructor_NotThrowAnything_WithOneCorrectArguments(int precision)
            {
                Action action = () => new NumberValidator(precision);
                action.ShouldNotThrow();
            }

            [Test]
            [TestCase(0, 1, TestName = "ThrowArgumentException_OnZeroPrecision")]
            [TestCase(-2, 1, TestName = "ThrowArgumentException_OnNegativePrecision")]
            [TestCase(4, -1, TestName = "ThrowArgumentException_OnNegativeScale")]
            [TestCase(4, 5, TestName = "ThrowArgumentException_OnScaleBiggerThanPrecision")]
            [TestCase(4, 4, TestName = "ThrowArgumentException_OnScaleSameAsPrecision")]
            public void Constructor_WhenIncorrectScaleOrPrecision(int precision, int scale)
            {
                Action action = () => new NumberValidator(precision, scale);
                action.ShouldThrow<ArgumentException>();
            }
        }


        public class NumberValidator_IsValidNumber_Tests
        {
            [Test]
            [TestCase("-1", TestName = "OnlyPositive_IsNotStatic")]
            [TestCase("1.0", TestName = "Scale_IsNotStatic")]
            [TestCase("1000", TestName = "Precision_IsNotStatic")]
            public void Validator_ShouldNotHave_StaticFields(string value)
            {
                var first = new NumberValidator(2, 0, true);
                var second = new NumberValidator(30, 10, false);
                first.IsValidNumber(value).Should().BeFalse();
            }

            [Test]
            [TestCase(null, TestName = "ReturnFalse_AtNullString")]
            [TestCase("", TestName = "ReturnFalse_AtEmptyString")]
            [TestCase("str", TestName = "ReturnFalse_AtStringOfLetters")]
            [TestCase("12r", TestName = "ReturnFalse_AtNumberWithLetters")]
            [TestCase("12;3", TestName = "ReturnFalse_AtStringWithWrongSeparator")]
            [TestCase("-+1", TestName = "ReturnFalse_AtTwoSignedNumber")]
            [TestCase("   ", TestName = "ReturnFalse_AtWhitespacesString")]
            public void IsValidNumber_ReturnFalse_AtInputWithNonNumbersOrNotExpectedSymbols(string value)
            {
                new NumberValidator(10, 5)
                    .IsValidNumber(value)
                    .Should()
                    .BeFalse();
            }

            [Test]
            [TestCase("0", true, TestName = "ReturnTrue_OnZero")]
            [TestCase("1", true, TestName = "ReturnTrue_OnNumberWithLength_ThatLessThanPrecision")]
            [TestCase("123", true, TestName = "ReturnTrue_OnNumberWithLength_ThatEqualsPrecision")]
            [TestCase("1234", false, TestName = "ReturnFalse_OnNumberWithLength_ThatBiggerThanPrecision")]
            public void IsValidNumber_WithNonZeroPrecisionAndZeroScale(string value, bool expectedResult)
            {
                new NumberValidator(3)
                    .IsValidNumber(value)
                    .Should()
                    .Be(expectedResult);
            }

            [Test]
            [TestCase("0.0", true, TestName = "ReturnTrue_OnNumberOfZeros")]
            [TestCase("1.23", true, TestName = "ReturnTrue_WhenScaleEquals_ToDecimalPartLength")]
            [TestCase("1.2", true, TestName = "ReturnTrue_WhenScaleLess_ThanDecimalPartLength")]
            [TestCase("1.234", false, TestName = "ReturnFalse_WhenScaleLess_ThanDecimalPartLength")]
            [TestCase(".12", false, TestName = "ReturnFalse_WhenIntegerPartIsEmptyAndNumberContainsSeparator")]
            [TestCase("123.", false, TestName = "ReturnFalse_WhenDecimalPartIsEmptyButNumberContainsSeparator")]
            [TestCase("12.0", true, TestName = "ReturnTrue_WhenLengthOfNumberEqualsPrecision_AndPointSeparator_IsNotCountingInLength")]
            [TestCase("12,0", true, TestName = "ReturnTrue_WhenLengthOfNumberEqualsPrecision_AndCommaSeparator_IsNotCountingInLength")]
            public void IsValidNumber_WithNonZeroPrecisionAndScale(string value, bool expectedResult)
            {
                new NumberValidator(3, 2)
                    .IsValidNumber(value)
                    .Should()
                    .Be(expectedResult);
            }

            [Test]
            [TestCase("+1.2", true, TestName = "ReturnTrue_OnPositiveNumber")]
            [TestCase("+0", true, TestName = "ReturnTrue_OnZeroWithPositiveSign")]
            [TestCase("-0", false, TestName = "ReturnFalse_OnZeroWithNegativeSign")]
            [TestCase("-1.2", false, TestName = "ReturnFalse_OnNegativeNumber")]
            [TestCase("+123", false, TestName = "ReturnFalse_WhenLengthOfNumberEqualsPrecision_AndSign_IsCountingInLength")]
            [TestCase("+123", false, TestName = "ReturnFalse_WhenLengthOfNumberEqualsPrecision_AndSign_IsCountingInLength")]
            public void IsValidNumber_WithOnlyPositive(string value, bool expectedResult)
            {
                new NumberValidator(3, 1, true)
                    .IsValidNumber(value)
                    .Should()
                    .Be(expectedResult);
            }
            [Test]
            [TestCase("+1.2", TestName = "ReturnTrue_OnPositiveNumber")]
            [TestCase("+0", TestName = "ReturnTrue_OnZeroWithPositiveSign")]
            [TestCase("-0", TestName = "ReturnTrue_OnZeroWithNegativeSign")]
            [TestCase("-1.2", TestName = "ReturnTrue_OnNegativeNumber")]
            public void IsValidNumber_With(string value)
            {
                new NumberValidator(3, 1)
                    .IsValidNumber(value)
                    .Should()
                    .BeTrue();
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