using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private static int precision = 5;
		private static int scale = 2;
		private static NumberValidator numberValidator = new NumberValidator(precision, scale);
		private static string restrictionsMessage = $"Precision: {precision}, Scale: {scale}";

		[TestCase(1, 1)]
		[TestCase(1, -1)]
		[TestCase(0, 1)]
		public void ShouldThrow_When_IncorrectPrecisionOrScaleRestrictions(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}

		[TestCase("+1")]
		[TestCase("-1")]
		public void ShouldBeValidNumber_WhenOneSign(string value)
		{
			numberValidator.IsValidNumber(value).Should().BeTrue();
		}

		[Test]
		public void ShouldNotBeValidNumber_WhenMoreThanOneSign()
		{
			numberValidator.IsValidNumber("+-1").Should().BeFalse();
		}

		[TestCase("1,1")]
		[TestCase("1.1")]
		public void ShouldBeValidNumber_WhenSeparated(string value)
		{
			numberValidator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase("")]
		[TestCase(null)]
		[TestCase("-.0")]
		[TestCase("+1,")]
		[TestCase("?1a.2b")]
		public void ShouldNotBeValidNumber_WhenIncorrectValue(string value)
		{
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		public void ShouldNotBeValidNumber_WhenOnlyPositiveAndMinus()
		{
			new NumberValidator(5, 2, true).IsValidNumber("-1").Should().BeFalse();
		}

		[TestCase("00000")]
		[TestCase("000.00")]
		[TestCase("+123.45")]
		[TestCase("-12.34")]
		public void ShouldBeValidNumber_When_CorrectPrecisionAndScale(string value)
		{
			numberValidator.IsValidNumber(value).Should().BeTrue(restrictionsMessage);
		}

		[TestCase("123456")]
		[TestCase("1234.56")]
		[TestCase("12.356")]
		[TestCase("-123.45")]
		public void ShouldNotBeValidNumber_When_IncorrectPrecisionOrScale(string value)
		{
			numberValidator.IsValidNumber(value).Should().BeFalse(restrictionsMessage);
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