using FluentAssertions;
using NUnit.Framework;
using System;

namespace HomeExercises.tests
{
	public class NumberValidator_Should
	{
		[TestCase(-1, 2, false, TestName = "NegativePrecision")]
		[TestCase(0, 0, false, TestName = "PrecisionIsZero")]
		[TestCase(3, -1, false, TestName = "NegativeScale")]
		[TestCase(1, 2, false, TestName = "ScaleIsGreaterThanPrecision")]
		[TestCase(1, 1, true, TestName = "PrecisionEqualsScale")]
		public void Creation_ShouldThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			Action creation = () => new NumberValidator(precision, scale, onlyPositive);
			creation.Should().Throw<ArgumentException>();
		}
		[Test]
		public void Creation_ShouldNotThrow()
		{
			Action creation = () => new NumberValidator(1, 0);
			creation
				.Should() 
				.NotThrow<Exception>();
			
		}
		
		[TestCase(3, 2, false, "-+0.0", TestName = "TwoSigns")]
		[TestCase(3, 2, false, " -0.0", TestName = "SpaceFirstDigitInNumber")]
		[TestCase(3, 2, false, "0,", TestName ="Aren'tDigitsAfterCommas")]
		[TestCase(3, 2, false, "0.0.0", TestName = "ThreeDigitsInNumberByTwoDots")]
		[TestCase(3, 2, false, "0,0,0", TestName = "ThreeDigitsInNumberByTwoCommas")]
		[TestCase(3, 2, false, "00.00", TestName = "NumberPrecisionGreaterThanValidatorPrecision")]
		[TestCase(3, 2, false, null, TestName = "NumberNull")]
		[TestCase(3, 2, false, "", TestName = "NumberEmpty")]
		[TestCase(3, 2, false, "+1.23", TestName = "NumberPrecisionGreaterThanValidatorPrecision")]
		[TestCase(3, 2, false, "-1.23", TestName = "NumberNegativeInNumberOnlyPositive")]
		[TestCase(17, 2, true, "0.000", TestName = "NumberScaleGreaterThanValidatorScale")]
		[TestCase(17, 2, true, "a.sd", TestName = "IncorrectNumberFormatBecauseLettersArePresent")]
		public void IsValidNumber_ShouldFalse_When(int precision, int scale, bool onlyPositive, string number)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase(17, 2, false, "0.0", TestName = "NumberWithFractionalPart")]
		[TestCase(17, 2, false, "0", TestName = "OnlyInteger")]
		[TestCase(4, 2, true, "+1.23", TestName = "NumberWithSign")]
		public void IsValidNumber_ShouldTrue_When(int precision, int scale, bool onlyPositive, string number)
		{
			var validator = new NumberValidator(precision, scale);
			validator.IsValidNumber(number).Should().BeTrue();
		}
	}
}
