using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true, TestName = "Negative Precision")]
		[TestCase(2, -1, true, TestName = "Negative Scale")]
		[TestCase(2, 3, true, TestName = "Precision < Scale")]
		[TestCase(3, 3, true, TestName = "Precision = Scale")]
		[TestCase(0, null, true, TestName = "Precision = Zero")]
		[TestCase(null, 3, true, TestName = "Precision is null")]
		public void NumberValidator_CreateInvalidNumber_ShouldBeTrowException(int precision, int scale,
			bool onlyPositive)
		{
			Action createNumberValidator = () =>
			{
				var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			};
			createNumberValidator.Should().Throw<ArgumentException>();
		}

		[TestCase(3, 2, null, TestName = "OnlyPositive is null")]
		[TestCase(3, null, true, TestName = "Scale is null")]
		[TestCase(3, 2, true, TestName = "Scale < Precision")]
		[TestCase(3, 0, true, TestName = "Scale == 0")]
		public void NumberValidator_CreateCorrectNumber_ShouldBeNotTrowException(int precision, int scale,
			bool onlyPositive)
		{
			Action createNumberValidator = () =>
			{
				var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			};
			createNumberValidator.Should().NotThrow();
		}

		[TestCase(2, 0, "   ", TestName = "number is someWhiteSpaces")]
		[TestCase(2, 0, "string", TestName = "number is word")]
		[TestCase(2, 0, "", TestName = "Number is Empty")]
		[TestCase(2, 0, null, TestName = "Number Is Null")]
		[TestCase(3, 0, "@20", TestName = "invalid sign. It isn't '-' or '+'")]
		[TestCase(4, 0, "-+20", TestName = "more than one sign")]
		[TestCase(4, 2, "20?00", TestName = "invalid separator. It isn't '.' or ','")]
		[TestCase(4, 2, "20......00", TestName = "more than one separators in a row")]
		[TestCase(2, 0, "20.", TestName = "Number with separator, but Empty fractional part")]
		[TestCase(1, 0, "+", TestName = "number with sign but without fractional and integer parts")]
		[TestCase(4, 1, "2.0.0.0", TestName = "more than one separator")]
		[TestCase(4, 1, "D75", TestName = "number in is not the correct number system")]
		[TestCase(4, 1, "    200    ", TestName = "number surrounded by spaces")]
		[TestCase(4, 1, "\n200\n", TestName = "number surrounded by singles LineFeed")]
		[TestCase(3, 2, "+.20", TestName = "number without integer part")]
		[TestCase(8, 6, "20.string", TestName = "fractional part is word")]
		public void IsValidNumber_IfInvalidTypeValue_ShouldBeFalse(int precision, int scale, string number,
			bool onlyPositive = true)
		{
			AssertIsValidNumber(precision, scale, onlyPositive, number, false);
		}

		[TestCase(4, 2, true, "+20.00", TestName = "precision == length value, but number with '+'")]
		[TestCase(4, 2, false, "-20.00", TestName = "precision == length value, but number with '-'")]
		[TestCase(5, 2, true, "-20.00", TestName = "negative number but onlyPositive flag == true")]
		[TestCase(3, 1, true, "20.00", TestName = "precision < length number")]
		[TestCase(4, 0, true, "20.00", TestName = "scale < length fractional part")]
		public void IsValidNumber_IfIncorrectArgs_ShouldBeFalse(int precision, int scale, bool onlyPositive,
			string number)
		{
			AssertIsValidNumber(precision, scale, onlyPositive, number, false);
		}

		[TestCase(3, 0, "+20", TestName = "use '+' before number")]
		[TestCase(3, 1, "20.0", TestName = "used '.' as a sign")]
		[TestCase(3, 1, "20,0", TestName = "used ',' as a sign")]
		[TestCase(2, 0, "20", TestName = "number without fractional part")]
		[TestCase(3, 0, "-20", TestName = "negative number")]
		public void IsValidNumber_IfCorrectValue_ShouldBeTrue(int precision, int scale, string number,
			bool onlyPositive = false)
		{
			AssertIsValidNumber(precision, scale, onlyPositive, number, true);
		}

		[TestCase(4, 2, true, "20.00", TestName = "precision of the number without sign == value Length")]
		[TestCase(5, 2, null, "-20.00", TestName = "negative number with onlyPositive == null")]
		[TestCase(21, 1, true, "+20.0", TestName = "precision of the number > value length + sign")]
		[TestCase(3, 2, true, "20.0", TestName = "scale > fractional part")]
		[TestCase(3, 1, true, "20.0", TestName = "scale == fractional part")]
		[TestCase(4, 1, false, "+20.0", TestName = "use '+' without onlyPositive flag")]
		[TestCase(2, null, true, "20", TestName = "number with Empty Scale")]
		[TestCase(3, 0, false, "-20", TestName = "precision of the number == value Length + sign")]
		[TestCase(2, 1, true, "20", TestName = "number without fractional part but scale > 0")]
		public void IsValidNumber_IfNumberWithCorrectArgs_ShouldBeTrue(int precision, int scale, bool onlyPositive,
			string number)
		{
			AssertIsValidNumber(precision, scale, onlyPositive, number, true);
		}

		private static void AssertIsValidNumber(int precision, int scale, bool onlyPositive, string number,
			bool expected)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(number).Should().Be(expected);
		}
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
			throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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

		if (onlyPositive && match.Groups[1].Value == "-")
			return false;
		return true;
	}
}