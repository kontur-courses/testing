using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        [TestCase(17, 2, true, null, false, TestName = "number is null")]
        [TestCase(17, 2, true, "", false, TestName = "number is empty")]
        [TestCase(17, 4, true, "/89", false, TestName = "number starts with wrong sign")]
        [TestCase(17, 4, true, "0.0.0", false, TestName = "two dots")]
        [TestCase(17, 4, true, ".0", false, TestName = "without intPart")]
        [TestCase(17, 2, true, "0.", false, TestName = "without fracPart")]
        [TestCase(17, 2, true, "++2", false, TestName = "too many signes")]
        [TestCase(17, 2, true, "ab.c", false, TestName = "letters instead of numbers")]
        [TestCase(17, 2, true, "-2.3", false, TestName = "expected only positive but was negative number")]
        [TestCase(17, 2, true, "2.333", false, TestName = "number length after dot > scale")]
        [TestCase(17, 2, false, "-2.3", true, TestName = "negative number")]
        [TestCase(17, 2, true, "2,3", true, TestName = "use comma")]
        [TestCase(17, 2, true, "2.3", true, TestName = "regular number")]
        [TestCase(17, 2, false, "-2.3", true, TestName = "regular double negative number")]
        [TestCase(17, 2, true, "5", true, TestName = "regular int number")]
        [TestCase(3, 2, true, "00.00", false, TestName = "number length > precision")]
        [TestCase(3, 2, true, "-0.00", false, TestName = "number length with sign > precision")]
        [TestCase(3, 0, true, "2.33", false, TestName = "expected int but was double")]
        public void Check_Number_Correctness_When(int precision, int scale, bool onlyPositive, string number, bool expected)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            validator.IsValidNumber(number).Should().Be(expected);
        }

        [Test]
        [TestCase(-1, 2, true, TestName = "negative precision")]
        [TestCase(1, -2, true, TestName = "negative scale")]
        [TestCase(3, 8, true, TestName = "precision < scale")]
        [TestCase(0, 0, true, TestName = "precision = 0")]
        [TestCase(2, 2, true, TestName = "precision = scale")]
        public void NumberValidator_ThrowException_On(int precision, int scale, bool onlyPositive)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
        }

        [Test]
        [TestCase(1, 0, true, TestName = "scale = 0")]
        [TestCase(1, 0, false, TestName = "not only positive numbers")]
        public void NumberValidator_NotThrowException_On(int precision, int scale, bool onlyPositive)
        {
            Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
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