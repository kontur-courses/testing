using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(2, TestName = "precision is positive")]
		[TestCase(2, 0, TestName = "scale is zero")]
		[TestCase(2, 1, TestName = "scale is positive and less then precision")]
		[TestCase(2, 1, true, TestName = "onlyPositive is true")]
		[TestCase(2, 1, false, TestName = "onlyPositive is false")]
		public void Constructor_OnValidParameters_DoesNotThrowException(int precision, int scale = 0, bool onlyPositive = true)
		{
			Action creation = () => new NumberValidator(precision, scale, onlyPositive);
			creation.Should().NotThrow();
		}

		[TestCase(-1, TestName = "precision is negative")]
		[TestCase(0, TestName = "precision is zero")]
		[TestCase(1, -1, TestName = "scale is negative")]
		[TestCase(1, 1, TestName = "scale is equal to precision")]
		[TestCase(1, 2, TestName = "scale is greater then precision")]
		public void Constructor_OnInvalidParameters_ThrowsArgumentException(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action creation = () => new NumberValidator(precision, scale, onlyPositive);
			creation.Should().Throw<ArgumentException>();
		}
		
		[TestCase("1234", 4, 2, true, TestName = "integer with max lenght")]
		[TestCase("123.4", 4, 2, true, TestName = "fractional with max lenght")]
		[TestCase("1.23", 4, 2, true, TestName = "fractional with max fractional part lenght")]
		[TestCase("1.0", 4, 2, true, TestName = "positive value when onlyPositive is true")]
		[TestCase("+1.0", 4, 2, true, TestName = "positive value with sign when onlyPositive is true")]
		[TestCase("1.0", 4, 2, false, TestName = "positive value when onlyPositive is false")]
		[TestCase("+1.0", 4, 2, false, TestName = "positive value with sign when onlyPositive is false")]
		[TestCase("-1.0", 4, 2, false, TestName = "negative value when onlyPositive is false")]
		[TestCase("1,0", 4, 2, true, TestName = "value with coma")]
		public void IsValidNumber_OnValidParameters_ReturnsTrue(
			string value, 
			int precision, 
			int scale = 0,
			bool onlyPositive = true)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeTrue();
		}
		
		[TestCase("ab.cd", TestName = "value includes letters")]
		[TestCase("@#.$*", TestName = "value includes special symbols")]
		[TestCase(".1", TestName = "fractional part without integer part")]
		[TestCase("1.", TestName = "integer part with dot, but without fractional part")]
		[TestCase("1+", TestName = "sign in wrong place")]
		[TestCase("+", TestName = "plus without integer part")]
		[TestCase("-", TestName = "minus without integer part")]
		[TestCase("++1", TestName = "more then one plus")]
		[TestCase("--1", TestName = "more then one minus")]
		[TestCase("+-1", TestName = "plus and minus in one value")]
		[TestCase("1.2.3", TestName = "more then one dot")]
		[TestCase("1.2,3", TestName = "dot and coma in one value")]
		public void IsValidNumber_OnMismatchingValueFormat_ReturnsFalse(string value)
		{
			var numberValidator = new NumberValidator(10, 5);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("", TestName = "empty value")]
		[TestCase("  ", TestName = "whitespace value")]
		[TestCase(null, TestName = "null value")]
		public void isValidNumber_OnEmptyOrNullValue_ReturnsFalse(string value)
		{
			var numberValidator = new NumberValidator(10, 5);
			numberValidator.IsValidNumber(value).Should().BeFalse();
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