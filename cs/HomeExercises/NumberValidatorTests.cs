using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private NumberValidator GetNumberValidator(int precision, int scale, bool onlyPositive)
        {
			return new NumberValidator(precision, scale, onlyPositive);
		}

		[TestCase(-1, 2, true, TestName = "ThrowException_WhenNotPositivePrecision")]
		[TestCase(0, 2, true, TestName = "ThrowException_WhenZeroPrecision")]
		[TestCase(0, -1, true, TestName = "ThrowException_WhenNegativeScale")]
		[TestCase(5, 5, true, TestName = "ThrowException_WhenScaleEqualPrecision")]
		[TestCase(4, 5, true, TestName = "ThrowException_WhenScaleBiggerThenPrecision")]
		public void ThrowException(int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => GetNumberValidator(precision, scale, onlyPositive));
		}

		[TestCase(4, 2, true, TestName = "DoesNotThrowException_WhenPrecisionBiggerThenScale")]
		[TestCase(2, 0, true, TestName = "DoesNotThrowException_WhenZeroScale")]
		public void DoesNotThrowException(int precision, int scale, bool onlyPositive)
		{
			Assert.DoesNotThrow(() => GetNumberValidator(precision, scale, onlyPositive));
		}

		[TestCase(4, 2, false, "0,1", TestName = "IsValidNumber_WhenFractNumberWithСomma")]
		[TestCase(4, 2, false, "0.1", TestName = "IsValidNumber_WhenFractNumberWithDot")]
		[TestCase(4, 2, true, "+0.0", TestName = "IsValidNumber_WhenPositiveFractNumber")]
		[TestCase(4, 2, true, "+0", TestName = "IsValidNumber_WhenPositiveIntNumber")]
		[TestCase(4, 2, false, "-0.0", TestName = "IsValidNumber_WhenNegativeFractNumber")]
		[TestCase(4, 2, false, "-0", TestName = "IsValidNumber_WhenNegativeIntNumber")]
		[TestCase(17, 2, false, "99999999999999", TestName = "IsValidNumber_WhenLargeIntNumber")]
		[TestCase(30, 17, false, "99999999999999,9999999999999", TestName = "IsValidNumber_WhenLargeFractNumber")]
		public void IsValidNumber(int precision, int scale, bool onlyPositive, string input)
		{
			GetNumberValidator(precision, scale, onlyPositive).IsValidNumber(input).Should().BeTrue();
		}

		[TestCase(3, 2, true, " ", TestName = "IsNotValidNumber_WhenEmptyInput")]
		[TestCase(3, 2, true, null, TestName = "IsNotValidNumber_WhenNullInput")]
		[TestCase(3, 2, true, "   0.1", TestName = "IsNotValidNumber_WhenEmptySpaceBeforeNumber")]
		[TestCase(3, 2, true, "0.1   ", TestName = "IsNotValidNumber_WhenEmptySpaceAfterNumber")]
		[TestCase(3, 2, true, "+ 0.1", TestName = "IsNotValidNumber_WhenEmptySpaceBetweenSignAndNumber")]
		[TestCase(3, 2, true, "0.1af", TestName = "IsNotValidNumber_WhenInputContainsCharacters")]
		[TestCase(3, 2, true, "ABCdeeeeeeffFFFff", TestName = "IsNotValidNumber_WhenTextInput")]
		[TestCase(3, 1, true, "+0.00", TestName = "IsNotValidNumber_WhenInputBiggerThenPrecision")]
		[TestCase(6, 2, true, "00.000", TestName = "IsNotValidNumber_WhenFracPartBiggerThenScale")]
		[TestCase(6, 2, true, "-0.00", TestName = "IsNotValidNumber_WhenOnlyPositiveButInputIsNegativeNumber")]
		[TestCase(6, 2, true, "++1.23", TestName = "IsNotValidNumber_WhenMoreThanOneSign")]
		[TestCase(6, 2, true, "1,23,23", TestName = "IsNotValidNumber_WhenMoreThanOneComma")]
		[TestCase(6, 2, true, "1.23.23", TestName = "IsNotValidNumber_WhenMoreThanOneDot")]
		[TestCase(6, 2, true, "1.", TestName = "IsNotValidNumber_WhenIntNumberWithDot")]
		[TestCase(6, 2, true, "1,", TestName = "IsNotValidNumber_WhenIntNumberWithComma")]
		public void IsNotValidNumber(int precision, int scale, bool onlyPositive, string input)
		{
			GetNumberValidator(precision, scale, onlyPositive).IsValidNumber(input).Should().BeFalse();
		}
	}
}