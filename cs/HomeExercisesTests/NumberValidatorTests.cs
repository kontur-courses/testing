using System;
using FluentAssertions;
using HomeExercises;
using NUnit.Framework;
using static HomeExercisesTests.NumberValidatorTestData;

namespace HomeExercisesTests
{
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

		[TestCaseSource(typeof(NumberValidatorTestData), nameof(CorrectInputCases))]
		[TestCaseSource(typeof(NumberValidatorTestData), nameof(InvalidInputCases))]
		[TestCaseSource(typeof(NumberValidatorTestData), nameof(NumberNotMeetValidatorRestrictionsCases))]
		public void IsValidNumber(string value, NumberValidator sut, bool expected)
		{
			sut.IsValidNumber(value).Should().Be(expected);
		}
	}
}