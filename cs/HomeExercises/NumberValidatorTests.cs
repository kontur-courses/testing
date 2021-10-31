using System;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(3, 2, true, "a.sd", TestName = "Should be false when input is not a number")]
		[TestCase(4, 1, false, "1.", TestName = "Should be false when input is incorrect")]
		[TestCase(4, 1, false, ".1", TestName = "Should be false when input is incorrect")]
		[TestCase(4, 1, false, ".", TestName = "Should be false when input is incorrect")]
		[TestCase(4, 1, false, "", TestName = "Should be false when input is empty")]
		[TestCase(4, 1, false, null, TestName = "Should be false when input is null")]
		public void IsValidNumbed_ShouldBeFalse_WithIncorrectValue(int precision, int scale, bool onlyPositive, string value)
		{
			Assert.False(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
		}

		[TestCase(17, 2, true, "0.0", TestName = "Should be true when input is correct")]
		[TestCase(17, 2, true, "0", TestName = "Should be true when input is correct")]
		[TestCase(4, 2, true, "+1.23", TestName = "Should be true when input is correct")]
		[TestCase(4, 2, false, "-1.23", TestName = "Should be true when input is correct")]
		public void IsValidNumber_ShouldBeTrue_WhenInputCorrect(int precision, int scale, bool onlyPositive, string value)
		{
			Assert.True(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
		}

		[TestCase(4, 2, true, "-1.23", TestName = "Should be false when onlyPositive is true, but input is negative")]
		public void IsValidNumber_ShouldBeFalse_WhenOnlyPositiveDoesNotMatchToInput(int precision, int scale,
			bool onlyPositive, string value)
		{
			Assert.False(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
		}

		[TestCase(3, 2, true, "00.00", TestName = "Should be false when precision less than necessary")]
		[TestCase(3, 2, true, "-0.00", TestName = "Should be false when precision less than necessary")]
		[TestCase(3, 2, true, "+1.23", TestName = "Should be false when precision less than necessary")]
		public void IsValidNumber_ShouldBeFalse_WhenPrecisionLessThanInput(int precision, int scale, bool onlyPositive,
			string value)
		{
			Assert.False(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
		}

		[TestCase(4, 1, false, "-1.23", TestName = "Should be false when scale less than necessary")]
		[TestCase(4, 0, false, "1.23", TestName = "Should be false when scale less than necessary")]
		[TestCase(17, 0, true, "0.1", TestName = "Should be false when scale less than necessary")]
		public void IsValidNumber_ShouldBeFalse_WhenScaleLessThanInput(int precision, int scale, bool onlyPositive,
			string value)
		{
			Assert.False(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
		}
		
		[TestCase(0, TestName = "Should be false when precision non positive")]
		[TestCase(-1, TestName = "Should be false when precision non positive")]
		public void Constructor_ShouldThrowException_WhenPrecisionNonPositive(int precision)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision));
		}

		[Test]
		public void Constructor_ShouldThrowException_WhenPrecisionLessOrEqualThanScale()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(2, 6));
		}
		
		[Test]
		public void Constructor_ShouldDoesNotThrowException_WhenPrecisionNonNegativeAndBiggerThanScale()
		{
			Assert.DoesNotThrow(() => new NumberValidator(7, 6));
		}

		[Test]
		public void Constructor_ShouldDoesNotThrowException_WhenPrecisionPositive()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1));
		}
	}
}