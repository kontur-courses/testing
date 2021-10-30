using System;
using FluentAssertions;
using HomeExercises;
using NUnit.Framework;
using static HomeExercisesTests.NumberValidatorTestData;

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

		[TestCase(1, 0, false, TestName = "ArgumentsCorrect")]
		public void Create_NotThrow_When(int precision, int scale, bool onlyPositive)
		{
			Action action = () => NumberValidator.Create(precision, scale, onlyPositive);

			action.Should().NotThrow();
		}

		[TestCaseSource(typeof(NumberValidatorTestData), nameof(CorrectInputCases))]
		[TestCaseSource(typeof(NumberValidatorTestData), nameof(InvalidInputCases))]
		[TestCaseSource(typeof(NumberValidatorTestData), nameof(NumberNotMeetValidatorRestrictionsCases))]
		public void IsValidNumber(string value, int precision, int scale, bool onlyPositive, bool expected)
		{
			var validator = NumberValidator.Create(precision, scale, onlyPositive);

			validator.IsValidNumber(value).Should().Be(expected);
		}
	}
}