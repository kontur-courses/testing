using FluentAssertions;
using NUnit.Framework;
using System;


namespace HomeExercises.NumberValidatorTask
{
	public class NumberValidatorTests
	{
		[TestCase("00.0", "int part can't contain more then one zero in highest digit when number is zero")]
		[TestCase("01", "int part can't start with zero when numbers isn't a zero")]
		public void ShouldBeFalse_WhenTooManyZero_InHighestDigitOfIntPart(string value, string whyNumberIsIncorrectMsg)
		{
			var validator = new NumberValidator(4, 2, true);
			validator.IsValidNumber(value).Should().BeFalse("Because {0}", whyNumberIsIncorrectMsg);
		}

		[Test]
		public void Precision_ShouldConsiderMinusSign()
		{
			var validator = new NumberValidator(4, 0, true);
			var value = "-1024";
			validator.IsValidNumber(value).Should().BeFalse("Because the minus sign is taken into account");
		}

		[Test]
		public void Precision_ShouldIgnorePlusSign()
		{
			var validator = new NumberValidator(4, 0, true);
			var value = "+1024";
			validator.IsValidNumber(value).Should().BeTrue("Because plus sign is ignored");
		}

		[TestCase("1.25.0", "has more then one delimiter")]
		[TestCase("1abc", "isn't a number")]
		[TestCase("-5", "is negative when onlyPositive flag is up")]
		public void ShouldBeFalse_WhenIncorrectValue(string value, string whyNumberIsIncorrectMsg)
		{
			var validator = new NumberValidator(10, 5, true);
			validator.IsValidNumber(value).Should()
				.BeFalse("Because {0} {1}", value, whyNumberIsIncorrectMsg);
		}

        [Category("Validate Fractals Delimiter")]
        [TestCase("1.5")]
        [TestCase("1,5")]
        public void ShouldBeTrue_WhenDelimiterIsDotOrComma(string value)
        {
            var validator = new NumberValidator(5, 1, true);
            validator.IsValidNumber(value).Should().BeTrue("Because the dot and comma are correct");
        }

        [Category("Ignore leading and trailing whitespaces")]
        [TestCase("    123")]
        [TestCase("\t123")]
        [TestCase("\n123")]
		[TestCase("\t123\t")]
        public void ShouldTrim_WhenLeadingOrTrailingWhitespaces(string value)
        {
	        var validator = new NumberValidator(10, 1);
	        validator.IsValidNumber(value).Should().BeTrue(
		        "Because the leading and trailing whitespaces were cut off");
		}

        [Category("Plus-minus zero")]
		[TestCase("-0")]
		[TestCase("-0.0")]
		[TestCase("+0")]
		[TestCase("+0.0")]
		public void ShouldBeFalse_WhenPlusOrMinusZero(string value)
		{
			var validator = new NumberValidator(3,1);
			validator.IsValidNumber(value).Should().BeFalse("Because a zero with a sign is incorrect.");
		}

		[Category("Plus-minus zero")]
		[TestCase("-0.1")]
		[TestCase("-0.000000001")]
		public void ShouldBeTrue_WhenNotPlusOrMinusZero(string value)
		{
			var validator = new NumberValidator(15, 10);
			validator.IsValidNumber(value).Should().BeTrue("Because {0} isn't a zero", value);
		}

		[Category("Constructor")]
		[TestCase(1, 1)]
		[TestCase(4, 5)]
		[TestCase(2, -1)]
		public void ShouldThrow_WhenScaleIsNotCorrect(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>("Because scale = {0} is incorrect", scale);
		}

		[Category("Constructor")]
		[TestCase(0)]
		[TestCase(-1)]
		public void ShouldThrow_WhenPrecisionIsNotCorrect(int precision)
		{
			Action action = () => new NumberValidator(precision);
			action.Should().Throw<ArgumentException>("Because precision = {0} is incorrect", precision);
		}
	}

	
}