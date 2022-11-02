using FluentAssertions;
using HomeExercises;
using NUnit.Framework;

namespace HomeExercisesTests;

public class NumberValidatorTests
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

	[TestCase(2, 1, TestName = "{m}ScaleLessThenPrecision")]
	[TestCase(1, 0, TestName = "{m}ZeroScale")]
	[TestCase(1, 0, true, TestName = "{m}OnlyPositive")]
	[TestCase(1, 0, false, TestName = "{m}NotOnlyPositive")]
	[TestCase(int.MaxValue, 0, TestName = "{m}MaxValuePrecision")]
	[TestCase(int.MaxValue, int.MaxValue - 1, TestName = "{m}MaxValueScale")]
	public void Constructor_NotThrowArgumentException_On(int precision,
		int scale, bool onlyPositive = false)
	{
		Action constructor = () => new NumberValidator(precision, scale, onlyPositive);
		constructor.Should()
			.NotThrow<ArgumentException>();
	}

	[TestCase(null, TestName = "{m}IsNull")]
	[TestCase("", TestName = "{m}IsEmpty")]
	[TestCase(" ", TestName = "{m}IsOnlySpace")]
	[TestCase("lette.rs", TestName = "{m}WithLetters")]
	[TestCase("+", TestName = "{m}IsOnlyPlusSign")]
	[TestCase("-", TestName = "{m}IsOnlyMinusSign")]
	[TestCase("0.", TestName = "{m}WithDotButWithoutFractionalPart")]
	[TestCase("0,", TestName = "{m}WithCommaButWithoutFractionalPart")]
	[TestCase(".0", TestName = "{m}WithDotButWithoutIntegerPart")]
	[TestCase(",0", TestName = "{m}WithCommaButWithoutIntegerPart")]
	[TestCase("0..0", TestName = "{m}WithDoubleComma")]
	[TestCase("0,,0", TestName = "{m}WithDoubleDot")]
	[TestCase("0.,0", TestName = "{m}WithDotAndComma")]
	[TestCase("++0", TestName = "{m}WithDoublePlus")]
	[TestCase("--0", TestName = "{m}WithDoubleMinus")]
	[TestCase("0, 0", TestName = "{m}WithExcessSpaceInsideNumber")]
	[TestCase("0,0 ", TestName = "{m}WithExcessSpaceOutsideNumber")]
	public void IsValidNumber_ReturnFalse_OnString(string number)
	{
		new NumberValidator(10, 2).IsValidNumber(number).Should().BeFalse();
	}

	[TestCase("000", 2, 0, TestName = "{m}TooLongNumberWithoutFractionalPart")]
	[TestCase("0.000", 3, 1, TestName = "{m}TooLongNumberWithoutSign")]
	[TestCase("+0.00", 3, 2, TestName = "{m}TooLongNumberWithPlusSign")]
	[TestCase("-0.00", 3, 2, TestName = "{m}TooLongNumberWithMinusSign")]
	[TestCase("0.00", 3, 1, TestName = "{m}TooLongFractionalPart")]
	[TestCase("0.00", 3, 0, TestName = "{m}FractionalPartAndIntegerValidator")]
	[TestCase("-0.00", 4, 2, true, TestName = "{m}NegativeNumberAndOnlyPositiveValidator")]
	public void IsValidNumber_ReturnFalse_On(string number, int precision, int scale, bool onlyPositive = false)
	{
		new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeFalse();
	}

	[TestCase("0", 1, 0, TestName = "{m}Zero")]
	[TestCase("1", 1, 0, TestName = "{m}IntegerNumber")]
	[TestCase("123", 3, 2, TestName = "{m}IntegerNumberAndNotIntegerValidator")]
	[TestCase("1", 10, 2, TestName = "{m}PrecisionGreaterThenNumberLength")]
	[TestCase("1.23", 3, 2, TestName = "{m}FractionalNumberWithDot")]
	[TestCase("1,23", 3, 2, TestName = "{m}FractionalNumberWithComma")]
	[TestCase("1.23", 11, 10, TestName = "{m}ScaleGreaterThenFractionalPartLength")]
	[TestCase("+1.23", 4, 2, TestName = "{m}PositiveNumberWithSign")]
	[TestCase("-1.23", 4, 2, TestName = "{m}NegativeNumberAndNotOnlyPositiveValidator")]
	[TestCase("+1.23", 4, 2, true, TestName = "{m}PositiveNumberAndOnlyPositiveValidator")]
	public void IsValidNumber_ReturnTrue_On(string number, int precision, int scale, bool onlyPositive = false)
	{
		new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeTrue();
	}
}