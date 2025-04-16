using System;
using System.Collections;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.Tests
{
	public class NumberValidator_Should
	{
		#region ConstructorTestsSources

		private static IEnumerable IncorrectConstructorParamsTests()
		{
			yield return new TestCaseData(-1, 2, true, "precision must be a positive number")
				.SetName("Constructor_ThrowsArgumentExceptionOnNegativePrecision");
			yield return new TestCaseData(0, 2, true, "precision must be a positive number")
				.SetName("Constructor_ThrowsArgumentExceptionOnZeroPrecision");
			yield return new TestCaseData(1, 2, true, "precision must be a non-negative number less or equal than precision")
				.SetName("Constructor_ThrowsArgumentExceptionOnPrecisionLessThanScale");
			yield return new TestCaseData(1, 1, true, "precision must be a non-negative number less or equal than precision")
				.SetName("Constructor_ThrowsArgumentExceptionSamePrecisionAndScale");
			yield return new TestCaseData(1, -1, true, "precision must be a non-negative number less or equal than precision")
				.SetName("Constructor_ThrowsArgumentExceptionOnNegativeScale");
		}

		private static IEnumerable CorrectConstructorParamsTests()
		{
			yield return new TestCaseData(2, 1, true)
				.SetName("Constructor_WorksWhenPrecisionIsNonNegativeAndGreaterThanScale");
			yield return new TestCaseData(2, 1, false)
				.SetName("Constructor_WorksWhenPrecisionIsNonNegativeAndGreaterThanScale");
		}

		#endregion

		#region IsValidNumberTestsSources

		private static IEnumerable IsValidNumberPrecisionTests()
		{
			yield return new TestCaseData(3, 2, true, "00.00")
				.SetName("IsValidNumber_ReturnsFalse_WhenSumOfIntPartAndFracPartIsGreaterThanPrecision")
				.Returns(false);
			yield return new TestCaseData(3, 2, true, "-0.00")
				.SetName("IsValidNumber_ReturnsFalse_WhenSignWithFracAndIntPartsIsGreaterThanPrecision")
				.Returns(false);
			yield return new TestCaseData(3, 2, true, "+0.00")
				.SetName("IsValidNumber_ReturnsFalse_WhenSignWithFracAndIntPartsIsGreaterThanPrecision")
				.Returns(false);

			yield return new TestCaseData(17, 2, true, "0.0")
				.SetName("IsValidNumber_ReturnsTrue_WhenSumOfIntPartAndFracPartIsNotGreaterThanPrecision")
				.Returns(true);
			yield return new TestCaseData(17, 2, true, "0")
				.SetName("IsValidNumber_ReturnsTrue_WhenIntPartIsNotGreaterThanPrecision")
				.Returns(true);
			yield return new TestCaseData(17, 2, true, "+0.0")
				.SetName("IsValidNumber_ReturnsTrue_WhenSumOfIntPartAndFracPartIsNotGreaterThanPrecision")
				.Returns(true);
		}

		private static IEnumerable IsValidNumberScaleTests()
		{
			yield return new TestCaseData(17, 2, true, "0.111")
				.SetName("IsValidNumber_ReturnsFalse_WhenFracPartIsGreaterThanScale")
				.Returns(false);

			yield return new TestCaseData(17, 2, true, "0.11")
				.SetName("IsValidNumber_ReturnsTrue_WhenFracPartIsNotGreaterThanScale")
				.Returns(true);
		}

		private static IEnumerable IsValidNumberSignTests()
		{
			yield return new TestCaseData(17, 2, true, "-0.11")
				.SetName("IsValidNumber_ReturnsFalse_WhenSignIsNegativeWithOnlyPositive")
				.Returns(false);

			yield return new TestCaseData(17, 2, true, "+0.11")
				.SetName("IsValidNumber_ReturnsTrue_WhenSignIsPositiveWithOnlyPositive")
				.Returns(true);
			yield return new TestCaseData(17, 2, false, "+0.11")
				.SetName("IsValidNumber_ReturnsTrue_WhenSignIsPositiveWithoutOnlyPositive")
				.Returns(true);
			yield return new TestCaseData(17, 2, false, "-0.11")
				.SetName("IsValidNumber_ReturnsTrue_WhenSignIsNegativeWithoutOnlyPositive")
				.Returns(true);
		}

		private static IEnumerable IsValidNumberValueTests()
		{
			yield return new TestCaseData(17, 2, true, null)
				.SetName("IsValidNumber_ReturnsFalse_WhenNullIsGiven")
				.Returns(false);
			yield return new TestCaseData(17, 2, true, "")
				.SetName("IsValidNumber_ReturnsFalse_WhenEmptyStringIsGiven")
				.Returns(false);
			yield return new TestCaseData(17, 2, true, "a.a")
				.SetName("IsValidNumber_ReturnsFalse_WhenNonDigitStringIsGiven")
				.Returns(false);
		}

		#endregion

		[TestCaseSource(nameof(IncorrectConstructorParamsTests))]
		public void FailsWithIncorrectConstructorArguments(int precision, int scale, bool onlyPositive, string message)
		{
			new Func<NumberValidator>(() => new NumberValidator(precision, scale, onlyPositive))
				.Should()
				.ThrowExactly<ArgumentException>()
				.Where(e => e.Message.Equals(message, StringComparison.OrdinalIgnoreCase));
		}

		[TestCaseSource(nameof(CorrectConstructorParamsTests))]
		public void InitsWithCorrectConstructorArguments(int precision, int scale, bool onlyPositive)
		{
			new Func<NumberValidator>(() => new NumberValidator(precision, scale, onlyPositive))
				.Should()
				.NotThrow();
		}

		[
			TestCaseSource(nameof(IsValidNumberPrecisionTests)),
			TestCaseSource(nameof(IsValidNumberScaleTests)),
			TestCaseSource(nameof(IsValidNumberSignTests)),
			TestCaseSource(nameof(IsValidNumberValueTests)),
		]
		public bool IsValidNumber(int precision, int scale, bool onlyPositive, string value)
		{
			return new Func<NumberValidator>(() => new NumberValidator(precision, scale, onlyPositive))()
				.IsValidNumber(value);
		}
	}
}