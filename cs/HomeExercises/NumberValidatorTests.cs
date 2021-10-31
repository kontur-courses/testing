using System;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HomeExercises
{
    public class NumberValidatorTests
	{

		[TestCase(0)]
		[TestCase(-1)]
		public void NonPositivePrecision_ThrowsArgumentException(int precision)
        {
			Action action = () => new NumberValidator(precision);
			action.Should().Throw<ArgumentException>();
        }

		[Test]
		public void RightParameters_DoesNotThrows()
        {
			Action action = () => new NumberValidator(1);
			action.Should().NotThrow();
        }

		[TestCase(-1, TestName = "ScaleNegative")]
		[TestCase(2, TestName ="ScaleMoreThenPrecision")]
		public void WrongScale_ThrowsArgumentException(int scale)
        {
			Action action = () => new NumberValidator(1, scale);
			action.Should().Throw<ArgumentException>();
		}

		[TestCase("1234567890")]
		[TestCase("6018954273")]
		[TestCase("2347601958")]
		[TestCase("9403875261")]
		[TestCase("4891536702")]
		public void IsValid_WithAllDigits_True(string number)
		{
			var validator = new NumberValidator(10);
			validator.IsValidNumber(number).Should().BeTrue();
		}

		[TestCase("0.0", TestName = "WithDot")]
		[TestCase("0,0", TestName = "WithComma")]
		public void IsValid_WithDifferentSeparators_True(string number)
		{
			var validator = new NumberValidator(2, 1);
			validator.IsValidNumber(number).Should().BeTrue();
		}

		[TestCase("--1")]
		[TestCase("+++1")]
		public void IsValid_WithManySigns_False(string number)
		{
			var validator = new NumberValidator(4);
			validator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("asd.fgh")]
		[TestCase("14,1.2")]
		[TestCase(" ")]
		[TestCase("+5f.a2")]
		[TestCase("+")]
		[TestCase(",")]
		[TestCase("1234,")]
		[TestCase(".1234")]
		[TestCase("1234,")]
		public void IsValid_WithNotNumbers_False(string number)
		{
			var validator = new NumberValidator(6, 3);
			validator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("")]
		[TestCase(null)]
		public void IsValid_WithEmptyWord_False(string number)
		{
			var validator = new NumberValidator(4);
			validator.IsValidNumber(number).Should().BeFalse();
		}
		
		[Test]
		public void IsValid_NumberLengthLessThenPrecision()
		{
			var validator = new NumberValidator(4, 2);
			var number = "1423";
			validator.IsValidNumber(number).Should().BeTrue();
		}

		[Test]
		public void IsValid_NumberLengthMoreThenPrecision()
		{
			var validator = new NumberValidator(4, 2);
			var number = "14235";
			validator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase("+1423")]
		[TestCase("-1423")]
		public void IsValid_PrecisionShouldConsiderSign(string number)
		{
			var validator = new NumberValidator(4, 2);
			validator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("14.23", ExpectedResult = true, TestName = "LessThenScale")]
		[TestCase("1.235", ExpectedResult = false, TestName = "MoreThenScale")]
		public bool IsValid_Fraction(string number)
		{
			var validator = new NumberValidator(4, 2);
			return validator.IsValidNumber(number);
		}

		[Test]
		public void IsValid_IntegerAndFractionLengthMoreThenPrecision_False()
		{
			var validator = new NumberValidator(4, 2);
			var number = "12.345";
			validator.IsValidNumber(number).Should().BeFalse();
		}

		[Test]
		public void IsValid_OnlyPositiveWithNegativeNumber_False()
		{
			var validator = new NumberValidator(4, 2, true);
			var number = "-12.3";
			validator.IsValidNumber(number).Should().BeFalse();
		}
	}
}