using System;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(3, 2, true, "a.sd")]
		[TestCase(4, 1, false, "1.")]
		[TestCase(4, 1, false, ".1")]
		[TestCase(4, 1, false, ".")]
		[TestCase(4, 1, false, "")]
		public void IsValidNumbed_ShouldBeFalse_WithIncorrectValue(int precision, int scale, bool onlyPositive, string value)
		{
			Assert.False(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
		}

		[TestCase(17, 2, true, "0.0")]
		[TestCase(17, 2, true, "0")]
		[TestCase(4, 2, true, "+1.23")]
		[TestCase(4, 2, false, "-1.23")]
		public void IsValidNumber_ShouldBeTrue_WhenInputCorrect(int precision, int scale, bool onlyPositive, string value)
		{
			Assert.True(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
		}

		[TestCase(4, 2, true, "-1.23")]
		public void IsValidNumber_ShouldBeFalse_WhenOnlyPositiveDoesNotMatchToInput(int precision, int scale,
			bool onlyPositive, string value)
		{
			Assert.False(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
		}

		[TestCase(3, 2, true, "00.00")]
		[TestCase(3, 2, true, "-0.00")]
		[TestCase(3, 2, true, "+0.00")]
		[TestCase(3, 2, true, "+1.23")]
		[TestCase(3, 2, false, "-1.23")]
		public void IsValidNumber_ShouldBeFalse_WhenPrecisionLessThanInput(int precision, int scale, bool onlyPositive,
			string value)
		{
			Assert.False(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
		}

		[TestCase(4, 0, false, "-1.23")]
		[TestCase(4, 0, false, "1.23")]
		[TestCase(17, 2, true, "0.000")]
		public void IsValidNumber_ShouldBeFalse_WhenScaleLessThanInput(int precision, int scale, bool onlyPositive,
			string value)
		{
			Assert.False(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
		}
		
		[TestCase(0)]
		[TestCase(-1)]
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