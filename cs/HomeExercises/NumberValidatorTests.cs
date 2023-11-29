using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, TestName = "precision is non-positive")]
		[TestCase(null, 2, TestName = "precision is null")]
		[TestCase(1, -1, TestName = "scale is negative")]
		[TestCase(1, 2, TestName = "scale > precision")]
		[TestCase(1, 1, TestName = "scale = precision")]
		public void NumberValidator_CreateWithIncorrectParams_ThrowsException(int precision, int scale)
		{
			Action action = () => { new NumberValidator(precision, scale); };
			action.Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, TestName = "scale = 0, precision > scale")]
		[TestCase(2, 1, TestName = "scale > 0, precision > scale")]
		[TestCase(2, 1, true, TestName = "scale > 0, precision > scale, onlyPositive = true")]
		public void NumberValidator_CreateWithCorrectParams_Successes(int precision, int scale,
			bool onlyPositive = false)
		{
			Action action = () => { new NumberValidator(precision, scale, onlyPositive); };
			action.Should().NotThrow();
		}

		[Test]
		public void IsValidNumber_CheckSameNumberTwoTimes_ResultsAreEqual()
		{
			var validator = new NumberValidator(17, 2, true);
			var numberToCheck = "0.0";

			var firstTimeCheck = validator.IsValidNumber(numberToCheck);
			var secondTimeCheck = validator.IsValidNumber(numberToCheck);

			firstTimeCheck.Should().Be(secondTimeCheck);
		}

		[TestCase(null, TestName = "value is null")]
		[TestCase("", TestName = "value is empty")]
		[TestCase("   ", TestName = "multiple whitespaces")]
		[TestCase("a.sd", TestName = "value has letters")]
		[TestCase("1.5.9", TestName = "more than one separator")]
		[TestCase("3^2", TestName = "value has special symbols")]
		public void IsValidNumber_CheckNonNumberValue_ReturnsFalse(string value)
		{
			var result = new NumberValidator(17, 2, true).IsValidNumber(value);

			result.Should().BeFalse();
		}

		[TestCase("1234", 3, TestName = "integer and length > precision")]
		[TestCase("0123", 3, TestName = "has leading zeros and total length > precision")]
		[TestCase("-123", 3, TestName = "negative integer and length with sign > precision")]
		[TestCase("-00123", 5, TestName = "negative integer with leading zero and length with sign > precision")]
		[TestCase("0.1234", 3, TestName = "only decimal part and length > precision")]
		[TestCase("1.234", 3, TestName = "total length > precision")]
		[TestCase("1234.1", 4, TestName = "number is decimal and integer part length = precision")]
		[TestCase("+1.23", 3, TestName = "has plus and length with sign > precision")]
		[TestCase("-1.23", 3, TestName = "has minus and length with sign > precision")]
		[TestCase("1.23", 3, 1, TestName = "decimal part length > scale")]
		[TestCase("1.200", 4, 1, TestName = "has trailing zeros and decimal length > scale")]
		[TestCase("-1.200", 4, 3, TestName = "negative, has trailing zeros and length without sign = precision")]
		[TestCase(".123", 4, 3, TestName = "no digits before separator")]
		[TestCase("2.", 4, 3, TestName = "no digits after separator")]
		[TestCase("-1.3", 3, 2, true, TestName = "number is negative and onlyPositive = true")]
		public void IsValidNumber_CheckNotValidNumber_ReturnsFalse(string value, int precision, int scale = 2,
			bool onlyPositive = false)
		{
			var result = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);

			result.Should().BeFalse();
		}

		[TestCase("123", 3, TestName = "integer and length = precision")]
		[TestCase("123", 4, TestName = "integer and length < precision")]
		[TestCase("-12", 3, TestName = "negative integer and length with sign = precision")]
		[TestCase("+1.23", 4, TestName = "has plus and length with sign = precision")]
		[TestCase("-1.23", 4, TestName = "has minus and length with sign = precision")]
		[TestCase("+1.03", 5, TestName = "has plus and length with sign < precision")]
		[TestCase("-1.03", 5, TestName = "has minus and length with sign < precision")]
		[TestCase("0.12", 3, TestName = "only decimal part and length = precision")]
		[TestCase("0123", 4, 3, TestName = "has leading zeros and length = precision")]
		[TestCase("1.200", 4, 3, TestName = "has trailing zeros and length = precision")]
		[TestCase("0.1", 3, 1, TestName = "only decimal part and length < precision")]
		[TestCase("1.23", 3, 2, TestName = "decimal part length = scale")]
		[TestCase("1.23", 4, 3, TestName = "decimal part length < scale")]
		[TestCase("-1.13", 4, 2, false, TestName = "negative and onlyPositive = false")]
		[TestCase("+1.13", 4, 2, true, TestName = "has plus and onlyPositive = true")]
		[TestCase("+1.13", 4, 2, false, TestName = "has plus and onlyPositive = false")]
		public void IsValidNumber_CheckValidNumber_ReturnsTrue(string value, int precision, int scale = 2,
			bool onlyPositive = false)
		{
			var result = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);

			result.Should().BeTrue();
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

			return !(onlyPositive && match.Groups[1].Value == "-");
		}
	}
}