using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;


namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [TestCase(17, 2, true, "1.23", Description = "Correct Real number")]
        [TestCase(17, 2, true, "0", Description = "Correct Zero")]
        [TestCase(17, 2, false, "-0", Description = "Correct Zero")]
        [TestCase(4, 2, true, "+1.23", Description = "Correct Real positive number")]
        [TestCase(4, 2, false, "-1.23", Description = "Correct Real negative number")]
        public void ReturnTrue_TestOnMultipleParameters(int precision, int scale, bool onlyPositive, string expected)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(expected).Should().BeTrue();
        }

        [TestCase(3, 2, true, "00.00", Description = "Wrong number length")]
        [TestCase(4, 2, true, "-0.00", Description = "Wrong onlyPositive parameter")]
        [TestCase(17, 2, true, "0.000", Description = "Wrong scale length")]
        [TestCase(3, 2, true, "-1.23", Description = "Wrong precision length")]
        [TestCase(3, 2, true, "a.sd", Description = "Not a number value")]
        [TestCase(1, 0, true, null, Description = "Null value")]
        [TestCase(1, 0, true, "", Description = "Empty value")]
        public void ReturnFalse_TestOnMultipleParameters(int precision, int scale, bool onlyPositive, string expected)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(expected).Should().BeFalse();
        }


        [Test]
        public void ThrowsArgumentExceptionOnNegativePrecision()
        {
            Action action = () => new NumberValidator(-1, 2);
            action.ShouldThrow<ArgumentException>();

        }

        [Test]
        public void ThrowsArgumentExceptonOnNegativeScale()
        {
            Action action = () => new NumberValidator(2, -1);
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void ThrowsArgumentExceptionIfScaleGreaterThenPrecision()
        {
            Action action = () => new NumberValidator(1, 2);
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void DoesNotThrowExceptionOnZeroScale()
        {
            Action action = () => new NumberValidator(1, 0);
            action.ShouldNotThrow();
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