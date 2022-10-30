using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{

		[TestCase(0, 0, TestName = "Zero precision")]
		[TestCase(-1, 0, TestName = "Simple negative precision")]
		[TestCase(int.MinValue, 0, TestName = "Max negative precision")]
		public void ArgumentException_WhenNegativeOrZeroPrecision(int precision, int scale)
		{
			Action validatorAction = () => new NumberValidator(precision, scale);
			validatorAction.Should().Throw<ArgumentException>("precision must be a positive number");
		}

		[TestCase(10, 10, TestName = "Precision equal scale")]
		[TestCase(int.MaxValue, int.MaxValue, TestName = "Precision equal scale maxInteger")]
		[TestCase(10, -10, TestName = "Negative scale")]
		[TestCase(10, int.MinValue, TestName = "Negative scale minInteger")]
		public void ArgumentException_WhenNegativeNumberUpperThanPrecisionScaleOrEquals(int precision, int scale)
		{
			Action validatorAction = () => new NumberValidator(precision, scale);
			validatorAction.Should()
				.Throw<ArgumentException>("precision must be a non-negative number less or equal than precision");
		}
		[TestCase(null, null, null, TestName = "Null number validator")]
		[TestCase(null, 0, true, TestName = "Null precision")]
		[TestCase(0, null, true, TestName = "Null scale")]
		[TestCase(0, 0, null, TestName = "Null onlyPositive")]
		public void IsValidNumber_WhenValidatorArgumentsIsNull(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive).IsValidNumber("1.0");
			action.Should().Throw<ArgumentException>("NumberValidator arguments must not be null or empty");
		}
		[Test]
		public void IsValidNumber_WhenValidatorIsNull()
		{
			NumberValidator numberValidator = null;
			Action action = () => numberValidator.IsValidNumber("1.0");
			action.Should().Throw<NullReferenceException>("NumberValidator must not be null or empty");
		}


        [TestCase("abc", TestName = "Simple strings")]
		[TestCase("#$%^&*", TestName = "Special symbol")]
		[TestCase("200a", TestName = "String with number")]
		public void IsValidNumber_NotNumber(string value)
		{
			var NumberValidator = new NumberValidator(1, 0, true).IsValidNumber(value);
			NumberValidator.Should().BeFalse();
		}

		[TestCase("20,0", TestName = "String with  \",\"")]
		[TestCase("20 0", TestName = "String with whitespace")]
		[TestCase("20-0", TestName = "String with  \"-\"")]
		[TestCase("20_0", TestName = "String with  \"_\"")]
		public void IsValidNumber_HaveNotDot(string value)
		{
			var numberValidator = new NumberValidator(1, 0, true).IsValidNumber(value);
			numberValidator.Should().BeFalse();
		}

		[TestCase("-1", TestName = "Simple value without dot")]
		[TestCase("-1.0", TestName = "Special value with dot")]

		public void IsValidNumber_OnlyPositiveButNegative(string value)
		{
			var numberValidator = new NumberValidator(10, 1, true).IsValidNumber(value);
			numberValidator.Should().BeFalse();
		}


		[TestCase(".10", TestName = "Number without int part")]
		[TestCase("10.", TestName = "Number without fractional part")]
		[TestCase("$1.10", TestName = "Number with special symbol")]
		[TestCase(".", TestName = "Dot without number")]
		[TestCase("1..1", TestName = "Number with double dot")]
		[TestCase("1. 1", TestName = "Number with dot and whitespace")]
		public void IsValidNumber_NotMatchWithRegex(string value)
		{
			var numberValidator = new NumberValidator(4, 3).IsValidNumber(value);
			numberValidator.Should().BeFalse();
		}

		[TestCase("", TestName = "Empty valid number")]
		[TestCase(null, TestName = "Null valid number")]
		public void IsValidNumberNullOrEmpty(string value)
		{
			var numberValidator = new NumberValidator(4, 3).IsValidNumber(value);
			numberValidator.Should().BeFalse();
		}



		[TestCase("+1.0", TestName = "Positive int")]
		[TestCase("-1.1", TestName = "Negative int")]

		public void IsValidNumber_WhenIntAndFractionalPart_NotBiggerPrecision(string value)
		{
			var numberValidator = new NumberValidator(3, 2).IsValidNumber(value);
			numberValidator.Should().BeTrue();
		}

		[TestCase("1.100", TestName = "Positive int with big scale")]
		[TestCase("-1.10", TestName = "Negative int with scale")]

		public void IsValidNumber_WhenFractionalPart_NotBiggerScale(string value)
		{
			var numberValidator = new NumberValidator(4, 3).IsValidNumber(value);
			numberValidator.Should().BeTrue();
		}



		[TestCase("100.00", TestName = "Positive int bigger precision")]
		[TestCase("-100.00", TestName = "Negative int bigger precision")]
		[TestCase("+1.1000", TestName = "Positive fractional with scale")]
		[TestCase("-1.1000", TestName = "Negative fractional with scale")]

		public void IsValidNumber_WhenIntOrFractionalPartBiggerPrecisionScale(string value)
		{
			var numberValidator = new NumberValidator(3, 2).IsValidNumber(value);
			numberValidator.Should().BeFalse();
		}

		[TestCase("10.-1", TestName = "Negative fractional part with \"-\"")]
		[TestCase("10.+1", TestName = "Positive fractional part with \"+\"")]

		public void IsValidNumber_WhenFractionalPartNotFormat(string value)
		{
			var numberValidator = new NumberValidator(5, 4).IsValidNumber(value);
			numberValidator.Should().BeFalse();
		}

		[TestCase(int.MaxValue, int.MaxValue, TestName = "Max int part and fractional part")]
		[TestCase(int.MinValue, int.MaxValue, TestName = "Min int part and fractional part")]
		public void IsValidNumber_WhenIntWithScaleLimitInteger(int intPart, int fractionalPart)
		{
			var numberValidator =
				new NumberValidator(int.MaxValue, int.MaxValue - 1).IsValidNumber(intPart + "." + fractionalPart);
			numberValidator.Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_WithMaxInt()
		{
			var numberValidator =
				new NumberValidator(int.MaxValue.ToString().Length).IsValidNumber(int.MaxValue.ToString());
			numberValidator.Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_WhenMaxIntWithScale()
		{
			var numberValidator =
				new NumberValidator(int.MaxValue, int.MaxValue - 1).IsValidNumber(int.MaxValue + "." + int.MaxValue);
			numberValidator.Should().BeTrue();
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

			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			var fractionalPart = match.Groups[4].Value.Length;

			if (intPart + fractionalPart > precision || fractionalPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}