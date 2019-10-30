using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
// ReSharper disable ObjectCreationAsStatement

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(13, 6, TestName = "PrecisionIsPositive_ScaleIsPositiveAndLessThanPrecision")]
		[TestCase(1, 0, TestName = "PrecisionIsPositive_ScaleIsZero")]
		public void NumberValidatorConstructor_DoesNotThrow(int precision, int scale) =>
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale));

		[TestCase(0, 1, TestName = "PrecisionIsZero")]
		[TestCase(-1, 0, TestName = "PrecisionIsNegative")]
		[TestCase(2, 2, TestName = "ScaleIsEqualToPrecision")]
		[TestCase(6, -2, TestName = "ScaleIsNegative")]
		public void NumberValidatorConstructor_ThrowArgumentException(int precision, int scale) =>
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));

		[Test]
		public void IsValidNumber_TakesNullOrEmpty_ReturnsFalse([Values(null, "")] string value) =>
			new NumberValidator(2, 1).IsValidNumber(value).Should().BeFalse();

		[TestCase("a.sd", TestName = "OnValueWithNonDigitalSymbols")]
		[TestCase(".1", TestName = "OnEmptyIntPartBeforeSeparator")]
		[TestCase("1.", TestName = "OnEmptyFracPartAfterSeparator")]
		[TestCase("++1.2", TestName = "OnExcessLeadingSigned")]
		[TestCase("  \t\v", TestName = "OnWhitespaceCharacters")]
		[TestCase("1.2.2", TestName = "OnSeveralFracParts")]
		[TestCase("1..2", TestName = "OnExcessSeparators")]
		public void IsValidNumber_TakesInvalidFormatValue_ReturnsFalse(string value) =>
			new NumberValidator(20, 10).IsValidNumber(value).Should().BeFalse();

		[TestCase(5, 4, true, "0.0", TestName = "OnNumberWithCorrectPrecisionAndScale")]
		[TestCase(17, 2, true, "0", TestName = "OnNumberWithCorrectPrecisionAndZeroScale")]
		[TestCase(3, 2, true, "4,42", TestName = "OnNumberWithCommaAsSeparator")]
		[TestCase(4, 2, true, "+1.23", TestName = "OnCorrectNumberWithSign")]
		[TestCase(4, 2, false, "-1.23", TestName = "OnNegativeNumber_WhenOnlyPositiveFlagIsFalse")]
		public void IsValidNumber_ReturnsTrue(int precision, int scale, bool onlyPositive, string value) =>
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();

		[TestCase("0.000", TestName = "OnNumberWithTooMuchScale")]
		[TestCase("10.00", TestName = "OnNumberWithTooMuchPrecision")]
		[TestCase("00.00", TestName = "OnNumberWithTooMuchPrecisionDueToLeadingZeros")]
		[TestCase("+0.00", TestName = "OnZeroWithTooMuchPrecisionDueToLeadingSign")]
		[TestCase("-0.0", TestName = "OnZeroWithLeadingMinus_WhenOnlyPositiveAccept")]
		[TestCase("+1.23", TestName = "OnNumberWithTooMuchPrecisionDueToLeadingSign")]
		[TestCase("-3.5", TestName = "OnNegativeNumber_WhenOnlyPositiveAccept")]
		public void IsValidNumber_ReturnsFalse(string value) =>
			new NumberValidator(3, 2, true).IsValidNumber(value).Should().BeFalse();
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