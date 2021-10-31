using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidator_Should
	{
		private NumberValidator validator;
		[SetUp]
		public void SetUpUniversalCorrect10n5NotOnlyPositiveValidator()
        {
			validator = new NumberValidator(10, 5);
        }

		[Test]
		public void Constructor_OnNegativePrecision_ThrowsArgumentException()
        {
			Action t1 = () => new NumberValidator(-1);
			Action t2 = () => new NumberValidator(-2, -3);
			Action t3 = () => new NumberValidator(-3, -2, true);
			t1.Should().Throw<ArgumentException>();
			t2.Should().Throw<ArgumentException>();
			t3.Should().Throw<ArgumentException>();
        }

		[Test]
		public void IsValidNumber_OnValidPositiveNumbersWithOnlyPositiveValidator_ShouldBeTrue()
        {
			var numberValidator = new NumberValidator(10, 5, true);
			numberValidator.IsValidNumber("0.0").Should().BeTrue();
			numberValidator.IsValidNumber("0").Should().BeTrue();
			numberValidator.IsValidNumber("1.2").Should().BeTrue();
			numberValidator.IsValidNumber("12345.67891").Should().BeTrue();
			numberValidator.IsValidNumber("0.01").Should().BeTrue();
			numberValidator.IsValidNumber("+1.23").Should().BeTrue();
			numberValidator.IsValidNumber("1,5").Should().BeTrue();
        }

		[Test]
		public void Constructor_OnCorrectInitialization_DoesNotThrow()
        {
            List<Action> actions = new List<Action>
            {
                () => new NumberValidator(10, 5, false),
                () => new NumberValidator(10, 5, true),
                () => new NumberValidator(1, 0, true),
                () => new NumberValidator(2, 1, false),
                () => new NumberValidator(5, 1, true),
                () => new NumberValidator(10000, 1000, false)
            };
            foreach (var action in actions)
            {
				action.Should().NotThrow();
            }
		}

		[Test]
		public void IsValidNumber_OnIncorrectSign_ShouldBeFalse()
        {
			validator.IsValidNumber("d0.0").Should().BeFalse();
			validator.IsValidNumber("=1.1").Should().BeFalse();
			validator.IsValidNumber("!-1.1").Should().BeFalse();
			validator.IsValidNumber("+-1.2").Should().BeFalse();
        }

		[Test]
		public void IsValidNumber_OnNegativeNumberWithOnlyPositiveIsTrue_ShouldBeFalse()
        {
			var numberValidator = new NumberValidator(10, 5, true);
			numberValidator.IsValidNumber("-1").Should().BeFalse();
			numberValidator.IsValidNumber("-1.01").Should().BeFalse();
			numberValidator.IsValidNumber("-0.1").Should().BeFalse();
			numberValidator.IsValidNumber("-1,2").Should().BeFalse();
			numberValidator.IsValidNumber("-0,1").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_OnNumbersLongerThanPrecision_ShouldBeFalse()
        {
			var numberValidator = new NumberValidator(5, 3);
			numberValidator.IsValidNumber("123456").Should().BeFalse();
			numberValidator.IsValidNumber("1234.56").Should().BeFalse();
			numberValidator.IsValidNumber("123.456").Should().BeFalse();
			numberValidator.IsValidNumber("12345634534534534534534534").Should().BeFalse();
			numberValidator.IsValidNumber("-23456").Should().BeFalse();
			numberValidator.IsValidNumber("-456.99").Should().BeFalse();
			numberValidator.IsValidNumber("+456.99").Should().BeFalse();
			numberValidator.IsValidNumber("+12345").Should().BeFalse();
		}
		[Test]
		public void IsValidNumber_OnNumbersWithFracLongerThanScale_ShouldBeFalse()
        {
			var numberValidator = new NumberValidator(5, 3);
			numberValidator.IsValidNumber("0.1111").Should().BeFalse();
			numberValidator.IsValidNumber("-0.1111").Should().BeFalse();
			numberValidator.IsValidNumber("0.0000").Should().BeFalse();
		}
		[Test]
		public void IsValidNumber_OnNotNumbers_ShouldBeFalse()
        {
			validator.IsValidNumber("skb.kontur").Should().BeFalse();
			validator.IsValidNumber("ayylmao").Should().BeFalse();
			validator.IsValidNumber(null).Should().BeFalse();
			validator.IsValidNumber("").Should().BeFalse();
			validator.IsValidNumber("-1.1f1").Should().BeFalse();
			validator.IsValidNumber("- . ").Should().BeFalse();
			validator.IsValidNumber("-.").Should().BeFalse();
			validator.IsValidNumber("-").Should().BeFalse();
			validator.IsValidNumber(" ").Should().BeFalse();
		}
		[Test]
		public void IsValidNumber_WhenDotIsNotDot_ShouldBeFalse()
        {
			validator.IsValidNumber("1d5").Should().BeFalse();
			validator.IsValidNumber("1 5").Should().BeFalse();
			validator.IsValidNumber("1]5").Should().BeFalse();
			validator.IsValidNumber("1/5").Should().BeFalse();
			validator.IsValidNumber("-1-5").Should().BeFalse();
			validator.IsValidNumber("1=5").Should().BeFalse();
		}
		[Test]
		public void IsValidNumber_OnValidNumbers_ShouldBeTrue()
        {
			validator.IsValidNumber("1.5").Should().BeTrue();
			validator.IsValidNumber("33,6").Should().BeTrue();
			validator.IsValidNumber("-11.3").Should().BeTrue();
			validator.IsValidNumber("+2.3").Should().BeTrue();
			validator.IsValidNumber("-8.123").Should().BeTrue();
			validator.IsValidNumber("+4.4004").Should().BeTrue();
			validator.IsValidNumber("10000").Should().BeTrue();
			validator.IsValidNumber("-1").Should().BeTrue();
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