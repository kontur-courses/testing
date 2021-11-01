using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
	{

		[TestCase(0, 0, TestName = "WhenPrecisionZero")]
		[TestCase(-1, 0, TestName = "WhenPrecisionNegative")]
		[TestCase(1, -1, TestName = "WhenScaleNegative")]
		[TestCase(1, 2, TestName = "WhenScaleMoreThenPrecision")]
		public void CtorShould_ThrowArgumentException(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}

		[TestCase("1234567890", 10, 0,
			ExpectedResult = true, TestName = "ShouldBeTrue_WhenAllDigitsUsed")]
		[TestCase("1,0", ExpectedResult = true,
			TestName = "ShouldBeTrue_WhenSeparatorIsComma")]
		[TestCase("1.0", ExpectedResult = true,
			TestName = "ShouldBeTrue_WhenSeparatorIsDot")]
		[TestCase("asd.f", ExpectedResult = false,
			TestName = "ShouldBeFalse_WhenNotNumber")]
		[TestCase("1.2.3", ExpectedResult = false,
			TestName = "ShouldBeFalse_WithManyDots")]
		[TestCase("asd.f", ExpectedResult = false,
			TestName = "ShouldBeFalse_WhenNotNumber")]
		[TestCase("", ExpectedResult = false,
			TestName = "ShouldBeFalse_WhenEmptyWord")]
		[TestCase(null, ExpectedResult = false,
			TestName = "ShouldBeFalse_WhenNull")]
		[TestCase("123.", ExpectedResult = false,
			TestName = "ShouldBeFalse_WhenEndsWithSeparator")]
		[TestCase(".123", ExpectedResult = false,
			TestName = "ShouldBeFalse_WhenBeginsWithSeparator")]
		[TestCase("123", ExpectedResult = true,
			TestName = "ShouldBeTrue_WhenNumberLengthLessThenPrecision")]
		[TestCase("1234", ExpectedResult = true,
			TestName = "ShouldBeTrue_WhenNumberLengthEqualsPrecision")]
		[TestCase("12345", ExpectedResult = false,
			TestName = "ShouldBeFalse_WhenNumberLengthMoreThenPrecision")]
		[TestCase("123.4", ExpectedResult = true,
			TestName = "ShouldBeTrue_WhenFractionLengthLessThenScale")]
		[TestCase("12.34", ExpectedResult = true,
			TestName = "ShouldBeTrue_WhenFractionLengthEqualsScale")]
		[TestCase("1.234", ExpectedResult = false,
			TestName = "ShouldBeFalse_WhenFractionLengthMoreThenScale")]
		[TestCase("-1234", ExpectedResult = false,
			TestName = "PrecisionShouldConsiderMinus")]
		[TestCase("+1234", ExpectedResult = false,
			TestName = "PrecisionShouldConsiderPlus")]
		[TestCase("-1", 2, 0, true,
			ExpectedResult = false,
			TestName = "ShouldBeFalse_WhenOnlyPositiveTrueAndNumberNegative")]
		public bool IsValidNumber(string number, int precision = 4,
			int scale = 2, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			return validator.IsValidNumber(number);
		}
	}
}