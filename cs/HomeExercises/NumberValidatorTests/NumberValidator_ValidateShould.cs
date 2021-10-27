using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidator_ValidateShould
	{
		[TestCase("123456.78", TestName = "True_WhenCorrespondsDimension")]
		[TestCase("1234567.8", TestName = "True_WhenIntegerCorrespondsDimension_AndFractionalLess")]
		[TestCase("12345678", TestName = "True_WhenIntegerCorrespondsDimension_WithotFractional")]
		[TestCase("12.34", TestName = "True_WhenIntegerLessThanDimension_AndFractionalCorrespods")]
		[TestCase("1.2", TestName = "True_WhenIntegerLessThanDimension_AndFractionalLess")]
		public void True_OnValidNumber(string value)
		{
			var validator = new NumberValidator(8, 2);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase("12.3", TestName = "Correct_WithDecimalPoint")]
		[TestCase("12,3", TestName = "Correct_WithDecimalComma")]
		public void ShouldBeCorrect_WithInvariantCulture(string value)
		{
			var validator = new NumberValidator(8, 2);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase("1.2345678", TestName = "False_WhenFractionalGreater")]
		[TestCase("123456789", TestName = "False_WhenIntegerGreater")]
		[TestCase("12345678.9", TestName = "False_WhenGreater")]
		public void False_OnNotValidNumber(string value)
		{
			var validator = new NumberValidator(8, 2);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		public void False_WhenOnlyPositive_OnNegative()
		{
			var validator = new NumberValidator(8, 2, onlyPositive: true);
			validator.IsValidNumber("-12.3").Should().BeFalse();
		}

		[TestCase("12.3", TestName = "True_WhenOnlyPositive_OnPositive")]
		[TestCase("+12.3", TestName = "True_WhenOnlyPositive_OnPositive_Explicit")]
		public void True_WhenOnlyPositive_OnPositive(string value)
		{
			var validator = new NumberValidator(8, 2, onlyPositive: true);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase("-123456.78", TestName = "False_OnDimensionExceeded_WithMinus")]
		[TestCase("+123456.78", TestName = "False_OnDimensionExceeded_WithPlus")]
		public void ShouldCountScaleWithSign_FalseExpected_OnDimensionExceeded(string value)
		{
			var validator = new NumberValidator(8, 2, onlyPositive: false);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("abcdef.gh", TestName = "False_OnNotNumberString_Letters")]
		[TestCase("123!4", TestName = "False_OnNotNumberString_UncorrectSeparator")]
		public void False_OnNotNumberString(string value)
		{
			var validator = new NumberValidator(8, 2, onlyPositive: false);
			validator.IsValidNumber(value).Should().BeFalse();
		}
	}
}