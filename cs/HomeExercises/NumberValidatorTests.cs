using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase("00.0")]
		[TestCase("01")]
		public void ShouldBeFalse_WhenTooManyZero_InHighestDigitOfIntegerPart(string value)
		{
			var validator = new NumberValidator(4, 2, true);
			validator.IsValidNumber(value).Should().BeFalse(
				"Because integer part can start with zero only if it is zero number");
		}

		[Test]
		public void Precision_ShouldConsiderMinusSign()
		{
			var validator = new NumberValidator(4, 0, true);
			var value = "-1024";
			validator.IsValidNumber(value).Should().BeFalse("Because minus sign is consider");
		}

		[Test]
		public void Precision_ShouldIgnorePlusSign()
		{
			var validator = new NumberValidator(4, 0, true);
			var value = "+1024";
			validator.IsValidNumber(value).Should().BeTrue("Because plus sign is ignored");
		}

		[Test]
		public void ShouldBeFalse_WhenMoreThenOneDelimiter()
		{
			var validator = new NumberValidator(5, 0, true);
			var value = "1.25.0";
			validator.IsValidNumber(value).Should().BeFalse("Because {0} is not a number", value);
		}

		[Test]
		public void ShouldBeFalse_OnNonNumberValue()
		{
			var validator = new NumberValidator(5, 0, true);
			var value = "1abc";

			validator.IsValidNumber(value).Should().BeFalse("Because {0} is not a number", value);
		}

		[Test]
		public void ShouldBeFalse_OnNegativeValue_WhenOnlyPositiveEnable()
		{
			var validator = new NumberValidator(2, 0, true);
			var value = "-5";

			validator.IsValidNumber(value).Should().BeFalse("Because {0} is not positive", value);
		}


		[Category("Validate Fractals Delimiter")]
		[TestCase("1.5")]
		[TestCase("1,5")]
		public void ShouldBeTrue_WhenDelimiterIsDotOrComma(string value)
		{
			var validator = new NumberValidator(2, 1, true);
			validator.IsValidNumber(value).Should().BeTrue("Because dot and comma are both correct");
		}

		[Category("Plus-minus zero")]
		[TestCase("-0")]
		[TestCase("-0.0")]
		[TestCase("+0")]
		[TestCase("+0.0")]
		public void ShouldBeFalse_WhenPlusOrMinusZero(string value)
		{
			var validator = new NumberValidator(2);
			validator.IsValidNumber(value).Should().BeFalse("Because zero with sign isn't correct");
		}

		[Category("Constructor")]
		[TestCase(1, 1)]
		[TestCase(4, 5)]
		[TestCase(2, -1)]
		public void ShouldThrow_WhenScaleIsNotCorrect(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}

		[Category("Constructor")]
		[TestCase(0)]
		[TestCase(-1)]
		public void ShouldThrow_WhenPrecisionIsNotCorrect(int precision)
		{
			Action action = () => new NumberValidator(precision);
			action.Should().Throw<ArgumentException>();
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

			if (IsValuePlusOrMinusZero(value))
				return false;

			if (HasValueTooManyZeroAtHighestDigit(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			var intPart = GetIntPartLength(match);
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}

		private int GetIntPartLength(Match match)
		{
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			intPart = match.Groups[1].Value.CompareTo("+") == 0 ? intPart - 1 : intPart;

			return intPart;
		}

		private bool IsValuePlusOrMinusZero(string value)
		{
			var regex = new Regex(@"^([+-])?(0{1})([.,])?");
			return regex.IsMatch(value);
		}

		private bool HasValueTooManyZeroAtHighestDigit(string value)
		{
			var regex = new Regex(@"^([+-])?(0+\d)");
			return regex.IsMatch(value);
		}
	}
}