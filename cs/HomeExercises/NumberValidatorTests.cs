using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		public NumberValidator numberValidator = new NumberValidator(5, 2);

		[Test]
		public void ShouldThrow_When_Scale_GreaterOrEqualPrecision()
		{
			Action action = () => new NumberValidator(1, 1);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void ShouldThrow_WhenScaleNegative()
		{
			Action action = () => new NumberValidator(1, -1);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void ShouldThrow_WhenPrecisionNonPositive()
		{
			Action action = () => new NumberValidator(0, 1);
			action.Should().Throw<ArgumentException>().WithMessage("precision must be a positive number");
		}

		[Test]
		public void ShouldBeValidNumber_WhenOnePlusSign()
		{
			numberValidator.IsValidNumber("+1").Should().BeTrue();
		}

		[Test]
		public void ShouldBeValidNumber_WhenOneMinusSign()
		{
			numberValidator.IsValidNumber("-1").Should().BeTrue();
		}

		[Test]
		public void ShouldNotBeValidNumber_WhenMoreThanOneSign()
		{
			numberValidator.IsValidNumber("+-1").Should().BeFalse();
		}

		[Test]
		public void ShouldBeValidNumber_WhenCommaSeparated()
		{
			numberValidator.IsValidNumber("1,1").Should().BeTrue();
		}

		[Test]
		public void ShouldBeValidNumber_WhenDotSeparated()
		{
			numberValidator.IsValidNumber("1.1").Should().BeTrue();
		}

		[Test]
		public void ShouldNotBeValidNumber_WhenEmpty()
		{
			numberValidator.IsValidNumber("").Should().BeFalse();
		}

		[Test]
		public void ShouldNotBeValidNumber_WhenNull()
		{
			numberValidator.IsValidNumber(null).Should().BeFalse();
		}

		[Test]
		public void ShouldNotBeValidNumber_WhenHasNotInt()
		{
			numberValidator.IsValidNumber("-.0").Should().BeFalse();
		}

		[Test]
		public void ShouldNotBeValidNumber_WhenHasSeparatorAndHasNotFrac()
		{
			numberValidator.IsValidNumber("+1,").Should().BeFalse();
		}

		[Test]
		public void ShouldNotBeValidNumber_WhenHasNonNumericCharacters()
		{
			numberValidator.IsValidNumber("?1a.2b").Should().BeFalse();
		}

		[Test]
		public void ShouldNotBeValidNumber_WhenOnlyPositiveAndMinus()
		{
			new NumberValidator(5, 2, true).IsValidNumber("-1").Should().BeFalse();
		}

		[Test]
		public void ShouldBeValidNumber_When_CountIntDigits_EqualPrecision()
		{
			numberValidator.IsValidNumber("00000").Should().BeTrue();
		}

		[Test]
		public void ShouldBeValidNumber_When_CountIntDigitsAndFracDigits_EqualPrecision()
		{
			numberValidator.IsValidNumber("000.00").Should().BeTrue();
		}

		[Test]
		public void ShouldNotBeValidNumber_When_CountIntDigits_GreaterThanPrecision()
		{
			numberValidator.IsValidNumber("123456").Should().BeFalse();
		}

		[Test]
		public void ShouldNotBeValidNumber_When_CountIntDigitsAndFracDigits_GreaterThanPrecision()
		{
			numberValidator.IsValidNumber("1234.56").Should().BeFalse();
		}

		[Test]
		public void ShouldNotBeValidNumber_When_FracDigits_GreaterThanScale()
		{
			numberValidator.IsValidNumber("12.356").Should().BeFalse();
		}

		[Test]
		public void ShouldNotBeValidNumber_When_CountIntDigitsAndFracDigitsAndMinusSign_GreaterThanPrecision()
		{
			numberValidator.IsValidNumber("-123.45").Should().BeFalse();
		}

		[Test]
		public void ShouldBeValidNumber_When_CountIntDigitsAndFracDigitsAndPlusSign_EqualPrecisionPlusOne()
		{
			numberValidator.IsValidNumber("+123.45").Should().BeTrue();
		}

		[Test]
		public void ShouldBeValidNumber_When_CountIntDigitsAndFracDigitsAndSign_EqualPrecision()
		{
			numberValidator.IsValidNumber("-12.34").Should().BeTrue();
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