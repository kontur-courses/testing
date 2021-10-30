using System;
using FluentAssertions;
using HomeExercises;
using NUnit.Framework;

namespace HomeExercisesTests
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(-1, 0, "precision must be a positive number", TestName = "PrecisionIsNegative")]
		[TestCase(1, -1, "scale must be a non-negative number less or equal than precision", TestName = "ScaleIsNegative")]
		[TestCase(1, 2, "scale must be a non-negative number less or equal than precision", TestName = "ScaleIsGreaterThanPrecision")]
		public void Create_ThrowArgumentException_When(int precision, int scale, string expectedMessage)
		{
			Action action = () => NumberValidator.Create(precision, scale);

			action.Should().Throw<ArgumentException>()
				.WithMessage(expectedMessage);
		}

		[TestCase(1, 0, false, TestName = "ArgumentsCorrectAndOnlyPositiveFalse")]
		[TestCase(1, 0, true, TestName = "ArgumentsCorrectAndOnlyPositiveTrue")]
		public void Create_NotThrow_When(int precision, int scale, bool onlyPositive)
		{
			Action action = () => NumberValidator.Create(precision, scale, onlyPositive);

			action.Should().NotThrow();
		}

		[TestCase("0", TestName = "PositiveInt")]
		[TestCase("0.0", TestName = "PositiveFloatWithDot")]
		[TestCase("0,0", TestName = "PositiveFloatWithComma")]
		[TestCase("+0", TestName = "PositiveIntWithPlus")]
		[TestCase("+0.0", TestName = "PositiveFloatWithDotWithPlus")]
		[TestCase("+0,0", TestName = "PositiveFloatWithCommaWithPlus")]
		[TestCase("-1", TestName = "NegativeInt")]
		[TestCase("-1.2", TestName = "NegativeFloatWithDot")]
		[TestCase("-1,2", TestName = "NegativeFloatWithComma")]
		public void IsValidNumber_True_When(string value)
		{
			var sut = NumberValidator.Create(17, 2);

			sut.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase(null, TestName = "Null")]
		[TestCase("", TestName = "StringIsEmpty")]
		[TestCase(" ", TestName = "StringOfSpace")]
		[TestCase("abcde", TestName = "StringOfLetters")]
		[TestCase("a.b", TestName = "FloatOfLetters")]
		[TestCase("1a", TestName = "LettersInInt")]
		[TestCase("1.1a", TestName = "LettersInFloat")]
		[TestCase(".1", TestName = "FloatMissingIntPart")]
		[TestCase("1.", TestName = "FloatMissingFracPart")]
		[TestCase("1,0,0", TestName = "FloatWithMultipleSeparators")]
		[TestCase("1,234", 17, 2, TestName = "FracPartLongerThanScale")]
		[TestCase("12", 1, 1, TestName = "IntPartWithoutSignLongerThanPrecision")]
		[TestCase("+1", 1, 1, TestName = "IntPartWithSignLongerThanPrecision")]
		[TestCase("0,0", 1, 1, TestName = "FloatWithFracPartLongerThanPrecision")]
		[TestCase("-0", 17, 2, true, TestName = "NegativeIntWhenOnlyPositive")]
		[TestCase("-0.0", 17, 2, true, TestName = "NegativeFloatWhenOnlyPositive")]
		public void IsValidNumber_False_When(string value, int precision = 17, int scale = 2, bool onlyPositive = false)
		{
			var sut = NumberValidator.Create(precision, scale, onlyPositive);

			sut.IsValidNumber(value).Should().BeFalse();
		}
	}
}