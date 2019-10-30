using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [TestCase(0, 0, Description = "Precision is zero")]
        [TestCase(-17, 0, Description = "Precision is a negative number")]
        [TestCase(1, -2, Description = "Scale is a negative number")]
        [TestCase(3, 3, Description = "Scale is equal to precision")]
        [TestCase(2, 3, Description = "Scale is more than precision")]
        public void NumberValidator_ThrowsArgumentException_CreationWithIncorrectArguments(int precision, int scale)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
        }

        [TestCase("0", Description = "Integer")]
        [TestCase("10.01", Description = "Fraction number with a point as a separator")]
        [TestCase("10,5", Description = "Fraction number with a comma as a separator")]
        [TestCase("+1.23", Description = "Number with a plus sign")]
        [TestCase("-9.99", Description = "Number with a minus sign")]
        [TestCase("1", true, Description = "Positive number without a plus sign")]
        [TestCase("+2", true, Description = "Positive number with a plus sign")]
        public void IsValidNumber_ReturnsTrue_CheckingCorrectNumbers(string value, bool onlyPositive = false)
        {
            Assert.IsTrue(new NumberValidator(4, 2, onlyPositive).IsValidNumber(value));
        }

        [TestCase("0.000", Description = "Number with a long fractional part")]
        [TestCase("+1.23", Description = "Long number with a plus sign")]
        [TestCase("-1.23", Description = "Long number with a minus sign")]
        [TestCase("-1", true, Description = "Non positive number")]
        [TestCase("a.sd", Description = "String contains letters")]
        [TestCase("", Description = "String is empty")]
        [TestCase(null, Description = "Value is null")]
        public void IsValidNumber_ReturnsFalse_CheckingIncorrectNumbers(string value, bool onlyPositive = false)
        {
            Assert.IsFalse(new NumberValidator(3, 2, onlyPositive).IsValidNumber(value));
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