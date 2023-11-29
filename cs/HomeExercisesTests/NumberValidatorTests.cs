using FluentAssertions;
using HomeExercises;

namespace HomeExercisesTests
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 0, Description = "Precision should be a positive number")]
		[TestCase(1, -1, Description = "Scale must be a non-negative number")]
		[TestCase(1, 2, Description = "Scale must be less than precision")]
		public void Constructor_ThrowsArgumentException_OnIncorrectInput(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, true));
		}

		[Test]
		public void Constructor_SuccessfullyCreatesObject_OnCorrectInput()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		}

		[TestCase(null)]
		[TestCase(" ")]
		[TestCase("                   ")]
		[TestCase("")]
		[TestCase("\n")]
		public void IsValidNumber_ReturnsFalse_WhenValueIsNullOrEmpty(string value)
		{
			var numberValidator = new NumberValidator(1, 0, true);

			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(".")]
		[TestCase("a.sd")]
		[TestCase("+-5")]
		[TestCase("1..3")]
		[TestCase("1.")]
		[TestCase(".3")]
		public void IsValidNumber_ReturnsFalse_WhenValueIsNaN(string value)
		{
			var numberValidator = new NumberValidator(2, 1, true);

			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("00.00", 3, 2)]
		[TestCase("+1.00", 3, 2)]
		[TestCase("-1.00", 3, 2)]
		[TestCase("-1", 1, 0)]
		public void IsValidNumber_ReturnsFalse_WhenIntAndFracPartsAreGreaterThanPrecision(string value, int precision,
			int scale)
		{
			var numberValidator = new NumberValidator(precision, scale);

			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("0.00", 3, 1)]
		[TestCase("-123.12", 6, 0)]
		public void IsValidNumber_ReturnsFalse_WhenFracPartIsGreaterThanScale(string value, int precision, int scale)
		{
			var numberValidator = new NumberValidator(precision, scale);

			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("-1.2", 3, 1)]
		[TestCase("-0.00", 4, 2)]
		public void IsValidNumber_ReturnsFalse_WhenValueIsNegativeButOnlyPositiveIsTrue(string value, int precision,
			int scale)
		{
			var numberValidator = new NumberValidator(precision, scale, true);

			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("0.0", 17, 2, true)]
		[TestCase("0.0", 2, 1, false)]
		[TestCase("+1.23", 4, 2, true)]
		[TestCase("-1.23", 4, 2, false)]
		[TestCase("0,1", 2, 1, true)]
		[TestCase("0", 1, 0, true)]
		public void IsValidNumber_ReturnsTrue_OnCorrectInputData(string value, int precision, int scale,
			bool onlyPositive)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			numberValidator.IsValidNumber(value).Should().BeTrue();
		}
	}
}