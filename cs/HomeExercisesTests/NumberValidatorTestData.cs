using System.Collections;
using NUnit.Framework;

namespace HomeExercisesTests
{
	internal class NumberValidatorTestData
	{
		public static IEnumerable CorrectInputCases => new[]
		{
			new TestCaseData("0", 17, 2, false, true).SetName("True_When_PositiveInt"),
			new TestCaseData("0.0",17, 2, false, true).SetName("True_When_PositiveFloatWithDot"),
			new TestCaseData("0,0", 17, 2, false, true).SetName("True_When_PositiveFloatWithComma"),
			new TestCaseData("+0", 17, 2, false, true).SetName("True_When_PositiveIntWithSign"),
			new TestCaseData("+0.0", 17, 2, false, true).SetName("True_When_PositiveFloatWithDotWithSign"),
			new TestCaseData("+0,0", 17, 2, false, true).SetName("True_When_PositiveFloatWithCommaWithSign"),
			new TestCaseData("-1", 17, 2, false, true).SetName("True_When_NegativeInt"),
			new TestCaseData("-1.2", 17, 2, false, true).SetName("True_When_NegativeFloatWithDot"),
			new TestCaseData("-1,2", 17, 2, false, true).SetName("True_When_NegativeFloatWithComma")
		};

		public static IEnumerable InvalidInputCases => new[]
		{
			new TestCaseData(null, 17, 2, false, false).SetName("False_When_Null"),
			new TestCaseData("", 17, 2, false, false).SetName("False_When_StringIsEmpty"),
			new TestCaseData(" ", 17, 2, false, false).SetName("False_When_StringOfSpace"),
			new TestCaseData("abcde", 17, 2, false, false).SetName("False_When_StringOfLetters"),
			new TestCaseData("a.b", 17, 2, false, false).SetName("False_When_FloatOfLetters"),
			new TestCaseData("1a", 17, 2, false, false).SetName("False_When_LettersInInt"),
			new TestCaseData("1.1a", 17, 2, false, false).SetName("False_When_LettersInFloat"),
			new TestCaseData(".1", 17, 2, false, false).SetName("False_When_FloatMissingIntPart"),
			new TestCaseData("1.", 17, 2, false, false).SetName("False_When_FloatMissingFracPart"),
			new TestCaseData("1,0,0", 17, 2, false, false).SetName("False_When_FloatWithMultipleSeparators")
		};

		public static IEnumerable NumberNotMeetValidatorRestrictionsCases => new[]
		{
			new TestCaseData("1,234", 17, 2, false, false).SetName("False_When_FracPartLongerThanScale"),
			new TestCaseData("12", 1, 1, false, false).SetName("False_When_IntPartWithoutSignLongerThanPrecision"),
			new TestCaseData("+1", 1, 1, false, false).SetName("False_When_IntPartWithSignLongerThanPrecision"),
			new TestCaseData("0,0", 1, 1, false, false).SetName("False_When_FloatWithFracPartLongerThanPrecision"),
			new TestCaseData("-0", 17, 2, true, false).SetName("False_When_NegativeIntWhenOnlyPositive"),
			new TestCaseData("-0.0", 17, 2, true, false).SetName("False_When_NegativeFloatWhenOnlyPositive")
		};
	}
}