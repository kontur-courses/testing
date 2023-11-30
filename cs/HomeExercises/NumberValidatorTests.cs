using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCaseSource(nameof(ThrowCases))]
		public void ShouldThrow_ArgumentException(int precision, int scale)
		{
			var action = new Func<NumberValidator>(() =>
				NumberValidator.Create(precision, scale));
			action.Should().Throw<ArgumentException>();
		}
		
		public static object[] ThrowCases =
		{
			new TestCaseData(3, 4).SetName("When_ScaleGreaterThanPrecision"),
			new TestCaseData(10, 10).SetName("When_ScaleEqualPrecision"),
			new TestCaseData(-1, 2).SetName("When_NegativePrecision"),
			new TestCaseData(17, -1).SetName("When_NegativeScale")
		};

		[TestCaseSource(nameof(InvalidDataCases))]
		public void ShouldReturn_False_On_InvalidData(int precision, int scale, string value, bool onlyPositive = false)
		{
			NumberValidator.Create(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
		}
		
		public static object[] InvalidDataCases =
		{
			new TestCaseData(17, 2, "ы", false).SetName("When_BadRegex"),
			new TestCaseData(17, 2, "", false).SetName("When_EmptyString"),
			new TestCaseData(17, 2, null!, false).SetName("When_NullString"),
			new TestCaseData(17, 2, null!, true).SetName("When_MinusValue_And_OnlyPositive")
		};

		[TestCaseSource(nameof(FalseCases))]
		public void ShouldReturn_False(int precision, int scale, string value)
		{
			NumberValidator.Create(precision, scale).IsValidNumber(value).Should().BeFalse();
		}

		public static object[] FalseCases =
		{
			new TestCaseData(17, 0, "0.0").SetName("When_FracPartWithZeroScale"),
			new TestCaseData(17, 2, "0.000").SetName("When_FracPartLongerThenScale"),
			new TestCaseData(2, 1, "+0.0").SetName("When_SymbolsMoreThenPrecisionWithPlusSign"),
			new TestCaseData(2, 1, "-0.0").SetName("When_SymbolsMoreThenPrecisionWithMinusSign"),
			new TestCaseData(4, 3, "0.0000").SetName("When_SymbolsMoreThenPrecisionWithScale"),
			new TestCaseData(2, 0, "189").SetName("When_SymbolsMoreThenPrecisionWithoutScale")
		};

		[TestCaseSource(nameof(TrueCases))]
		public void ShouldReturn_True(int precision, int scale, string value)
		{
			NumberValidator.Create(precision, scale).IsValidNumber(value).Should().BeTrue();
		}

		public static object[] TrueCases =
		{
			new TestCaseData(17, 2, "0.0").SetName("When_WithScale"),
			new TestCaseData(17, 2, "0").SetName("When_WithoutScale"),
			new TestCaseData(4, 2, "+1.23").SetName("When_WithPlusSign"),
			new TestCaseData(5, 3, "-1.234").SetName("When_WithMinusSign")
		};
	}
}