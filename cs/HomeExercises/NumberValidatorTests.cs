using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests {
		private NumberValidator largeValidator;
		private NumberValidator smallValidator;

		[SetUp]
		public void SetUp() {
			largeValidator = new NumberValidator(17, 2, true);
			smallValidator = new NumberValidator(3, 2, true);
		}

		[Test]
		public void Test_Exceptions()
		{
			Assert.Multiple(() => {
				const string? alarmForPrecision = "Precision must be positive integer!";
				Assert.Throws<ArgumentException>(() => new NumberValidator(-1), alarmForPrecision);
				Assert.Throws<ArgumentException>(() => new NumberValidator(0), alarmForPrecision);
				Assert.DoesNotThrow(() => new NumberValidator(1), alarmForPrecision);
				
				const string? alarmForScale = "Scale must be non-negative integer!";
				Assert.Throws<ArgumentException>(() => new NumberValidator(1, -1), alarmForScale);
				Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2), alarmForScale);
				Assert.DoesNotThrow(() => new NumberValidator(2, 1), alarmForScale);
			});

			// Assert.IsFalse(smallValidator.IsValidNumber("00.00"));
			// Assert.IsFalse(smallValidator.IsValidNumber("-0.00"));
			// Assert.IsFalse(smallValidator.IsValidNumber("+0.00"));
			//
			// Assert.IsTrue(smallValidator.IsValidNumber("1.23"));
			// Assert.IsFalse(smallValidator.IsValidNumber("+1.23"));
			// Assert.IsFalse(smallValidator.IsValidNumber("-1.23")); 
			// Assert.IsFalse(smallValidator.IsValidNumber("a.sd"));
		}
		
		[TestCase(null, ExpectedResult = false, TestName = "ShouldBeFalse_WhenNullReference")]
		[TestCase("abc", ExpectedResult = false, TestName = "ShouldBeFalse_WhenValueIsLettersOnly")]
		[TestCase("11c", ExpectedResult = false, TestName = "ShouldBeFalse_WhenIntegerContainsLetters")]
		[TestCase("1.2c", ExpectedResult = false, TestName = "ShouldBeFalse_WhenFractContainsLetters")]
		[TestCase("a.bc", ExpectedResult = false, TestName = "ShouldBeFalse_WhenFractIsLettersOnly")]
		
		[TestCase("+1.c", ExpectedResult = false, TestName = "ShouldBeFalse_WhenContainsPlusLettersAndNumbers")]
		[TestCase("-1.c", ExpectedResult = false, TestName = "ShouldBeFalse_WhenContainsMinusLettersAndNumbers")]
		[TestCase("+1", ExpectedResult = true, TestName = "ShouldBeTrue_WhenInputIsPositiveInt_ValidatorOnlyPositive")]
		[TestCase("+99", ExpectedResult = true, TestName = "ShouldBeTrue_WhenInputIsMaxPositiveInt_ValidatorOnlyPositive")]
		[TestCase("+1.0", ExpectedResult = true, TestName = "ShouldBeTrue_WhenInputIsPositiveDecimal_ValidatorOnlyPositive")]
		[TestCase("+9.9", ExpectedResult = true, TestName = "ShouldBeTrue_WhenInputIsMaxPositiveDecimal_ValidatorOnlyPositive")]
		[TestCase("-1", ExpectedResult = false, TestName = "ShouldBeFalse_WhenInputIsNegativeInt_ValidatorOnlyPositive")]
		[TestCase("-99", ExpectedResult = false, TestName = "ShouldBeFalse_WhenInputIsMaxNegativeInt_ValidatorOnlyPositive")]
		[TestCase("-1.0", ExpectedResult = false, TestName = "ShouldBeFalse_WhenInputIsNegativeDecimal_ValidatorOnlyPositive")]
		[TestCase("-9.9", ExpectedResult = false, TestName = "ShouldBeFalse_WhenInputIsMaxNegativeDecimal_ValidatorOnlyPositive")]

		[TestCase("0", ExpectedResult = true, TestName = "ShouldBeTrue_WhenIntegerOnly")]
		[TestCase("999", ExpectedResult = true, TestName = "ShouldBeTrue_WhenMaxPossibleCorrectInteger")]
		[TestCase("9999", ExpectedResult = false, TestName = "ShouldBeTrue_WhenTooBigValue")]
		
		[TestCase("0.0", ExpectedResult = true, TestName = "ShouldBeTrue_WhenSmallCorrectValue")]
		[TestCase("9.99", ExpectedResult = true, TestName = "ShouldBeTrue_WhenMaxPossibleCorrectFraction")]
		[TestCase("0.000", ExpectedResult = false, TestName = "ShouldBeFalse_WhenIncorrectFract")]
		public bool Test_IsValidNumber(string value) {
			return smallValidator.IsValidNumber(value);
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