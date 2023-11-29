using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.NumberValidator
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(0, 2, true, TestName = "precision is zero")]
		[TestCase(-1, 2, true, TestName = "precision is negative")]
		[TestCase(1, -1, false, TestName = "scale is negative")]
		[TestCase(1, 2, false, TestName = "scale is greater than precision")]
		[TestCase(1, 1, false, TestName = "scale is equals precision")]
		public void Constructor_Fails_OnIncorrectArguments(int precision, int scale, bool onlyPositive)
		{
			Action a = () => { new NumberValidator(precision, scale, onlyPositive); };
			a.Should().Throw<ArgumentException>();
		}
		
		[TestCase(1, 0, true, TestName = "with all arguments")]
		public void Constructor_Success_OnCorrectArguments(int precision, int scale, bool onlyPositive)
		{
			Action a = () => { new NumberValidator(precision, scale, onlyPositive); };
			a.Should().NotThrow();
		}

		[TestCase(3, 2, true, null, TestName = "value is null")]
		[TestCase(3, 2, true, "", TestName = "value is empty")]
		[TestCase(3, 2, true, "+1..23", TestName = "value has incorrect format")]
		[TestCase(17, 2, true, "0.000", TestName = "value's fraction part length is greater than scale")]
		[TestCase(5, 2, true, "-0.00", TestName = "negative sign when onlyPositive is true")]
		[TestCase(3, 2, true, "+0.00", TestName = "intPart and fractPart together is greater than precision")]
		public void IsValidNumber_ReturnsFalse_OnIncorrectArguments(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(value)
				.Should()
				.BeFalse();
		}
		
		[TestCase(17, 2, true, "0", TestName = "value without sign")]
		[TestCase(17, 2, true, "+0", TestName = "value with positive sign")]
		[TestCase(17, 2, false, "-0", TestName = "value with negative sign")]
		[TestCase(17, 2, true, "0.0", TestName = "value with period as delimiter")]
		[TestCase(17, 2, true, "0,0", TestName = "value with comma as delimiter")]
		[TestCase(17, 2, true, "+0,0", TestName = "value with sign and delimiter")]
		[TestCase(40, 20, true, "1234567890", TestName = "value with different numbers")]
		public void IsValidNumber_ReturnsTrue_OnCorrectArguments(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(value)
				.Should()
				.BeTrue();
		}
	}

}