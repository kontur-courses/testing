using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		public static TestCaseData[] unsignedPositiveNumberTestCases =
		{
			new TestCaseData(1, 0, "0").SetName("When_UnsignedZero"),
			new TestCaseData(2, 1, "0.0").SetName("When_UnsignedZero_WithOneDigitAfter"),
			new TestCaseData(1, 0, "1").SetName("When_UnsignedOneDigitNumber"),
			new TestCaseData(2, 0, "12").SetName("When_UnsignedTwoDigitNumber"),
			new TestCaseData(2, 1, "1.2").SetName("When_UnsignedTwoDigitNumber_WithOneDigitAfterPoint"),
			new TestCaseData(3, 2, "1.23").SetName("When_UnsignedThreeDigitNumber_WithTwoDigitsAfterPoint"),
			new TestCaseData(3, 1, "12.3").SetName("When_UnsignedThreeDigitNumber_WithOneDigitAfterPoint"),
			new TestCaseData(4, 2, "12.34").SetName("When_UnsignedFourDigitNumber_WithTwoDigitsAfterPoint"),
		};

		public static TestCaseData[] signedPositiveNumberTestCases =
		{
			new TestCaseData(2, 0, "+0").SetName("When_PositiveZero"),
			new TestCaseData(3, 1, "+0.0").SetName("When_PositiveZero_WithOneDigitAfter"),
			new TestCaseData(2, 0, "+1").SetName("When_PositiveOneDigitNumber"),
			new TestCaseData(3, 0, "+12").SetName("When_PositiveTwoDigitNumber"),
			new TestCaseData(3, 1, "+1.2").SetName("When_PositiveTwoDigitNumber_WithOneDigitAfterPoint"),
			new TestCaseData(4, 2, "+1.23").SetName("When_PositiveThreeDigitNumber_WithTwoDigitsAfterPoint"),
			new TestCaseData(4, 1, "+12.3").SetName("When_PositiveThreeDigitNumber_WithOneDigitAfterPoint"),
			new TestCaseData(5, 2, "+12.34").SetName("When_PositiveFourDigitNumber_WithTwoDigitsAfterPoint"),
		};

		public static TestCaseData[] negativeNumberTestCases =
		{
			new TestCaseData(2, 0, "-0").SetName("When_NegativeZero"),
			new TestCaseData(3, 1, "-0.0").SetName("When_NegativeZero_WithOneDigitAfter"),
			new TestCaseData(2, 0, "-1").SetName("When_NegativeOneDigitNumber"),
			new TestCaseData(3, 0, "-12").SetName("When_NegativeTwoDigitNumber"),
			new TestCaseData(3, 1, "-1.2").SetName("When_NegativeTwoDigitNumber_WithOneDigitAfterPoint"),
			new TestCaseData(4, 2, "-1.23").SetName("When_NegativeThreeDigitNumber_WithTwoDigitsAfterPoint"),
			new TestCaseData(4, 1, "-12.3").SetName("When_NegativeThreeDigitNumber_WithOneDigitAfterPoint"),
			new TestCaseData(5, 2, "-12.34").SetName("When_NegativeFourDigitNumber_WithTwoDigitsAfterPoint"),
		};

		public static TestCaseData[] wrongCases =
		{
			new TestCaseData(1, 0, "-1")
				.SetName("When_NegativeIntegerNumber_HavePrecisionLesserOrEqual_ThanDigitsCount").Returns(false),
			new TestCaseData(1, 0, "+1")
				.SetName("When_PositiveIntegerNumber_HavePrecisionLesserOrEqual_ThanDigitsCount").Returns(false),
			new TestCaseData(2, 1, "+1.2")
				.SetName("When_PositiveNumberWithFractionalPart_HavePrecisionLesserOrEqual_ThanDigitsCount")
				.Returns(false),
			new TestCaseData(2, 1, "-1.2")
				.SetName("When_NegativeNumberWithFractionalPart_HavePrecisionLesserOrEqual_ThanDigitsCount")
				.Returns(false),
		};

		[TestCaseSource(nameof(unsignedPositiveNumberTestCases))]
		[TestCaseSource(nameof(signedPositiveNumberTestCases))]
		[TestCaseSource(nameof(negativeNumberTestCases))]
		public void IsValidNumber(int precision, int scale, string validatingString)
		{
			new NumberValidator(precision, scale)
				.IsValidNumber(validatingString)
				.Should()
				.BeTrue();
		}

		[TestCaseSource(nameof(wrongCases))]
		public bool IsNotValidNumber(int precision, int scale, string validatingString)
		{
			return new NumberValidator(precision, scale).IsValidNumber(validatingString);
		}

		[TestCaseSource(nameof(negativeNumberTestCases))]
		public void IsFailWithOnlyPositiveFlag(int precision, int scale, string validatingString)
		{
			new NumberValidator(precision, scale, true)
				.IsValidNumber(validatingString)
				.Should()
				.BeFalse();
		}

		[TestCase(3, 2, "a.sd", TestName = "WhenContain_NonDigitSymbols")]
		[TestCase(2, 1, ".0", TestName = "WhenHaveNot_DigitBeforePoint")]
		[TestCase(1, 0, "0.", TestName = "WhenHaveNot_DigitAfterPointIfExist")]
		public void IsWrongFormat(int precision, int scale, string validatingString, bool onlyPositive = true)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(validatingString)
				.Should()
				.BeFalse();
		}

		[TestCase(-1, 1, TestName = "When_NegativePrecision")]
		[TestCase(1, -1, TestName = "When_NegativeScale")]
		[TestCase(-1, -1, TestName = "When_NegativePrecisionAndScale")]
		[TestCase(1, 1, TestName = "When_PrecisionEqualsScale")]
		[TestCase(1, 2, TestName = "When_PrecisionLessThanScale")]
		public void ShouldThrow(int precision, int scale, bool onlyPositive = true)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
		}
	}

	public class NumberValidator
	{
		private readonly Regex numberRegex;
		private readonly bool onlyPositive;
		private readonly int precision;
		private readonly int scale;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			this.precision = precision;
			this.scale = scale;
			this.onlyPositive = onlyPositive;
			if (precision <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");
			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value)
		{
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

			if (string.IsNullOrEmpty(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			// Знак и целая часть
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}