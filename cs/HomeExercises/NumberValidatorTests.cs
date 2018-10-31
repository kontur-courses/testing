using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {

        [TestCase(-1, 0, "precision must be a positive number", TestName = "When Precision is negative")]
        [TestCase(0, 0, "precision must be a positive number", TestName = "When Precision is zero")]
        [TestCase(1, -1, "scale must be a non-negative number less or equal than precision", TestName = "When scale is negative")]
        [TestCase(1, 2, "scale must be a non-negative number less or equal than precision", TestName = "When scale is greater than precision")]
        [TestCase(1, 1, "scale must be a non-negative number less or equal than precision", TestName = "When scale is equal to precision")]
        public void Constructor_ThrowsArgumentException(int precision, int scale, string message)
        {
            Action constructor = () => new NumberValidator(precision, scale);

            constructor.Should().Throw<ArgumentException>()
                .WithMessage(message);
        }

        [TestCase(" ", TestName = "When input is empty")]
        [TestCase(null, TestName = "When input is null")]
        [TestCase("0.000", TestName = "When fractal part is greater than scale")]
        [TestCase("1234567", TestName = "When int part is greater than precision")]
        [TestCase("-1.-2", TestName = "When input isn't a number")]
        [TestCase("-1", true, TestName = "When validator is only for positive and input is negative")]
        public void IsValidNumber_ReturnFalse(string input, bool onlyPositive = false)
        {
            var validator = new NumberValidator(6, 2, onlyPositive);

            validator.IsValidNumber(input).Should().BeFalse();
        }

        [TestCase("-0", TestName = "When int part length less than precision")]
        [TestCase("-12345", TestName = "When int part length is equal to precision")]
        [TestCase("0", TestName = "When fractal part length is zero")]
        [TestCase("-0.0", TestName = "When fractal part length less than scale")]
        [TestCase("0.00", TestName = "When fractal part length is equal to scale")]
        [TestCase("1", true, TestName = "When validator is only for positive and input is positive")]
        public void IsValidNumber_ReturnTrue(string input, bool onlyPositive = false)
        {
            var validator = new NumberValidator(6, 2, onlyPositive);

            validator.IsValidNumber(input).Should().BeTrue();
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