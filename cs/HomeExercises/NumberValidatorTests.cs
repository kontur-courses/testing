using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(0, TestName = "Zero precision")]
		[TestCase(-1, TestName = "Negative precision")]
		[TestCase(1, -1, TestName = "Negative scale")]
		[TestCase(1, 10, TestName = "Scale is greater than precision")]
		public void Constructor_ThrowsArgumentException_OnIncorrectParameters(int precision, int scale = 0)
		{
			Action constructorCall = () => new NumberValidator(precision, scale);
			constructorCall.Should().Throw<ArgumentException>();
		}

		[TestCase(1, TestName = "Positive precision")]
		[TestCase(2, 0, TestName = "Zero scale")]
		[TestCase(2, 1, TestName = "Positive scale")]
		[TestCase(3, 2, TestName = "Scale is less than precision")]
		[TestCase(3, 2, TestName = "Scale is less than precision")]
		[TestCase(3, 2, true, TestName = "Setting onlyPositive parameter as true")]
		[TestCase(3, 2, false, TestName = "Setting onlyPositive parameter as false")]
		public void Constructor_DoesNotThrowArgumentException_OnCorrectParameters(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action constructorCall = () => new NumberValidator(precision, scale);
			constructorCall.Should().NotThrow<ArgumentException>();
		}

		[TestCase("abcdef", TestName = "String of letters")]
		[TestCase("!@#$%^&*=", TestName = "String of special characters")]
		[TestCase("1.", TestName = "Positive number with a full-stop divider but no fractional part")]
		[TestCase("+1.", TestName = "Positive number with a plus sign and full-stop divider but no fractional part")]
		[TestCase("-1.", TestName = "Negative number with a full-stop divider but no fractional part")]
		[TestCase("1,", TestName = "Positive number with a comma divider but no fractional part")]
		[TestCase("+1,", TestName = "Positive number with a plus sign and comma divider but no fractional part")]
		[TestCase("-1,", TestName = "Negative number with a comma divider but no fractional part")]
		[TestCase("+", TestName = "Plus sign with no integer part")]
		[TestCase("-", TestName = "Minus sign with no integer part")]
		[TestCase(".0", TestName = "Fractional part with no integer part")]
		[TestCase("-.0", TestName = "Negative fractional part with no integer part")]
		public void IsValidNumber_False_WhenValueDoesNotMatchFormat(string value)
		{
			var validator = new NumberValidator(1);
			validator.IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCase("12.345", TestName = "Positive number")]
		[TestCase("-12.345", TestName = "Negative number")]
		[TestCase("-1234", TestName = "Minus sign should count as a digit")]
		[TestCase("+1234", TestName = "Plus sign should count as a digit")]
		public void IsValidNumber_False_WhenValuePrecisionIsTooBig(string value)
		{
			var validator = new NumberValidator(4, 3);
			validator.IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCase("1.234", TestName = "Positive number")]
		[TestCase("-1.234", TestName = "Negative number")]
		public void IsValidNumber_False_WhenValueScaleIsTooBig(string value)
		{
			var validator = new NumberValidator(4, 2);
			validator.IsValidNumber(value).Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_False_OnNegativeNumbers_WhenDeclaredAsOnlyPositive()
		{
			var validator = new NumberValidator(4, 2, true);
			validator.IsValidNumber("-1").Should().BeFalse();
		}

		[TestCase(3, 2, true, "1.2", TestName = "Positive number with no sign")]
		[TestCase(3, 2, true, "+1.2", TestName = "Positive number with a sign")]
		[TestCase(3, 2, false, "1.2", TestName = "Positive number when onlyPositive is false")]
		[TestCase(3, 2, false, "-1.2", TestName = "Negative number when onlyPositive is false")]
		public void IsValidNumber_True_OnCorrectParameters(int precision, int scale, bool onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(value).Should().BeTrue();
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