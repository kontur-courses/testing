using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {
        private NumberValidator correctPositiveOnlyNumberValidator;
        private NumberValidator correctNumberValidator;

        [SetUp]
        public void SetUp()
        {
            correctNumberValidator = new NumberValidator(3, 2);
            correctPositiveOnlyNumberValidator = new NumberValidator(3, 2, true);
        }

        [TestCase("+0.00", TestName = "WhenSignsNumberGreaterThanPrecision")]
        [TestCase("0.000", TestName = "WhenNumberOfDecimalPlacesGreaterThanScale")]
        [TestCase("0.0a", TestName = "WhenValueNotANumber")]
        [TestCase("-", TestName = "WhenValueNotANumber")]
        [TestCase(null, TestName = "WhenValueIsNull")]
        public void IsValidNumber_Fails_OnBothValidators(string value)
        {
            correctNumberValidator.IsValidNumber(value).Should().BeFalse();
            correctPositiveOnlyNumberValidator.IsValidNumber(value).Should().BeFalse();
        }

        [TestCase("0.00", TestName = "WhenZeros")]
        [TestCase("0", TestName = "WhenZero")]
        [TestCase("+1", TestName = "WhenPositiveNumber")]
        [TestCase("111", TestName = "WhenNumberWithoutSign")]
        public void IsValidNumber_Passes_OnBothValidators(string value)
        {
            correctNumberValidator.IsValidNumber(value).Should().BeTrue();
            correctPositiveOnlyNumberValidator.IsValidNumber(value).Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_Fails_WhenNegativeNumberOnPositiveOnlyValidator()
        {
            correctPositiveOnlyNumberValidator.IsValidNumber("-1.2").Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_Passes_WhenNegativeNumberOnNegativeValidator()
        {
            correctNumberValidator.IsValidNumber("-1.2").Should().BeTrue();
        }

        [TestCase(-1, 2, true, "precision must be a positive number",
             TestName = "When precision is negative")]
        [TestCase(1, -2, true, "scale must be a non-negative number less or equal than precision",
             TestName = "When scale is negative")]
        [TestCase(2, 2, true, "scale must be a non-negative number less or equal than precision",
             TestName = "When scale is bigger than precision")]
        public void NumberValidator_ThrowArgumentException(int precision, int scale, bool onlyPositive, string message)
        {
            new Action(() => new NumberValidator(precision, scale, onlyPositive))
                .ShouldThrow<ArgumentException>()
                .WithMessage(message);
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