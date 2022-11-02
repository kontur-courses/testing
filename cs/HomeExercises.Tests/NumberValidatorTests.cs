using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.Tests;

public class NumberValidatorTests
{
	[TestCase(0, TestName = "Precision is zero")]
	[TestCase(-1, TestName = "Precision less than zero")]
	public void Constructor_ThrowsArgumentException_OnNonPositivePrecision(int precision)
	{
		Action act = () => new NumberValidator(precision, 0);

		act.Should().Throw<ArgumentException>().WithMessage("precision must be a positive number");
	}

	[TestCase(2, TestName = "Scale is greater than precision")]
	[TestCase(-1, TestName = "Scale less than zero")]
	[TestCase(1, TestName = "Scale is equal a precision")]
	public void Constructor_ThrowsArgumentException_OnWrongScale(int scale)
	{
		Action act = () => new NumberValidator(1, scale, true);

		act.Should().Throw<ArgumentException>()
			.WithMessage("scale must be a non-negative number less than precision");
	}

	[TestCase(1, 0, TestName = "Zero scale")]
	[TestCase(3, 2, TestName = "Scale less than precision")]
	public void Constructor_DoesNotThrowArgumentException_OnCorrectParameters(int precision, int scale,
		bool onlyPositive = false)
	{
		Action act = () => new NumberValidator(precision, scale, onlyPositive);

		act.Should().NotThrow<ArgumentException>();
	}

	[TestCase("", TestName = "Empty string")]
	[TestCase(null, TestName = "Null string")]
	[TestCase(" ", TestName = "Whitespace string")]
	public void IsValidNumber_ReturnFalse_OnNullOrEmptyValue(string value)
	{
		var sut = new NumberValidator(2);
		sut.IsValidNumber(value).Should().BeFalse();
	}

	[TestCase("abc", TestName = "Letter string")]
	[TestCase("!@#", TestName = "Symbol string")]
	[TestCase("1.", TestName = "Without fractional part")]
	[TestCase("-1.", TestName = "With minus sign and without fractional part")]
	[TestCase("1.0!", TestName = "Ends with symbol")]
	[TestCase("+1,", TestName = "With plus sign and without fractional part")]
	[TestCase("++1", TestName = "With duplicate plus sign")]
	[TestCase("1..0", TestName = "With duplicate dot")]
	public void IsValidNumber_ReturnFalse_OnWrongValueFormat(string value)
	{
		var sut = new NumberValidator(2, 1);
		sut.IsValidNumber(value).Should().BeFalse();
	}


	[TestCase("12.34", TestName = "Integer part equal fractional")]
	[TestCase("+12.3", TestName = "Integer part contain plus sign")]
	[TestCase("-1234", TestName = "Integer part contain minus sign, without fractional part")]
	public void IsValidNumber_ReturnFalse_WhenValuePrecisionMoreThanNumberValidatorPrecision(string value)
	{
		var sut = new NumberValidator(3, 2);
		sut.IsValidNumber(value).Should().BeFalse();
	}

	[TestCase("12.34", TestName = "Integer part equal fractional")]
	[TestCase("+1.23", TestName = "Too long fractional part")]
	public void IsValidNumber_ReturnFalse_WhenValueScaleMoreThanNumberValidatorScale(string value)
	{
		var sut = new NumberValidator(3, 1);
		sut.IsValidNumber(value).Should().BeFalse();
	}

	[TestCase("1.23", true, TestName = "Only positive value")]
	[TestCase("+1.2", true, TestName = "Only positive value with plus sign")]
	[TestCase("1.23", false, TestName = "Without only positive")]
	[TestCase("-1.2", false, TestName = "Without only positive and with minus sign")]
	public void IsValidNumber_ReturnTrue_OnCorrectValue(string value, bool onlyPositive)
	{
		var sut = new NumberValidator(3, 2, onlyPositive);
		sut.IsValidNumber(value).Should().BeTrue();
	}

	[Test]
	public void IsValidNumber_ReturnFalse_ForNegativeNumber_WhenOnlyPositive()
	{
		var sut = new NumberValidator(2, 1, true);
		sut.IsValidNumber("-1.0").Should().BeFalse();
	}
}