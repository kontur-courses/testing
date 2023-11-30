using System.Collections.Generic;
using NUnit.Framework;

namespace HomeExercises
{
	public abstract class NumberValidatorTestData
	{
		public static IEnumerable<TestCaseData> ThrowCases
		{
			get
			{
				yield return new TestCaseData(3, 4).SetName("When_ScaleGreaterThanPrecision");
				yield return new TestCaseData(10, 10).SetName("When_ScaleEqualPrecision");
				yield return new TestCaseData(-1, 2).SetName("When_NegativePrecision");
				yield return new TestCaseData(17, -1).SetName("When_NegativeScale");
			}
		}
		
		public static IEnumerable<TestCaseData> InvalidDataCases
		{
			get
			{
				yield return new TestCaseData(17, 2, "ы", false).SetName("When_BadRegex");
				yield return new TestCaseData(17, 2, "", false).SetName("When_EmptyString");
				yield return new TestCaseData(17, 2, null!, false).SetName("When_NullString");
				yield return new TestCaseData(17, 2, null!, true).SetName("When_MinusValue_And_OnlyPositive");
			}
		}
		
		public static IEnumerable<TestCaseData> FalseCases
		{
			get
			{
				yield return new TestCaseData(17, 0, "0.0").SetName("When_FracPartWithZeroScale");
				yield return new TestCaseData(17, 2, "0.000").SetName("When_FracPartLongerThenScale");
				yield return new TestCaseData(2, 1, "+0.0").SetName("When_SymbolsMoreThenPrecisionWithPlusSign");
				yield return new TestCaseData(2, 1, "-0.0").SetName("When_SymbolsMoreThenPrecisionWithMinusSign");
				yield return new TestCaseData(4, 3, "0.0000").SetName("When_SymbolsMoreThenPrecisionWithScale");
				yield return new TestCaseData(2, 0, "189").SetName("When_SymbolsMoreThenPrecisionWithoutScale");
			}
		}
		
		public static IEnumerable<TestCaseData> TrueCases
		{
			get
			{
				yield return new TestCaseData(17, 2, "0.0").SetName("When_WithScale");
				yield return new TestCaseData(17, 2, "0").SetName("When_WithoutScale");
				yield return new TestCaseData(4, 2, "+1.23").SetName("When_WithPlusSign");
				yield return new TestCaseData(5, 3, "-1.234").SetName("When_WithMinusSign");
			}
		}
	}
}