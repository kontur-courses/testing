using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[Test]
		public void Constructor_ThrowArgumentException_WhenPrecisionIsNegative()
		{
			Action action = () => new NumberValidator(-1);

			action.Should().Throw<ArgumentException>().WithMessage("precision must be a positive number");
		}

		[TestCase(1, -1, TestName = "Constructor_ThrowArgumentException_WhenScaleIsNegative")]
		[TestCase(1, 2, TestName = "Constructor_ThrowArgumentException_WhenScaleIsGreaterThanPrecision")]
		public void Constructor_ThrowArgumentException_WhenScaleIsInvalid(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);

			action.Should().Throw<ArgumentException>()
				.WithMessage("scale must be a non-negative number less or equal than precision");
		}

		[TestCase(1, 0, false)]
		[TestCase(1, 0, true)]
		public void Constructor_NotThrow_OnCorrectParams(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);

			action.Should().NotThrow();
		}

		[TestCase("0", TestName = "IsValidNumber_True_PositiveInt")]
		[TestCase("0.0", TestName = "IsValidNumber_True_PositiveFloatWithDot")]
		[TestCase("0,0", TestName = "IsValidNumber_True_PositiveFloatWithComma")]
		[TestCase("+0", TestName = "IsValidNumber_True_PositiveIntWithPlus")]
		[TestCase("+0.0", TestName = "IsValidNumber_True_PositiveFloatWithDotWithPlus")]
		[TestCase("+0,0", TestName = "IsValidNumber_True_PositiveFloatWithCommaWithPlus")]
		public void IsValidNumber_True_Positive(string value)
		{
			var validator = new NumberValidator(17, 2, true);

			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase("-1", TestName = "IsValidNumber_True_NegativeInt")]
		[TestCase("-1.2", TestName = "IsValidNumber_True_NegativeFloatWithDot")]
		[TestCase("-1,2", TestName = "IsValidNumber_True_NegativeFloatWithComma")]
		public void IsValidNumber_True_Negative(string value)
		{
			var validator = new NumberValidator(17, 2, false);

			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase(null, TestName = "IsValidNumber_False_Null")]
		[TestCase("", TestName = "IsValidNumber_False_StringEmpty")]
		[TestCase("a", TestName = "IsValidNumber_False_Letter")]
		[TestCase("a.b", TestName = "IsValidNumber_False_LettersFloat")]
		[TestCase(".1", TestName = "IsValidNumber_False_FloatMissingIntPart")]
		[TestCase("1.", TestName = "IsValidNumber_False_FloatMissingFracPart")]
		[TestCase("1,0,0", TestName = "IsValidNumber_False_FloatWithMultipleSeparators")]
		public void IsValidNumber_False_NaN(string value)
		{
			var validator = new NumberValidator(17, 2, false);

			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("-0", TestName = "IsValidNumber_False_NegativeIntWhenOnlyPositive")]
		[TestCase("-0.0", TestName = "IsValidNumber_False_NegativeFloatWhenOnlyPositive")]
		public void IsValidNumber_False_WhenOnlyPositive(string value)
		{
			var validator = new NumberValidator(17, 2, true);

			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(17, 2, "1,234", TestName = "IsValidNumber_False_FracPartLongerThanScale")]
		[TestCase(1, 1, "12", TestName = "IsValidNumber_False_IntPartLongerThanPrecision")]
		[TestCase(1, 1, "+1", TestName = "IsValidNumber_False_IntPartWithSignLongerThanPrecision")]
		[TestCase(1, 1, "0,0", TestName = "IsValidNumber_False_NumberWithFracPartLongerThanPrecision")]
		public void IsValidNumber_False_NumberNotMeetLengthRestrictions(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale);

			validator.IsValidNumber(value).Should().BeFalse();
		}
	}
}