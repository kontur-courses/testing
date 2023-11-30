using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCaseSource(typeof(NumberValidatorTestData), nameof(NumberValidatorTestData.ThrowCases))]
		public void ShouldThrow_ArgumentException(int precision, int scale)
		{
			var action = new Func<NumberValidator>(() =>
				NumberValidator.Create(precision, scale));
			action.Should().Throw<ArgumentException>();
		}

		[TestCaseSource(typeof(NumberValidatorTestData), nameof(NumberValidatorTestData.InvalidDataCases))]
		public void ShouldReturn_False_On_InvalidData(int precision, int scale, string value, bool onlyPositive = false)
		{
			NumberValidator.Create(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCaseSource(typeof(NumberValidatorTestData), nameof(NumberValidatorTestData.FalseCases))]
		public void ShouldReturn_False(int precision, int scale, string value)
		{
			NumberValidator.Create(precision, scale).IsValidNumber(value).Should().BeFalse();
		}

		[TestCaseSource(typeof(NumberValidatorTestData), nameof(NumberValidatorTestData.TrueCases))]
		public void ShouldReturn_True(int precision, int scale, string value)
		{
			NumberValidator.Create(precision, scale).IsValidNumber(value).Should().BeTrue();
		}
	}
}