using System;
using System.Text.RegularExpressions;

using FluentAssertions;

using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [TestCase(-1, 0, TestName = "Precision is negative")]
        [TestCase(0, 0, TestName = "Precision is equal to zero")]
        [TestCase(1, -1, TestName = "Scale is negative")]
        [TestCase(1, 1, TestName = "Scale is equal to precision")]
        [TestCase(1, 2, TestName = "Scale is greater than precision")]
        public void Constructor_Should_ThrowException(int precision, int scale)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
        }

        [TestCase(1, 0, TestName = "Scale is zero")]
        [TestCase(17, 3, TestName = "Scale is greater than zero less than precision")]
        public void Constructor_Should_CreateValidator(int precision, int scale)
        {
            Assert.DoesNotThrow(() => new NumberValidator(precision, scale));
        }

        [TestCase(2, 1, true, "0.0", TestName = "Unsigned Fractional number with dot")]
        [TestCase(2, 1, true, "0,0", TestName = "Unsigned Fractional number with comma")]
        [TestCase(1, 0, true, "0", TestName = "Unsigned integer number")]
        [TestCase(2, 0, true, "+0", TestName = "Signed integer number (+)")]
        [TestCase(3, 1, true, "+0.0", TestName = "Signed fractional number with dot (+)")]
        [TestCase(3, 1, true, "+0,0", TestName = "Signed fractional number with comma (+)")]
        [TestCase(2, 0, false, "-0", TestName = "Signed number (-) negative allowed")]
        [TestCase(3, 1, false, "-0.0", TestName = "Signed fractional number with dot (-) negative allowed")]
        [TestCase(3, 1, false, "-0,0", TestName = "Signed fractional number with comma (-) negative allowed")]
        public void IsValidNumber_Should_BeTrue(int precision, int scale, bool onlyPositive, string expression)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);

            var actual = validator.IsValidNumber(expression);

            actual.Should().BeTrue();
        }

        [TestCase(3, 2, false, "00.00", TestName = "Precision is too small")]
        [TestCase(17, 2, false, "0 0", TestName = "White space between digits")]
        [TestCase(17, 10, false, "0.0 0", TestName = "White space between fraction part digits")]
        [TestCase(3, 0, false, "0.0", TestName = "Expected integer, but got fractional number")]
        [TestCase(3, 2, false, "-0.00", TestName = "Precision is too small, sign counts (-)")]
        [TestCase(3, 2, false, "+0.00", TestName = "Precision is too small, sign counts (+)")]
        [TestCase(17, 2, false, "0.000", TestName = "Scale is too small")]
        [TestCase(17, 2, true, "-0.0", TestName = "Only positive but got negative number")]
        [TestCase(3, 2, false, "a.sd", TestName = "Not a number with dot")]
        [TestCase(3, 2, false, "asd", TestName = "Not a integer number")]
        [TestCase(3, 2, false, null, TestName = "Get null")]
        [TestCase(3, 2, false, "", TestName = "Get empty string")]
        [TestCase(3, 2, false, " ", TestName = "Get white space")]
        [TestCase(3, 2, false, ".", TestName = "Get only dot")]
        [TestCase(3, 2, false, ".0", TestName = "Empty integer part")]
        [TestCase(3, 2, false, "0.", TestName = "Empty fractional part, but with dot")]
        public void IsValidNumber_Should_BeFalse(int precision, int scale, bool onlyPositive, string expression)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);

            var actual = validator.IsValidNumber(expression);

            actual.Should().BeFalse();
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