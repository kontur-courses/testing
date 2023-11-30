using FluentAssertions;
using NUnit.Framework;
using System;

namespace HomeExercises.tests
{
	public class NumberValidator_Should
	{
		[TestCase(-1, 2, TestName = "NegativePrecision")]
		[TestCase(0, 0, TestName = "PrecisionIsZero")]
		[TestCase(3, -1, TestName = "NegativeScale")]
		[TestCase(1, 2, TestName = "ScaleIsGreaterThanPrecision")]
		[TestCase(1, 1, TestName = "PrecisionEqualsScale")]
		public void Creation_ShouldThrowArgumentException(int precision, int scale)
		{
			Action creation = () => new NumberValidator(precision, scale);
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
		
		[TestCase(3, 2,"-+0.0", TestName = "TwoSigns")]
		[TestCase(3, 2," -0.0", TestName = "SpaceFirstDigitInNumber")]
		[TestCase(3, 2,"0,", TestName ="Aren'tDigitsAfterCommas")]
		[TestCase(3, 2,"0.0.0", TestName = "ThreeDigitsInNumberByTwoDots")]
		[TestCase(3, 2,"0,0,0", TestName = "ThreeDigitsInNumberByTwoCommas")]
		[TestCase(3, 2, "00.00", TestName = "NumberPrecisionGreaterThanValidatorPrecision")]
		[TestCase(3, 2, null, TestName = "NumberNull")]
		[TestCase(3, 2, "", TestName = "NumberEmpty")]
		[TestCase(3, 2, "+1.23", TestName = "NumberPrecisionGreaterThanValidatorPrecision")]
		[TestCase(3, 2, "-1.23", TestName = "NumberNegativeInNumberOnlyPositive")]
		[TestCase(17, 2, "0.000", TestName = "NumberScaleGreaterThanValidatorScale")]
		[TestCase(17, 2, "a.sd", TestName = "IncorrectNumberFormatBecauseLettersArePresent")]
		public void IsValidNumber_False(int precision, int scale, string number)
		{
			var validator = new NumberValidator(precision, scale);
			validator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase(17, 2, "0.0", TestName = "NumberWithFractionalPart")]
		[TestCase(17, 2, "0", TestName = "OnlyInteger")]
		[TestCase(4, 2, "+1.23", TestName = "NumberWithSign")]
		public void IsValidNumber_True(int precision, int scale, string number)
		{
			var validator = new NumberValidator(precision, scale);
			validator.IsValidNumber(number).Should().BeTrue();
		}
	}
}
