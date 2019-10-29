using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[Test]
		[TestCase(17, 2, false, "0.0", true, TestName = "ReturnsTrue_WhenStringContainsDotAndDigitCountLessThanScale")]
		[TestCase(17, 2, false, "0,0", true, TestName =
			"ReturnsTrue_WhenStringContainsCommaAndDigitCountLessThanScale")]
		[TestCase(17, 2, false, "0", true, TestName = "ReturnsTrue_WhenDigitCountLessThanScale")]
		[TestCase(17, 2, false, "00.00", true, TestName =
			"ReturnsTrue_WhenStringContainsDotAndFractionalDigitCountEqualToScale")]
		[TestCase(4, 2, false, "+123", true, TestName = "ReturnsTrue_WhenStringContainsPlusBefore")]
		[TestCase(4, 2, false, "-123", true, TestName = "ReturnsTrue_WhenStringContainsMinusBefore")]
		[TestCase(4, 2, false, "1+2", false, TestName = "ReturnsFalse_WhenContainsPlusInside")]
		[TestCase(4, 2, false, "1-2", false, TestName = "ReturnsFalse_WhenContainsMinusInside")]
		[TestCase(4, 2, false, "12+", false, TestName = "ReturnsFalse_WhenContainsPlusAfter")]
		[TestCase(4, 2, false, "12-", false, TestName = "ReturnsFalse_WhenContainsMinusAfter")]
		[TestCase(3, 2, false, " 12", false, TestName = "ReturnsFalse_WhenSpacesBeforeNumber")]
		[TestCase(3, 2, false, "1 2", false, TestName = "ReturnsFalse_WhenSpacesInsideNumber")]
		[TestCase(3, 2, false, "12 ", false, TestName = "ReturnsFalse_WhenSpacesAfterNumber")]
		[TestCase(4, 0, false, "123", true, TestName = "ReturnsTrue_WithInteger_WhenScaleEqualsToZero")]
		[TestCase(4, 0, false, "123.4", false, TestName = "ReturnsFalse_WithFraction_WhenScaleEqualsToZero")]
		[TestCase(3, 2, false, "1234", false, TestName = "ReturnsFalse_WhenPrecisionIsLessThanDigitCount")]
		[TestCase(3, 2, false, " ", false, TestName = "ReturnsFalse_WithSpaceCharacterString")]
		[TestCase(3, 2, false, null, false, TestName = "ReturnsFalse_WithNullString")]
		[TestCase(3, 1, false, "1.23", false, TestName = "ReturnsFalse_WhenFractionalDigitsCountGreaterThanScale")]
		[TestCase(2, 2, false, "1.23", false, TestName = "ReturnsFalse_WhenDigitsCountGreaterThanPrecision")]
		[TestCase(4, 2, true, "-1.23", false, TestName =
			"ReturnsFalse_WhenOnlyPositiveIsTrueAndNumberStringStartsWithMinus")]
		[TestCase(4, 2, false, ".23", false, TestName = "ReturnsFalse_WhenNumberDoesntHaveIntegerPart")]
		[TestCase(4, 2, false, "12.", false, TestName = "ReturnsFalse_WhenNumberDoesntHaveFractionalPartButHaveADot")]
		[TestCase(4, 2, false, "++12", false, TestName = "ReturnsFalse_WhenContainsMultiplePluses")]
		[TestCase(4, 2, false, "--12", false, TestName = "ReturnsFalse_WhenContainsMultipleMinuses")]
		[TestCase(4, 2, false, "+-12", false, TestName = "ReturnsFalse_WhenContainsPlusMinus")]
		[TestCase(4, 2, false, "-+12", false, TestName = "ReturnsFalse_WhenContainsMinusPlus")]
		public void TestNumberValidator(int precision, int scale, bool onlyPositive,
			string numString, bool expected)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numString).Should().Be(expected);
		}

		[Test]
		[TestCase(-1, 2, TestName = "Fails_WhenPrecisionLessThanZero")]
		[TestCase(1, 2, TestName = "Fails_WhenScaleGreaterThanPrecision")]
		[TestCase(1, -1, TestName = "Fails_WhenScaleLessThanZero")]
		public void TestNumberValidator_Throws_Exceptions(int precision, int scale)
		{
			new Func<NumberValidator>(() => new NumberValidator(precision, scale)).Should().Throw<ArgumentException>();
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
			if (scale < 0 || scale > precision)
				throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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