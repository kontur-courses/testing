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

		[Test, TestCaseSource(nameof(casesForRegex))]
		[TestCaseSource(nameof(casesForEmptyAndNull))]
		[TestCaseSource(nameof(casesForPrecision))]
		[TestCaseSource(nameof(casesForScale))]
		[TestCaseSource(nameof(casesForSign))]
		[TestCase("-1", 2, 0, true,
			ExpectedResult = false,
			TestName = "ShouldBeFalse_WhenOnlyPositiveTrueAndNumberNegative")]
		public bool IsValidNumber(string number, int precision,
			int scale, bool onlyPositive)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			return validator.IsValidNumber(number);
		}

		private static TestCaseData[] casesForRegex =
		{
			new TestCaseData("1234567890", 10, 0, false)
				.SetName("ShouldBeTrue_WhenAllDigitsUsed")
				.Returns(true),
			new TestCaseData("1,0", 4, 2, false)
				.SetName("ShouldBeTrue_WhenAllDigitsUsed")
				.Returns(true),
			new TestCaseData("1.0", 4, 2, false)
				.SetName("ShouldBeTrue_WhenSeparatorIsDot")
				.Returns(true),
			new TestCaseData("asd.f", 4, 2, false)
				.SetName("ShouldBeFalse_WhenNotNumber")
				.Returns(false),
			new TestCaseData("1.2.3", 4, 2, false)
				.SetName("ShouldBeFalse_WithManyDots")
				.Returns(false),
			new TestCaseData("123.", 4, 2, false)
				.SetName("ShouldBeFalse_WhenEndsWithSeparator")
				.Returns(false),
			new TestCaseData(".123", 4, 2, false)
				.SetName("ShouldBeFalse_WhenBeginsWithSeparator")
				.Returns(false)
		};

        private static TestCaseData[] casesForEmptyAndNull =
		{
			new TestCaseData("", 4, 2, false)
				.SetName("ShouldBeFalse_WhenEmptyWord")
				.Returns(false),
			new TestCaseData(null, 4, 2, false)
				.SetName("ShouldBeFalse_WhenNull")
				.Returns(false)
		};

		private static TestCaseData[] casesForPrecision =
		{
			new TestCaseData("123", 4, 2, false)
				.SetName("ShouldBeTrue_WhenNumberLengthLessThenPrecision")
				.Returns(true),
			new TestCaseData("1234", 4, 2, false)
				.SetName("ShouldBeTrue_WhenNumberLengthEqualsPrecision")
				.Returns(true),
			new TestCaseData("12345", 4, 2, false)
				.SetName("ShouldBeFalse_WhenNumberLengthMoreThenPrecision")
				.Returns(false)
		};

		private static TestCaseData[] casesForScale =
		{
			new TestCaseData("123.4", 4, 2, false)
				.SetName("ShouldBeTrue_WhenNumberLengthLessThenScale")
				.Returns(true),
			new TestCaseData("12.34", 4, 2, false)
				.SetName("ShouldBeTrue_WhenNumberLengthEqualsScale")
				.Returns(true),
			new TestCaseData("1.234", 4, 2, false)
				.SetName("ShouldBeFalse_WhenNumberLengthMoreThenScale")
				.Returns(false)
		};

		private static TestCaseData[] casesForSign =
		{
			new TestCaseData("-1234", 4, 2, false)
				.SetName("PrecisionShouldConsiderMinus")
				.Returns(false),
			new TestCaseData("+1234", 4, 2, false)
				.SetName("PrecisionShouldConsiderPlus")
				.Returns(false)
		};
	}
}