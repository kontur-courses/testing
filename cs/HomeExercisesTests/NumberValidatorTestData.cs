using System.Collections.Generic;
using HomeExercises;
using NUnit.Framework;

namespace HomeExercisesTests
{
	internal class NumberValidatorTestData
	{
		public static IEnumerable<TestCaseData> CorrectInputCases
		{
			get
			{
				var sut = NumberValidator.Create(17, 2, false);
				yield return new TestCaseData("0", sut, true).SetName("True_When_PositiveInt");
				yield return new TestCaseData("0.0", sut, true).SetName("True_When_PositiveFloatWithDot");
				yield return new TestCaseData("0,0", sut, true).SetName("True_When_PositiveFloatWithComma");
				yield return new TestCaseData("+0", sut, true).SetName("True_When_PositiveIntWithSign");
				yield return new TestCaseData("+0.0", sut, true).SetName("True_When_PositiveFloatWithDotWithSign");
				yield return new TestCaseData("+0,0", sut, true).SetName("True_When_PositiveFloatWithCommaWithSign");
				yield return new TestCaseData("-1", sut, true).SetName("True_When_NegativeInt");
				yield return new TestCaseData("-1.2", sut, true).SetName("True_When_NegativeFloatWithDot");
				yield return new TestCaseData("-1,2", sut, true).SetName("True_When_NegativeFloatWithComma");
			}
		}

		public static IEnumerable<TestCaseData> InvalidInputCases
		{
			get
			{
				var sut = NumberValidator.Create(17, 2, false);
				yield return new TestCaseData(null, sut, false).SetName("False_When_Null");
				yield return new TestCaseData("", sut, false).SetName("False_When_StringIsEmpty");
				yield return new TestCaseData(" ", sut, false).SetName("False_When_StringOfSpace");
				yield return new TestCaseData("abcde", sut, false).SetName("False_When_StringOfLetters");
				yield return new TestCaseData("a.b", sut, false).SetName("False_When_FloatOfLetters");
				yield return new TestCaseData("1a", sut, false).SetName("False_When_LettersInInt");
				yield return new TestCaseData("1.1a", sut, false).SetName("False_When_LettersInFloat");
				yield return new TestCaseData(".1", sut, false).SetName("False_When_FloatMissingIntPart");
				yield return new TestCaseData("1.", sut, false).SetName("False_When_FloatMissingFracPart");
				yield return new TestCaseData("1,0,0", sut, false).SetName("False_When_FloatWithMultipleSeparators");
			}
		}

		public static IEnumerable<TestCaseData> NumberNotMeetValidatorRestrictionsCases
		{
			get
			{
				var sut = NumberValidator.Create(17, 2, false);
				yield return new TestCaseData("1,234", sut, false).SetName("False_When_FracPartLongerThanScale");
				sut = NumberValidator.Create(1, 1, false);
				yield return new TestCaseData("12", sut, false).SetName("False_When_IntPartWithoutSignLongerThanPrecision");
				yield return new TestCaseData("+1", sut, false).SetName("False_When_IntPartWithSignLongerThanPrecision");
				yield return new TestCaseData("0,0", sut, false).SetName("False_When_FloatWithFracPartLongerThanPrecision");
				sut = NumberValidator.Create(17, 2, true);
				yield return new TestCaseData("-0", sut, false).SetName("False_When_NegativeIntWhenOnlyPositive");
				yield return new TestCaseData("-0.0", sut, false).SetName("False_When_NegativeFloatWhenOnlyPositive");
			}
		}
	}
}