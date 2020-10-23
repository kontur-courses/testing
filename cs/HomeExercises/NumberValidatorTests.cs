using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
		public class NumberValidatorTests {
		private NumberValidator simpleValidator;
		private NumberValidator positiveOnlyValidator;
		private NumberValidator plainValidator;

		[SetUp]
		public void SetUp() {
			simpleValidator = new NumberValidator(3);
			plainValidator = new NumberValidator(3, 2);
			positiveOnlyValidator = new NumberValidator(3, 2, true);
		}

		[TestCase(-1, 0, TestName = "Should_ArgumentException_WhenPrecisionIsNegative")]
		[TestCase(0, 0, TestName = "Should_ArgumentException_WhenPrecisionIsZero")]
		[TestCase(1, -1, TestName = "Should_ArgumentException_WhenScaleIsNegative")]
		[TestCase(1, 2, TestName = "Should_ArgumentException_WhenScaleGreaterPrecision")]
		public void Test_NumberValidatorConstructor_OnIncorrectInputs(int precision, int scale) {
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
		}
		
<<<<<<< HEAD
		[TestCase(1, 0, TestName = "Should_NotException_WhenScaleIsZero")]
		[TestCase(3, 2, TestName = "Should_NotException_WhenScaleLessPrecision")]
		[TestCase(100, 99, TestName = "Should_NotException_WhenPrecisionScaleAreBigAndCorrect")]
=======
		[TestCase(1, 0, TestName = "Should_NotException_WhenPrecisionIsNegative")]
		[TestCase(3, 2, TestName = "Should_NotException_WhenScaleIsNegative")]
		[TestCase(100, 99, TestName = "Should_NotException_WhenScaleGreaterPrecision")]
>>>>>>> 4375c8585f50df411a2da7b32b93c51d57fa7f98
		public void Test_NumberValidatorConstructor_OnCorrectInputs(int precision, int scale) {
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale));
		}


		[TestCase(null, ExpectedResult = false, TestName = "Should_BeFalse_WhenNullReference")]
		[TestCase("abc", ExpectedResult = false, TestName = "Should_BeFalse_WhenValueIsLettersOnly")]
		[TestCase("1vc", ExpectedResult = false, TestName = "Should_BeFalse_WhenIntegerContainsLetters")]
		[TestCase("1.2c", ExpectedResult = false, TestName = "Should_BeFalse_WhenFractContainsLetters")]
		[TestCase("a.bc", ExpectedResult = false, TestName = "Should_BeFalse_WhenFractIsLettersOnly")]
		[TestCase("+a.b", ExpectedResult = false, TestName = "Should_BeFalse_WhenLettersWithSign")]
		[TestCase("+1.c", ExpectedResult = false, TestName = "Should_BeFalse_WhenSignedLettersAndNumbers")]
		[TestCase("+-1", ExpectedResult = false, TestName = "Should_BeFalse_WhenTwiceSigns")]
		[TestCase("9999", ExpectedResult = false, TestName = "Should_BeTrue_WhenTooBigValue")]
		[TestCase("0.000", ExpectedResult = false, TestName = "Should_BeFalse_WhenTooBigFract")]
		public bool Test_IsValidNumber_OnIncorrectInputs(string value) {
			return simpleValidator.IsValidNumber(value);
		}

		[TestCase("0", ExpectedResult = true, TestName = "Should_BeTrue_WhenIntegerOnly")]
		[TestCase("999", ExpectedResult = true, TestName = "Should_BeTrue_WhenMaxPossibleCorrectInteger")]
		[TestCase("+10", ExpectedResult = true, TestName = "Should_BeTrue_WhenPositiveSignedInteger")]
		[TestCase("-10", ExpectedResult = true, TestName = "Should_BeTrue_WhenNegativeSignedInteger")]

		[TestCase("0.0", ExpectedResult = true, TestName = "Should_BeTrue_WhenSmallCorrectValue")]
		[TestCase("0,0", ExpectedResult = true, TestName = "Should_BeTrue_WhenSeparatorIsComma")]
		[TestCase("9.99", ExpectedResult = true, TestName = "Should_BeTrue_WhenMaxPossibleCorrectDecimal")]
		[TestCase("+9.9", ExpectedResult = true, TestName = "Should_BeTrue_WhenPositiveSignedDecimal")]
		[TestCase("-9.9", ExpectedResult = true, TestName = "Should_BeTrue_WhenNegativeSignedDecimal")]
		public bool Test_IsValidNumber_CorrectInputs_SignDontMatter(string value) {
			return plainValidator.IsValidNumber(value);
		}

		[TestCase("+1", ExpectedResult = true, TestName = "Should_BeTrue_WhenInputIsPositiveInt")]
		[TestCase("+99", ExpectedResult = true, TestName = "Should_BeTrue_WhenInputIsMaxPositiveInt")]
		[TestCase("+1.0", ExpectedResult = true, TestName = "Should_BeTrue_WhenInputIsPositiveDecimal")]
		[TestCase("+9.9", ExpectedResult = true, TestName = "Should_BeTrue_WhenInputIsMaxPositiveDecimal")]
		[TestCase("-1", ExpectedResult = false, TestName = "Should_BeFalse_WhenInputIsNegativeInt")]
		[TestCase("-99", ExpectedResult = false, TestName = "Should_BeFalse_WhenInputIsMaxNegativeInt")]
		[TestCase("-1.0", ExpectedResult = false, TestName = "Should_BeFalse_WhenInputIsNegativeDecimal")]
		[TestCase("-9.9", ExpectedResult = false, TestName = "Should_BeFalse_WhenInputIsMaxNegativeDecimal")]
		public bool Test_IsValidNumber_OnlyPositiveInputs(string value) {
			return positiveOnlyValidator.IsValidNumber(value);
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