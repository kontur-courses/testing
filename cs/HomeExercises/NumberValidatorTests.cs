using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        #region Tests for constructor

        [TestCase(-10, 10, TestName = "Precision < 0")]
        [TestCase(0, 10, TestName = "Precision = 0")]
        public void Constructor_ShouldThrowArgumentException_OnIncorrectPrecision(int precision, int scale)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
        }


        [TestCase(1, 10, TestName = "Scale > precision")]
        [TestCase(10, 10, TestName = "Scale = precision")]
        [TestCase(1, -10, TestName = "Scale < 0")]
        public void Constructor_ShouldThrowArgumentException_OnIncorrectScale(int precision, int scale)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
        }

        [TestCase(10, 2, TestName = "Scale > 0")]
        [TestCase(10, 0, TestName = "Scale = 0")]
        public void Constructor_ShouldNotThrowArgumentException_OnCorrectArguments(int precision, int scale)
        {
            Assert.DoesNotThrow(() => new NumberValidator(precision, scale));
        }

        #endregion

        #region Tests for IsValidNumber

        [TestCase(4, 2, "", TestName = "String is empty")]
        [TestCase(4, 2, null, TestName = "String = null")]
        public void IsValidNumber_ShouldReturnFalse_WhenInputIsNullOrEmpty(int precision, int scale, string value)
        {
            new NumberValidator(precision, scale).IsValidNumber(value).Should().BeFalse();
        }

        [TestCase(3, 0, "+-1", TestName = "Starts with 2 signs")]
        [TestCase(3, 1, "1..1", TestName = "Contains 2 points in a row")]
        [TestCase(3, 2, ".11", TestName = "Starts with point")]
        [TestCase(4, 2, "1.1.1.1", TestName = "Count of points > 1")]
        [TestCase(4, 2, "a.sd", TestName = "Contains letters with point")]
        [TestCase(3, 0, "abc", TestName = "Contains letters")]
        [TestCase(4, 0, "13sd", TestName = "Contains letters with numbers")]
        [TestCase(2, 0, "\n\r", TestName = "Contains escape-symbols")]
        [TestCase(4, 0, "10.\n\r", TestName = "Contains number and escape-symbols")]
        [TestCase(3, 0, "✹✹✹", TestName = "Contains unresolved symbols")]
        [TestCase(4, 0, "13✹✹", TestName = "Contains number and unresolved symbol")]
        [TestCase(2, 0, "11.", TestName = "Ends with point")]
        [TestCase(1, 0, "-", TestName = "Contains only sing")]
        [TestCase(1, 0, ".", TestName = "Contains only point")]
        [TestCase(3, 2, "-.11", TestName = "Starts with sing and point")]
        [TestCase(4, 2, "1.-11", TestName = "Sing isn't ahead")]
        public void IsValidNumber_ShouldReturnFalse_WhenInputDoesNotMatchRegex(int precision, int scale, string value)
        {
            new NumberValidator(precision, scale).IsValidNumber(value).Should().BeFalse();
        }

        [TestCase(3, 2, "11.23", TestName = "Input longer than precision and doesn't contain sign")]
        [TestCase(3, 2, "+0.00", TestName = "Input longer than precision and contains sign")]
        [TestCase(5, 2, "11.111", TestName = "Frac part longer than scale")]
        [TestCase(2, 0, "-10", TestName = "Sign included in precision")]
        public void IsValidNumber_ShouldReturnFalse_OnIncorrectPrecisionOrScale(int precision, int scale, string value)
        {
            new NumberValidator(precision, scale).IsValidNumber(value).Should().BeFalse();
        }

        [TestCase(2, 0, "-1", TestName = "Negative number")]
        [TestCase(2, 0, "-0", TestName = "Number is -0")]
        public void IsValidNumber_ShouldReturnFalse_WhenFlagOnlyPositiveIsTrueAndInputIsNegative(
            int precision, int scale, string value)
        {
            new NumberValidator(precision, scale, true).IsValidNumber(value).Should().BeFalse();
        }

        [TestCase(6, 2, false, "-10,15", TestName = "Num is negative, \",\" instead \".\"")]
        [TestCase(2, 0, false, "10", TestName = "Positive integer, onlyPositive is false")]
        [TestCase(17, 2, true, "0.0", TestName = "Input shorter than precision and scale > frac part")]
        [TestCase(4, 2, true, "10.11", TestName = "Precision = lenght of input (except point), scale = frac part")]
        [TestCase(4, 2, false, "+1", TestName = "OnlyPositive is false, num > 0, scale > 0, frac part is missing")]
        [TestCase(100, 0, false, "13", TestName = "Input shorter than precision")]
        public void IsValidNumber_ShouldReturnTrue_OnCorrectInput(
            int precision, int scale, bool onlyPositive, string value)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
        }

        #endregion
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