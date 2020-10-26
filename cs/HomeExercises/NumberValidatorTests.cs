using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private string CreateTestMessage(int precision, int scale = 0,
			bool onlyPositive = false, string? isValidNumberParameter = null)
		{
			var message = $"precision = {precision}\n" +
			              $"scale = {scale}\n" +
			              $"onlyPositive = {onlyPositive}\n";
			if (isValidNumberParameter != null)
				message += $"IsValidNumber parameter = {isValidNumberParameter}\n";
			return message;
		}

		[Test]
		public void Ctor_WithValidParameters_DoesNotThrowException()
		{
			Assert.DoesNotThrow(
				() => new NumberValidator(3, 1, true),
				CreateTestMessage(3, 1, true));
		}

		[TestCase(0, TestName = "PrecisionEqualZero")]
		[TestCase(-1, TestName = "PrecisionLessThanZero")]
		public void Ctor_PrecisionLessOrEqualZero_ThrowException(int precision)
		{
			Assert.Throws<ArgumentException>(
				() => new NumberValidator(precision),
				CreateTestMessage(precision));
		}

		[Test]
		public void Ctor_ScaleLessThanZero_ThrowException()
		{
			Assert.Throws<ArgumentException>(
				() => new NumberValidator(2, -1),
				CreateTestMessage(2, -1));
		}

		[TestCase(2, 3, TestName = "ScaleGreaterThanPrecision")]
		[TestCase(2, 2, TestName = "ScaleEqualPrecision")]
		public void Ctor_ScaleGreaterOrEqualPrecision_ThrowException(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(
				() => new NumberValidator(precision, scale),
				CreateTestMessage(precision, scale));
		}

		[TestCase(null, TestName = "GotNull")]
		[TestCase("", TestName = "GotEmptyString")]
		public void IsValidNumber_GotNullOrEmpty_ReturnFalse(string isValidNumberParameter)
		{
			var validator = new NumberValidator(17);

			Assert.IsFalse(validator.IsValidNumber(isValidNumberParameter));
		}

		[TestCase("a.bc", TestName = "LettersInsteadNumbers")]
		[TestCase("1.", TestName = "ParameterHasSeparatorButNoDecimalPart")]
		[TestCase(".54", TestName = "ParameterHasSeparatorButNoIntegerPart")]
		[TestCase("+", TestName = "ParameterIsJustSignOfNumber")]
		[TestCase("1,23,4", TestName = "ParameterHasMoreThanOneSeparator")]
		[TestCase("++1.23", TestName = "ParameterHasMoreThanOneSignOfNumber")]
		[TestCase("1;23", TestName = "SeparatorIsNotDotOrComma")]
		public void IsValidNumber_ParameterHasNotValidForm_ReturnFalse(string isValidNumberParameter)
		{
			var validator = new NumberValidator(10, 4);

			Assert.IsFalse(validator.IsValidNumber(isValidNumberParameter),
				CreateTestMessage(10, 4, isValidNumberParameter: isValidNumberParameter));
		}

		[Test]
		public void IsValidNumber_DecimalPartOfParameterGreaterThanScale_ReturnFalse()
		{
			var validator = new NumberValidator(10, 1);

			Assert.IsFalse(validator.IsValidNumber("1.23"),
				CreateTestMessage(10, 1, isValidNumberParameter: "1.23"));
		}

		[Test]
		public void IsValidNumber_SignCountOfParameterGreaterThanPrecision_ReturnFalse()
		{
			var validator = new NumberValidator(3, 2);

			Assert.IsFalse(validator.IsValidNumber("12.34"),
				CreateTestMessage(3, 2, isValidNumberParameter: "12.34"));
		}

		[TestCase(".", TestName = "SeparatorIsDot")]
		[TestCase(",", TestName = "SeparatorIsComma")]
		public void IsValidNumber_SeparatorIsNotIncludeInPrecision(string separator)
		{
			var validator = new NumberValidator(2, 1);

			Assert.IsTrue(validator.IsValidNumber($"0{separator}0"),
				CreateTestMessage(2, 1, isValidNumberParameter: $"0{separator}0"));
		}

		[TestCase(".", TestName = "SeparatorIsDot")]
		[TestCase(",", TestName = "SeparatorIsComma")]
		public void IsValidNumber_SeparatorCanBeDotOrComma(string separator)
		{
			var validator = new NumberValidator(5, 2);

			Assert.IsTrue(validator.IsValidNumber($"0{separator}0"),
				CreateTestMessage(5, 2, isValidNumberParameter: $"0{separator}0"));
		}

		[TestCase("+", TestName = "PlusIsCounted")]
		[TestCase("-", TestName = "MinusIsCounted")]
		public void IsValidNumber_SignOfNumberIncludeInPrecision(string sign)
		{
			var validator = new NumberValidator(3, 2);

			Assert.IsFalse(validator.IsValidNumber(sign + "1.23"),
				CreateTestMessage(3, 2, isValidNumberParameter: sign + "1.23"));
		}

		[Test]
		public void IsValidNumber_GotNegativeNumberWhenExpectOnlyPositive_ReturnFalse()
		{
			var validator = new NumberValidator(5, onlyPositive: true);

			Assert.IsFalse(validator.IsValidNumber("-12"),
				CreateTestMessage(5, onlyPositive: true, isValidNumberParameter: "-12"));
		}

		[Test]
		public void IsValidNumber_WhenScaleIsNotZero_ParameterCanBeWithoutDecimalPart()
		{
			var validator = new NumberValidator(10, 2);

			Assert.IsTrue(validator.IsValidNumber("123"),
				CreateTestMessage(10, 2, isValidNumberParameter: "123"));
		}

		[Test]
		public void IsValidNumber_WhenGotValidNumber_ReturnTrue()
		{
			var validator = new NumberValidator(17, 2, true);

			Assert.IsTrue(validator.IsValidNumber("0.0"),
				CreateTestMessage(17, 2, true, "0.0"));
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