using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{

		[TestCase(0, 0, TestName = "WhenPrecisionZero")]
		[TestCase(-1, 0, TestName = "WhenPrecisionNegative")]
		[TestCase(1, -1, TestName = "WhenScaleNegative")]
		[TestCase(1, 2, TestName = "WhenScaleMoreThenPrecision")]
		public void CtorShould_ThrowArgumentException(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}

		[Test, TestCaseSource(nameof(CasesForRegex))]
		[TestCaseSource(nameof(CasesForEmptyAndNull))]
		[TestCaseSource(nameof(CasesForPrecision))]
		[TestCaseSource(nameof(CasesForScale))]
		[TestCaseSource(nameof(CasesForSign))]
		[TestCaseSource(nameof(CaseForOnlyPositive))]
		public bool IsValidNumber(string number, NumberValidator validator)
		{
			return validator.IsValidNumber(number);
		}

		private static IEnumerable<TestCaseData> CasesForRegex
		{
			get
			{
				var validator = new NumberValidator(10);
				yield return new TestCaseData("1234567890", validator)
					.SetName("ShouldBeTrue_WhenAllDigitsUsed")
					.Returns(true);
				validator = new NumberValidator(4, 2);
				yield return new TestCaseData("1,0", validator)
					.SetName("ShouldBeTrue_WhenAllDigitsUsed")
					.Returns(true);
				yield return new TestCaseData("1.0", validator)
					.SetName("ShouldBeTrue_WhenSeparatorIsDot")
					.Returns(true);
				yield return new TestCaseData("asd.f", validator)
					.SetName("ShouldBeFalse_WhenNotNumber")
					.Returns(false);
				yield return new TestCaseData("1.2.3", validator)
					.SetName("ShouldBeFalse_WithManyDots")
					.Returns(false);
				yield return new TestCaseData("123.", validator)
					.SetName("ShouldBeFalse_WhenEndsWithSeparator")
					.Returns(false);
				yield return new TestCaseData(".123", validator)
					.SetName("ShouldBeFalse_WhenBeginsWithSeparator")
					.Returns(false);
			}
		}

        private static IEnumerable<TestCaseData> CasesForEmptyAndNull
		{
			get
			{
				var validator = new NumberValidator(4, 2);
				yield return new TestCaseData("", validator)
					.SetName("ShouldBeFalse_WhenEmptyWord")
					.Returns(false);
				yield return new TestCaseData(null, validator)
					.SetName("ShouldBeFalse_WhenNull")
					.Returns(false);
			}
		}

		private static IEnumerable<TestCaseData> CasesForPrecision
		{
			get
			{
				var validator = new NumberValidator(4, 2);
				yield return new TestCaseData("123", validator)
					.SetName("ShouldBeTrue_WhenNumberLengthLessThenPrecision")
					.Returns(true);
				yield return new TestCaseData("1234", validator)
					.SetName("ShouldBeTrue_WhenNumberLengthEqualsPrecision")
					.Returns(true);
				yield return new TestCaseData("12345", validator)
					.SetName("ShouldBeFalse_WhenNumberLengthMoreThenPrecision")
					.Returns(false);
			}
		}

		private static IEnumerable<TestCaseData> CasesForScale 
		{
			get
			{
				var validator = new NumberValidator(4, 2);
				yield return new TestCaseData("123.4", validator)
					.SetName("ShouldBeTrue_WhenNumberLengthLessThenScale")
					.Returns(true);
				yield return new TestCaseData("12.34", validator)
					.SetName("ShouldBeTrue_WhenNumberLengthEqualsScale")
					.Returns(true);
				yield return new TestCaseData("1.234", validator)
					.SetName("ShouldBeFalse_WhenNumberLengthMoreThenScale")
					.Returns(false);
			}
		}

		private static IEnumerable<TestCaseData> CasesForSign
		{
			get
			{
				var validator = new NumberValidator(4, 2);
				yield return new TestCaseData("-1234", validator)
					.SetName("PrecisionShouldConsiderMinus")
					.Returns(false);
				yield return new TestCaseData("+1234", validator)
					.SetName("PrecisionShouldConsiderPlus")
					.Returns(false);
			}
		}

		private static IEnumerable<TestCaseData> CaseForOnlyPositive
		{
			get
			{
				var validator = new NumberValidator(2, 0, true);
				yield return new TestCaseData("-1", validator)
					.SetName("ShouldBeFalse_WhenOnlyPositiveTrueAndNumberNegative")
					.Returns(false);
			}
		}
	}
}