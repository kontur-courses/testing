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
			numberValidator = new NumberValidator(4, 2, true);
		}

		[Test]
		public void ThrowException_WhenPrecisionNegativeNumber()
		{
			Action init = (() => new NumberValidator(-1, 2, false));
			init.Should().Throw<ArgumentException>();
		}

		[Test]
		public void DoesNotThrowException_WhenScaleZero()
		{
			Action init = (() => new NumberValidator(1, 0, false));
			init.Should().NotThrow();
		}

		[Test]
		public void ThrowException_WhenScaleMorePrecision()
		{
			Action init = (() => new NumberValidator(2, 3, false));
			init.Should().Throw<ArgumentException>();
		}

		[Test]
		public void False_WhenNumberLongerThenPrecision()
		{
			numberValidator.IsValidNumber("12.345").Should().Be(false);
		}

		[Test]
		public void False_WhenFractionalPartNumberLongerThanScale()
		{
			numberValidator.IsValidNumber("1.234").Should().Be(false);
		}

		[Test]
		public void False_WhenNumberValidatorOnlyForPositiveNumberButTakeNegative()
		{
			numberValidator.IsValidNumber("-1.2").Should().Be(false);
		}

		[Test]
		public void False_WhenNumberHaveNotDigitOrSignChar()
		{
			numberValidator.IsValidNumber("a.b").Should().Be(false);
		}

		[Test]
		public void False_WhenIncorrectFormatNumber()
		{
			numberValidator.IsValidNumber(".1").Should().Be(false);
		}

		[Test]
		public void True_WhenNumberHaveCorrectFormatWithoutFractalPart()
		{
			numberValidator.IsValidNumber("1").Should().Be(true);
		}

		[Test]
		public void True_WhenNumberHaveCorrectFormatWithFractalPart()
		{
			numberValidator.IsValidNumber("1.23").Should().Be(true);
		}

		[Test]
		public void True_WhenNumberWithSign()
		{
			numberValidator.IsValidNumber("+1.23").Should().Be(true);
		}

		[Test]
		public void True_WhenNumberValidatorNotOnlyForPositiveNumberButTakeNegative()
		{
			var numberValidatorWithNegativeNumber = new NumberValidator(4, 2, false);
			numberValidatorWithNegativeNumber.IsValidNumber("-1.23").Should().Be(true);
		}

		[Test]
		public void False_WhenStringNull()
		{
			numberValidator.IsValidNumber(null).Should().Be(false);
		}

		[Test]
		public void False_WhenStringEmpty()
		{
			numberValidator.IsValidNumber(string.Empty).Should().Be(false);
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