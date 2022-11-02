using FluentAssertions;
using HomeExercises;
using NUnit.Framework;

namespace NumberValidatorTests;

public class NumberValidator_Should
{
	[TestCase(-1, 0, TestName = "{m}IsNegative")]
	[TestCase(0, 0, TestName = "{m}IsZero")]
	public void Constructor_ThrowArgumentException_OnPrecision(int precision, int scale)
	{
		Action constructor = () => new NumberValidator(precision, scale);
		constructor.Should()
			.Throw<ArgumentException>()
			.WithMessage("precision must be a positive number");
	}

	[TestCase(1, -1, TestName = "{m}IsNegative")]
	[TestCase(1, 1, TestName = "{m}EqualsPrecision")]
	[TestCase(1, 2, TestName = "{m}GreaterThenPrecision")]
	public void Constructor_ThrowArgumentException_OnScale(int precision, int scale)
	{
		Action constructor = () => new NumberValidator(precision, scale);
		constructor.Should()
			.Throw<ArgumentException>()
			.WithMessage("scale must be a non-negative number less or equal than precision");
	}
	
	[TestCase(2, 1, TestName = "Scale less then precision")]
	[TestCase(1, 0, TestName = "Zero scale")]
	[TestCase(1, 0, true, TestName = "Only positive")]
	[TestCase(1, 0, false, TestName = "Not only positive")]
	[TestCase(int.MaxValue, 0, TestName = "Max value precision")]
	[TestCase(int.MaxValue, int.MaxValue - 1, TestName = "Max value scale")]
	public void Constructor_NotThrowArgumentException_OnValidParameters(int precision,
		int scale, bool onlyPositive = false)
	{
		Action constructor = () => new NumberValidator(precision, scale, onlyPositive);
		constructor.Should()
			.NotThrow<ArgumentException>();
	}
	
			[TestCase(null, TestName = "Null string")]
		[TestCase("", TestName = "Empty string")]
		[TestCase(" ", TestName = "Space string")]
		[TestCase("lette.rs", TestName = "String with letters")]
		[TestCase("+", TestName = "Only plus sign")]
		[TestCase("-", TestName = "Only minus sign")]
		[TestCase("0.", TestName = "With dot but without fractional part")]
		[TestCase("0,", TestName = "With comma but without fractional part")]
		[TestCase(".0", TestName = "With dot but without integer part")]
		[TestCase(",0", TestName = "With comma but without integer part")]
		public void IsValidNumber_ReturnFalse_OnNotNumber(string number)
		{
			new NumberValidator(10, 2).IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("000", 2, 0, TestName = "Too long number without fractional part")]
		[TestCase("0.000", 3, 1, TestName = "Too long number without sign")]
		[TestCase("+0.00", 3, 2, TestName = "Too long number with plus sign")]
		[TestCase("-0.00", 3, 2, TestName = "Too long number with minus sign")]
		[TestCase("0.00", 3, 1, TestName = "Too long fractional part")]
		[TestCase("0.00", 3, 0, TestName = "Fractional part and integer validator")]
		[TestCase("-0.00", 4, 2, true, TestName = "Negative Number and only positive validator")]
		public void IsValidNumber_ReturnFalse__OnIncorrectParameters(
			string number, int precision, int scale, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("0", 1, 0, TestName = "Zero")]
		[TestCase("1", 1, 0, TestName = "Integer number")]
		[TestCase("123", 3, 2, TestName = "Integer number and not integer validator")]
		[TestCase("1", 10, 2, TestName = "Precision greater then number length")]
		[TestCase("1.23", 3, 2, TestName = "Fractional number with dot")]
		[TestCase("1,23", 3, 2, TestName = "Fractional number with comma")]
		[TestCase("1.23", 11, 10, TestName = "Fractional number and big scale")]
		[TestCase("+1.23", 4, 2, TestName = "Positive number with sign")]
		[TestCase("-1.23", 4, 2, TestName = "Negative number with sign and not only positive validator")]
		[TestCase("+1.23", 4, 2, true, TestName = "Positive number with sign and only positive validator")]
		public void IsValidNumber_ReturnTrue_OnCorrectParameters(
			string number, int precision, int scale, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeTrue();
		}
}