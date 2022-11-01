using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(0, 2, TestName = "Precision zero")]
		[TestCase(-1, 2, TestName = "Precision negative")]
		[TestCase(1, -2, TestName = "Scale negative")]
		[TestCase(1, 1, TestName = "Scale equals to precision")]
		[TestCase(1, 2, TestName = "Scale is greater than precision")]
		public void Constructor_ThrowExceptions_OnIncorrectArguments(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}


		[TestCase(1, 0, TestName = "Small values")]
		[TestCase(int.MaxValue, 0, TestName = "Big precision value")]
		[TestCase(int.MaxValue, int.MaxValue - 1, TestName = "Big precision and scale values")]
		public void Constructor_ThrowNoExceptions_OnCorrectArguments(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().NotThrow();
		}

		[TestCase(null, TestName = "Null value")]
		[TestCase("", TestName = "Empty")]
		[TestCase("abc", TestName = "Letters")]
		[TestCase("a.bc", TestName = "Letters with dot")]
		[TestCase("%&#", TestName = "Non digits")]
		[TestCase(".", TestName = "Only dot")]
		[TestCase(".56", TestName = "Without integer part")]
		[TestCase("2.", TestName = "Dot without fractional part")]
		[TestCase("2..2", TestName = "More than one dot")]
		[TestCase("2.,2", TestName = "Dot comma mixed")]
		[TestCase("++5", TestName = "More than one plus sign")]
		[TestCase("+-5", TestName = "Plus minus signs mixed")]
		public void IsValidNumber_ReturnFalse_OnIncorrectFormattedString(string value)
		{
			new NumberValidator(5, 2).IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("1234", TestName = "Integer number")]
		[TestCase("12.34", TestName = "Float number")]
		[TestCase("0000", TestName = "Only zeros")]
		[TestCase("0.000", TestName = "Zeros with dot")]
		public void IsValidNumber_ReturnFalse_OnTooLongNumber(string value)
		{
			new NumberValidator(3, 2).IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnFalse_OnTooLongFractionalPart()
		{
			new NumberValidator(1000, 2).IsValidNumber("1.234").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnFalse_OnNegativeNumberForOnlyPositive()
		{
			new NumberValidator(20, 10, true).IsValidNumber("-5").Should().BeFalse();
		}

		public static readonly TestCaseData[] CorrectPositiveValuesTestCaseData =
		{
			new TestCaseData("12").SetName("Integer"),
			new TestCaseData("12345").SetName("Max length integer"),
			new TestCaseData("0.1").SetName("Float with dot"),
			new TestCaseData("0,1").SetName("Float with comma"),
			new TestCaseData("123.45").SetName("Max length float with dot"),
			new TestCaseData("123,45").SetName("Max length float with comma"),
			new TestCaseData("+1234").SetName("Max length integer with plus"),
			new TestCaseData("+12.45").SetName("Max length float with plus"),
			new TestCaseData("00000").SetName("Only zeros"),
			new TestCaseData("00.00").SetName("Only zeros with dot")
		};

		[TestCaseSource(nameof(CorrectPositiveValuesTestCaseData))]
		public void IsValidNumber_ReturnTrue_OnCorrectValueForOnlyPositive(string value)
		{
			new NumberValidator(5, 2, true).IsValidNumber(value).Should().BeTrue();
		}

		[TestCaseSource(nameof(CorrectPositiveValuesTestCaseData))]
		[TestCase("-1234", TestName = "Max length integer with minus")]
		[TestCase("-12.45", TestName = "Max length float with minus")]
		public void IsValidNumber_ReturnTrue_OnCorrectValueForAllRange(string value)
		{
			new NumberValidator(5, 2).IsValidNumber(value).Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnTrue_WithCorrectBigNumbers()
		{
			var bigInteger = new string('5', 10000);
			var bigFloat = new string('1', 5000) + "." + new string('2', 5000);
			var validator = new NumberValidator(int.MaxValue, int.MaxValue - 1);

			validator.IsValidNumber(bigInteger).Should().BeTrue();
			validator.IsValidNumber(bigFloat).Should().BeTrue();
		}


		[Test, Explicit]
		public void Test()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
		}
	}

	public class NumberValidator
	{
		private const string NumberRegexPattern = @"^([+-]?)(\d+)([.,](\d+))?$";
		private static readonly Regex NumberRegex = new(NumberRegexPattern, RegexOptions.IgnoreCase);

		private readonly int _precision;
		private readonly int _scale;
		private readonly bool _onlyPositive;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			if (precision <= 0)
				throw new ArgumentException($"{nameof(precision)} must be a positive number");
			_precision = precision;

			if (scale < 0 || scale >= precision)
				throw new ArgumentException(
					$"{nameof(scale)} must be a non-negative number less or equal than {nameof(precision)}");
			_scale = scale;

			_onlyPositive = onlyPositive;
		}

		public bool IsValidNumber(string value)
		{
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном
			// виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе,
			// включая знак (для отрицательного числа), целую и дробную часть числа без разделяющей десятичной
			// точки, k – максимальное число знаков дробной части числа. Если число знаков дробной части
			// числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

			if (string.IsNullOrEmpty(value))
				return false;

			var match = NumberRegex.Match(value);
			if (!match.Success)
				return false;

			var intPartLength = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			var fracPartLength = match.Groups[4].Value.Length;

			return intPartLength + fracPartLength <= _precision &&
			       fracPartLength <= _scale &&
			       (!_onlyPositive || match.Groups[1].Value != "-");
		}
	}
}