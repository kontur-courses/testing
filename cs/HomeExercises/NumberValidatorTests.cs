using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
        [Test]
        public void Constructor_ThrowsArgumentExceptionWithMessage_IfPrecisionLessThanZero()
        {
            FluentActions
                .Invoking(() => new NumberValidator(-1))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("precision must be a positive number");
        }

        [Test]
        public void Constructor_ThrowsArgumentExceptionWithMessage_IfPrecisionEqualsZero()

        {
            FluentActions
                .Invoking(() => new NumberValidator(0))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("precision must be a positive number");
        }

        [Test]
        public void Constructor_ThrowsArgumentExceptionWithMessage_IfScaleLessThanZero()
        {
            FluentActions
                .Invoking(() => new NumberValidator(1, -1, true))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("precision must be a non-negative number less or equal than precision");
        }

        [Test]
        public void Constructor_ThrowsArgumentExceptionWithMessage_IfScaleEqualsPrecision()
        {
            FluentActions
                .Invoking(() => new NumberValidator(1, 1, true))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("precision must be a non-negative number less or equal than precision");
        }

        [Test]
        public void Constructor_ThrowsArgumentExceptionWithMessage_IfScaleMoreThanPrecision()
        {
            FluentActions
                .Invoking(() => new NumberValidator(1, 2, true))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("precision must be a non-negative number less or equal than precision");
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_IfInputStringIsNull()
        {
            new NumberValidator(3, 2, true).IsValidNumber(null).Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_IfInputStringIsEmpty()
        {
            new NumberValidator(3, 2, true).IsValidNumber(String.Empty).Should().BeFalse();
        }

        [TestCase(" +1")]
        [TestCase("1+1")]
        [TestCase("++1")]
        [TestCase("+ 1")]
        [TestCase("+1+")]
        [TestCase("+ ")]
        [TestCase("+1 ")]
        [TestCase("+a")]
        [TestCase("a")]
        [TestCase(".")]
        [TestCase(".1")]
        [TestCase("1.")]
        [TestCase("1..")]
        [TestCase("1+")]
        [TestCase("1 ")]
        [TestCase("1a")]
        [TestCase("1.+")]
        [TestCase("1. ")]
        [TestCase("1.a")]
        [TestCase("1.1.")]
        [TestCase("1.1+")]
        [TestCase("1.1 ")]
        [TestCase("1.1a")]
        public void IsValidNumber_ReturnsFalse_IfRegexMatchIsNotSuccess(string inputString)
        {
            new NumberValidator(10).IsValidNumber(inputString).Should().BeFalse();
        }

        [TestCase("0")]
        [TestCase("0.0")]
        [TestCase("00.00")]
        [TestCase("+0")]
        [TestCase("+0.0")]
        [TestCase("+00.00")]
        public void IsValidNumber_ReturnsTrue_IfInputStringRepresentsValidPositiveNumber(string inputString)
        {
	        new NumberValidator(10, 5, true).IsValidNumber(inputString).Should().BeTrue();
        }

        [TestCase("00.00")]
        [TestCase("+0.00")]
        public void IsValidNumber_ReturnsFalse_IfNumberLengthMoreThanPrecision(string inputString)
        {
            new NumberValidator(3, 2, true).IsValidNumber(inputString).Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_IfFractionalPartLengthMoreThanScale()
        {
            new NumberValidator(10, 2, true).IsValidNumber("0.000").Should().BeFalse();
        }

        [TestCase("-1")]
        [TestCase("-1.0")]
        [TestCase("-1.00")]
        public void IsValidNumber_ReturnsTrue_IfInputStringRepresentsValidNegativeNumber(string inputString)
        {
            new NumberValidator(10, 5, false).IsValidNumber(inputString).Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_IfInputStringRepresentsValidNegativeNumberAndOnlyPositiveIsTrue()
        {
            new NumberValidator(10, 5, true).IsValidNumber("-1").Should().BeFalse();
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