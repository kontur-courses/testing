using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        [TestCase(-1, 0, TestName = "ConstructorThrowArgumentException_WhenNonPositivePrecision")]
        [TestCase(3, -2, TestName = "ConstructorThrowArgumentException_WhenNegativeScale")]
        [TestCase(1, 3, TestName = "ConstructorThrowArgumentException_WhenScaleOverPrecision")]
        public void NumberValidatorThrowExceptionOnWrongParameters(int precision, int scale)
        {
            ((Action)(() => new NumberValidator(precision, scale))).Should().Throw<ArgumentException>();
        }

        [Test]
        [TestCase(2, 1, TestName = "ConstructorNotThrowArgumentException_WhenPrecisionPositive")]
        [TestCase(2, 0, TestName = "ConstructorNotThrowArgumentException_WhenScaleZero")]
        public void NumberValidatorNotThrowExceptionOnCorrectParameters(int precision, int scale)
        {
            ((Action)(() => new NumberValidator(precision, scale))).Should().NotThrow();
        }

        [Test]
        [TestCase(17, 2, "0.0", TestName = "NumberIsValid_WhenValueCorrect")]
        [TestCase(17, 0, "2", TestName = "NumberIsValid_WhenValueInt")]
        [TestCase(17, 2, "+2.3", TestName = "NumberIsValid_WhenValuePositive")]
        [TestCase(17, 2, "-2.3", TestName = "NumberIsValid_WhenValueNegative")]
        [TestCase(17, 2, "2,3", TestName = "NumberIsValid_WhenCommaIsSeparatorInValue")]
        [TestCase(4, 2, "12,12", TestName = "NumberIsValid_SeparatorNotAffectOnCountNumberInPrecision")]
        public void ValidNumberOnCorrectValue(int precision, int scale, string value, bool onlyPositive = false)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            validator.IsValidNumber(value).Should().BeTrue();
        }

        [Test]
        [TestCase(3, 2, "12,12", TestName = "NumberNotValid_WhenNumbersInValueOverPrecision")]
        [TestCase(5, 1, "12,123", TestName = "NumberNotValid_WhenFractionalPartInValueOverScale")]
        [TestCase(4, 2, "-123,1", TestName = "NumberNotValid_SignAffectOnCountNumberInPrecision")]
        [TestCase(4, 2, "-1,1", true, TestName = "NumberNotValid_WhenValueNegativeAndOnlyPositiveIsTrue")]
        [TestCase(4, 2, "asfa", TestName = "NumberNotValid_WhenValueWrong")]
        [TestCase(4, 2, "12,", TestName = "NumberNotValid_WhenValueNotContainFractionalPart")]
        [TestCase(4, 2, ",12", TestName = "NumberNotValid_WhenValueNotContainIntegerPart")]
        [TestCase(4, 2, " ", TestName = "NumberNotValid_WhenValueEmpty")]
        [TestCase(4, 2, null, TestName = "NumberNotValid_WhenValueNull")]
        [TestCase(4, 2, " 12.1", TestName = "NumberNotValid_WhenValueNotAtBeginningOfLine")]
        [TestCase(4, 2, "12.1 ", TestName = "NumberNotValid_WhenValueNotAtEndOfLine")]
        public void ValidNumberOnWrongValue(int precision, int scale, string value, bool onlyPositive = false)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            validator.IsValidNumber(value).Should().BeFalse();
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