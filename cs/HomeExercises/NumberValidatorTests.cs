﻿using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(0, 1, Description = "Zero precision")]
		[TestCase(-1, 1, Description = "Negative precision")]
		[TestCase(2, -1, Description = "Negative scale")]
		[TestCase(2, 2, Description = "Scale is equal to precision")]
		[TestCase(2, 3, Description = "Scale is less than precision")]
		public void Constructor_ThrowsArgumentException_OnNotCorrectArguments(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}

		[TestCase("")]
		[TestCase("a.sd")]
		[TestCase("a,sd")]
		[TestCase(".")]
		[TestCase(",")]
		[TestCase("0.")]
		[TestCase("0,")]
		[TestCase(".0")]
		[TestCase(",0")]
		[TestCase("+")]
		[TestCase("-")]
		[TestCase("-0")]
		[TestCase("+0")]
		[TestCase("02")]
		public void IsValidNumber_ReturnsFalse_OnNotNumber(string notNumber)
		{
			new NumberValidator(17, 2, false).IsValidNumber(notNumber).Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_OnNumberWithTooLargeScale()
		{
			new NumberValidator(17, 2, false).IsValidNumber("0,123").Should().BeFalse();
		}

		[TestCase("12,34")]
		[TestCase("123,4")]
		[TestCase("+12,3")]
		[TestCase("-12,3")]
		public void IsValidNumber_ReturnsFalse_OnNumberWithTooLargePrecision(string number)
		{
			new NumberValidator(3, 2, false).IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("1,12")]
		[TestCase("+1,2")]
		[TestCase("112")]
		[TestCase("1")]
		[TestCase("+10")]
		[TestCase("-1,2")]
		[TestCase("-12")]
		[TestCase("-1")]
		[TestCase("0")]
		[TestCase("0,0")]
		[TestCase("0,00")]
		public void IsValidNumber_ReturnsTrue_OnCorrectNumber(string number)
		{
			new NumberValidator(3, 2, false).IsValidNumber(number).Should().BeTrue();
		}

		[TestCase("-1")]
		[TestCase("-10")]
		public void IsValidNumberWithOnlyPositiveFlag_ReturnsFalse_OnNegativeNumber(string number)
		{
			new NumberValidator(3, 2, true).IsValidNumber(number).Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_OnCorrectNumberWithComma()
		{
			new NumberValidator(3, 2, true).IsValidNumber("0,12").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_OnCorrectNumberWithPeriod()
		{
			new NumberValidator(3, 2, true).IsValidNumber("0.12").Should().BeTrue();
		}
	}
}