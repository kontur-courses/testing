using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private NumberValidator numberValidator;

		[SetUp]
		public void SetUp()
		{
			numberValidator = new NumberValidator(3, 2);
		}


		[Test]
		[TestCase(-1, 0, true, TestName = "negative precision")]
		[TestCase(0, 0, true, TestName = "zero precision")]
        public void Constructor_ThrowsArgumentException_WhenPrecisionIsNotPositive(int precision, int scale, bool onlyPositive)
		{
			Action create = () => new NumberValidator(precision, scale, onlyPositive);
			create.ShouldThrowExactly<ArgumentException>($"precision {precision} is not positive")
				.WithMessage("*precision*");
		}

        [Test]
        [TestCase(1, -1, true, TestName = "scale less than zero")]
        [TestCase(1, 2, true, TestName = "scale bigger than precision")]
        [TestCase(1, 1, true, TestName = "scale equals precision")]
        public void Constructor_ThrowsArgumentException_WhenScaleIsIncorrect(int precision, int scale, bool onlyPositive)
        {
	        Action create = () => new NumberValidator(precision, scale, onlyPositive);
	        create.ShouldThrowExactly<ArgumentException>($"scale {scale} can't be with precision {precision}")
		        .WithMessage("*scale*");
        }

        [Test]
        [TestCase(1, 0, true, TestName = "correct integer")]
        [TestCase(2, 1, true, TestName = "correct non-integer")]
        public void Constructor_DoesNotThrowException_WhenInputIsCorrect(int precision, int scale, bool onlyPositive)
        {
	        Action create = () => new NumberValidator(precision, scale, onlyPositive);
	        create.ShouldNotThrow($"both precision {precision} and scale {scale} are correct");
        }

        [Test]
        [TestCase("0", TestName = "correct unsigned int")]
        [TestCase("0.0", TestName = "correct unsigned non-int")]
        [TestCase("+1.2", TestName = "correct positive")]
        [TestCase("-1.2", TestName = "correct negative")]
        [TestCase("0,0", TestName = "correct number with comma")]
        public void IsValid_True_OnCorrectInput(string value)
        {
	        numberValidator.IsValidNumber(value).Should().BeTrue($"{value} is correct argument");
        }

        [Test]
        [TestCase("0.000", TestName = "too big scale")]
        [TestCase("0000", TestName = "too big precision")]
        [TestCase("00.00", TestName = "length is bigger than precision")]
        [TestCase("+0.00", TestName = "length with sign is bigger than precision")]
        [TestCase("a.sd", TestName = "letters instead of digits")]
        [TestCase("--0.0", TestName = "too many signs")]
        [TestCase("", TestName = "empty string")]
        [TestCase(null, TestName = "null input")]
        [TestCase("0.", TestName = "empty fraction")]
        public void IsValid_False_OnIncorrectInput(string value)
        {
	        numberValidator.IsValidNumber(value).Should().BeFalse($"{value} is incorrect argument");
        }

        [Test]
        [TestCase("-0.0", TestName = "negative zero")]
        [TestCase("-2", TestName = "negative integer")]
        public void IsValid_False_OnNegativeWhenCreatedWithOnlyPositive(string value)
        {
	        numberValidator = new NumberValidator(3, 2, true);
	        numberValidator.IsValidNumber(value).Should().BeFalse($"{value} is negative");
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