using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidator_Should
	{
		private NumberValidator validator;
		[OneTimeSetUp]
		public void OneTimeSetUp()
        {
			validator = new NumberValidator(10, 5);
        }

		[TestCase(-1, 0, false)]
		[TestCase(-2, -3, false)]
		[TestCase(-3, -2, true)]
		public void Constructor_OnNegativePrecision_ShouldThrowArgumentException(int precision, int scale, bool isPositive)
        {
			Action action = () => new NumberValidator(precision, scale, isPositive);
			action.Should().Throw<ArgumentException>();
        }

		[TestCase(10, 5, false)]
		[TestCase(10, 5, true)]
		[TestCase(1, 0, true)]
		[TestCase(2, 1, false)]
		[TestCase(5, 1, true)]
		[TestCase(100000, 1000, false)]
		public void Constructor_OnCorrectInitialization_ShouldNotThrow(int precision, int scale, bool onlyPositive)
        {
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().NotThrow();
		}

		[TestCase("d0.0")]
		[TestCase("=1.1")]
		[TestCase("!-1.1")]
		[TestCase("+-1.2")]
		public void IsValidNumber_OnIncorrectSign_ShouldBeFalse(string number)
        {
			validator.IsValidNumber(number).Should().BeFalse();
        }

		[TestCase("-1")]
		[TestCase("-1.01")]
		[TestCase("-0.1")]
		[TestCase("-1,2")]
		[TestCase("-0,1")]
		public void IsValidNumber_OnNegativeNumberWithOnlyPositiveIsTrue_ShouldBeFalse(string number)
        {
			var numberValidator = new NumberValidator(10, 5, true);
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[Test]
		[TestCase("123456")]
		[TestCase("1234.56")]
		[TestCase("123.456")]
		[TestCase("423256267345343453453")]
		[TestCase("-23456")]
		[TestCase("-456.88")]
		[TestCase("+456.33")]
		[TestCase("+12345")]
		public void IsValidNumber_OnNumbersLongerThanPrecision_ShouldBeFalse(string number)
        {
			var numberValidator = new NumberValidator(5, 3);
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		[TestCase("0.1111")]
		[TestCase("-0.1111")]
		[TestCase("0.0000")]
		public void IsValidNumber_OnNumbersWithFracLongerThanScale_ShouldBeFalse(string number)
        {
			var numberValidator = new NumberValidator(5, 3);
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		[TestCase("skb.kontur")]
		[TestCase("ayylmao")]
		[TestCase(null)]
		[TestCase("")]
		[TestCase("-1.1f1")]
		[TestCase("- . ")]
		[TestCase("-.")]
		[TestCase("-")]
		[TestCase(" ")]
		public void IsValidNumber_OnNotNumbers_ShouldBeFalse(string number)
        {
			validator.IsValidNumber(number).Should().BeFalse();
		}
		[TestCase("1d5")]
		[TestCase("1 5")]
		[TestCase("1]5")]
		[TestCase("1/5")]
		[TestCase("-1-5")]
		[TestCase("1=5")]
		public void IsValidNumber_WhenDotIsNotDot_ShouldBeFalse(string number)
        {
			validator.IsValidNumber(number).Should().BeFalse();
		}
		[TestCase("0.0")]
		[TestCase("0")]
		[TestCase("1.2")]
		[TestCase("12345.67891")]
		[TestCase("0.01")]
		[TestCase("+1.23")]
		[TestCase("1,5")]
		public void IsValidNumber_OnValidPositiveNumbersWithOnlyPositiveValidator_ShouldBeTrue(string number)
		{
			var numberValidator = new NumberValidator(10, 5, true);
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}
		[TestCase("1.5")]
		[TestCase("33,6")]
		[TestCase("-11.3")]
		[TestCase("+2.3")]
		[TestCase("-8.123")]
		[TestCase("+4.4004")]
		[TestCase("10000")]
		[TestCase("-1")]
		public void IsValidNumber_OnValidNumbers_ShouldBeTrue(string number)
        {
			validator.IsValidNumber(number).Should().BeTrue();
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
			numberRegex = new Regex(@"^(?<sign>[+-]?)(?<intPart>\d+)([.,](?<fracPart>\d+))?$", RegexOptions.IgnoreCase);
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

			var intAndSignPartLength = match.Groups["sign"].Value.Length + match.Groups["intPart"].Value.Length;
			var fracPartLength = match.Groups["fracPart"].Value.Length;

			if (intAndSignPartLength + fracPartLength > precision || fracPartLength > scale)
				return false;

			if (onlyPositive && match.Groups["sign"].Value == "-")
				return false;
			return true;
		}
	}
}