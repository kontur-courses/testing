using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.NumberValidator
{
	public class NumberValidatorTests
	{
		[TestCase(0, 2, TestName = "precision is zero")]
		[TestCase(-1, 2, TestName = "precision is negative")]
		[TestCase(1, -1, TestName = "scale is negative")]
		[TestCase(1, 2, TestName = "scale is greater than precision")]
		[TestCase(1, 1, TestName = "scale is equals precision")]
		public void Constructor_Fails_OnIncorrectArguments(int precision, int scale)
		{
			Action a = () => { new NumberValidator(precision, scale); };
			a.Should().Throw<ArgumentException>();
		}
		
		[Test]
		public void Constructor_Success_WithOneArgument()
		{
			Action a = () => { new NumberValidator(1); };
			a.Should().NotThrow();
		}
		
		[TestCase(3, 2, true, null, TestName = "value is null")]
		[TestCase(3, 2, true, "", TestName = "value is empty")]
		[TestCase(3, 2, true, " ", TestName = "value is space")]
		[TestCase(3, 2, true, "+1..23", TestName = "value contains two separators")]
		[TestCase(3, 2, true, "++0", TestName = "value contains two signs")]
		[TestCase(3, 2, true, "1.2a", TestName = "value contains letters")]
		[TestCase(3, 2, true, "+", TestName = "value only contains sign")]
		[TestCase(3, 2, true, "0?0", TestName = "value separated by other symbol than dot or comma")]
		[TestCase(3, 2, true, " 0", TestName = "value contains spaces before number")]
		[TestCase(3, 2, true, "0 ", TestName = "value contains spaces after number")]
		[TestCase(3, 2, true, "0.", TestName = "value hasn't contains numbers after separator")]
		[TestCase(3, 2, true, ".0", TestName = "value hasn't contains numbers before separator")]
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