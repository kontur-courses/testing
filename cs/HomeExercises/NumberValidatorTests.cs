using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        private NumberValidator sutValidator;

        [SetUp]
        public void SetUp()
        {
            sutValidator = new NumberValidator(3, 2, false);
        }

        [TestCase("12", TestName = "Positive number without plus sign")]
        [TestCase("+12", TestName = "Positive number with plus sign")]
        public void NumberValidator_ShouldValidateNumbers_WithCorrectSign(string input)
        {
            sutValidator.IsValidNumber(input).Should().BeTrue();
        }

        [TestCase("1234", TestName = "Number without fractional part")]
        [TestCase("123.4", TestName = "Number with fractional part")]
        [TestCase("-13.4", TestName = "Negative number")]
        public void NumberValidator_ShouldNotValidateNumber_WhenPrecisionIsNotCorrect(string input)
        {
            sutValidator.IsValidNumber(input).Should().BeFalse();
        }

        [TestCase("0.000000000", TestName = "Zero with many trailing zeros")]
        [TestCase("213.00000000", TestName = "Positive number with many trailng zeros")]
        [TestCase("-123.100000", TestName = "Negative number with many trailing zeros")]
        public void NumberValidator_ShouldValidateNumber_WithTrailingZeroes(string input)
        {
            sutValidator.IsValidNumber(input).Should().BeFalse();
        }

        [TestCase("13,3", TestName = "Positive number with comma as a delimiter")]
        [TestCase("-1,4", TestName = "Negative number with comma as a delimiter")]
        [TestCase("0,00", TestName = "Zero with comma as a delimiter")]
        public void NumberValidator_ShouldValidateNumber_WithDifferentFractionalPartDelimiters(string input)
        {
            sutValidator.IsValidNumber(input).Should().BeTrue();
        }

        [Test]
        public void NumberValidator_ShouldNotValidateNegativeNumbers_WhenOnlyPositiveFlagIsTrue()
        {
            var sutOnlyPositiveValidator = new NumberValidator(3, 2, true);
            sutOnlyPositiveValidator.IsValidNumber("-12").Should().BeFalse();
        }

        [TestCase("12.3456", TestName = "Number with correct precision and incorrect scale")]
        public void NumberValidator_ShouldNotValidateNumber_WhenScaleIsNotCorrect(string input)
        {
            var sutBigPrecisionValidator = new NumberValidator(10, 3);
            sutBigPrecisionValidator.IsValidNumber(input).Should().BeFalse();
        }

        [TestCase("", TestName = "Empty string")]
        [TestCase(null, TestName = "Null")]
        [TestCase("asdfsa", TestName = "String without digits")]
        [TestCase("12das fd", TestName = "String with digits at the start")]
        [TestCase("dsa12", TestName = "String with digits at the end")]
        [TestCase("ads13das", TestName = "String with digits inside letters")]
        public void NumberValidator_ShouldNotValidateNonNumbers(string input)
        {
            sutValidator.IsValidNumber(input).Should().BeFalse();
        }

        [Test]
        public void NumberValidator_ShouldValidateNegativeZero()
        {
            sutValidator.IsValidNumber("-0.0").Should().BeTrue();
        }

        [Test]
        public void NumberValidator_ShouldValidateZero()
        {
            sutValidator.IsValidNumber("0").Should().BeTrue();
        }

        [Test]
        public void NumberValidator_ShouldValidateNumber_WithLeadingZeroes()
        {
            sutValidator.IsValidNumber("001").Should().BeTrue();
        }

        [TestCase(1, TestName = "Positive precision")]
        [TestCase(0, TestName = "Zero precision")]
        public void NumberValidator_ShouldThrowAnException_OnNonPositivePrecision(int precision)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, 5));
        }

        [Test]
        public void NumberValidator_ShouldThrowAnException_OnNegativeScale()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(1, -2, false));
        }

        [Test]
        public void NumberValidator_ShouldThrowAnException_WhenPrecisionIsLessThanScale()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(2, 3, false));
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
                throw new ArgumentException("precision must be a non-negative number less or equal than scale");
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