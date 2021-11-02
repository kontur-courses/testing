using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidator_IsValidNumberShould
    {
        [TestCase("123456.78", TestName = "True_WhenCorrespondsDimension")]
        [TestCase("1234567.8", TestName = "True_WhenIntegerCorrespondsDimension_AndFractionalLess")]
        [TestCase("12345678", TestName = "True_WhenIntegerCorrespondsDimension_WithotFractional")]
        [TestCase("12.34", TestName = "True_WhenIntegerLessThanDimension_AndFractionalCorrespods")]
        [TestCase("1.2", TestName = "True_WhenIntegerLessThanDimension_AndFractionalLess")]
        [TestCase(".2", TestName = "True_WithoutIntegerPart")]
        public void True_OnValidNumber(string value)
        {
            var validator = new NumberValidator(8, 2);
            validator.IsValidNumber(value).Should().BeTrue();
        }

        [TestCase("12.3", TestName = "Correct_WithDecimalPoint")]
        [TestCase("12,3", TestName = "Correct_WithDecimalComma")]
        public void Correct_WithInvariantCulture(string value)
        {
            var validator = new NumberValidator(8, 2);
            validator.IsValidNumber(value).Should().BeTrue();
        }

        [TestCase("1.2345678", TestName = "False_WhenFractionalGreaterThanScale")]
        [TestCase("123456789", TestName = "False_WhenIntegerGreaterThanPrecision")]
        [TestCase("12345678.9", TestName = "False_WhenLengthGreaterThanPrecision")]
        [TestCase(".", TestName = "False_WhenOnlySeparator")]
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
        [TestCase(" 123.4 ", TestName = "False_OnNotOnlyNumberString_WithWhiteSpace")]
        [TestCase(null, TestName = "False_OnNotNumberString_Null")]
        [TestCase("", TestName = "False_OnNotNumberString_EmptyString")]
        [TestCase(" ", TestName = "False_OnNotNumberString_WhiteSpaceString")]
        [TestCase("++34.5", TestName = "False_OnSeveralSigns_Plus")]
        [TestCase("--34.5", TestName = "False_OnSeveralSigns_Minus")]
        [TestCase("ab34.5c", TestName = "False_OnNotOnlyNumberString")]
        [TestCase("ab\n5.6\nc", TestName = "False_OnNotOnlyNumberString_WithLineBreak")]
        public void False_OnNotNumberString(string value)
        {
            var validator = new NumberValidator(12, 4, onlyPositive: false);
            validator.IsValidNumber(value).Should().BeFalse();
        }

        [TestCase("+12.3", TestName = "True_WhenNotOnlyPositive_OnPositive")]
        [TestCase("-12.3", TestName = "True_WhenNotOnlyPositive_OnNegative")]
        [TestCase("12.3", TestName = "True_WhenNotOnlyPositive_WithoutSign")]
        public void True_WhenNotOnlyPositive(string value)
        {
            var validator = new NumberValidator(8, 2, onlyPositive: false);
            validator.IsValidNumber(value).Should().BeTrue();
        }
    }
}