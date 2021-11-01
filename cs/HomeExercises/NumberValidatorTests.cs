using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
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

		[TestCase(-1, 2, true, TestName = "Precision can't be negative")]
		[TestCase(0, 1, true, TestName = "Precision can't be equal to zero")]
		[TestCase(1, -1, true, TestName = "Scale can't be negative")]
		[TestCase(5, 10, true, TestName = "Scale can't be more than precision")]
		[TestCase(5, 5, true, TestName = "Scale can't be equal to precision")]
		public void Constructor_ThrowsArgumentException_WhenInvalidParametersGiven(
			int precision,
			int scale,
			bool positiveOnly)
		{
			Action action = () => new NumberValidator(precision, scale, positiveOnly);
			action.Should().Throw<ArgumentException>();
		}


		[TestCase("", TestName = "Empty string is not a number")]
		[TestCase(null, TestName = "Null is not a number")]
		[TestCase("      ", TestName = "Multiplied white spaces are not a number")]
		[TestCase("abc", TestName = "Letters are not a number")]
		[TestCase("a1b2c3", TestName = "Group of letters and digits are not a number")]
		[TestCase("-", TestName = "Symbol '-' is not a number")]
		[TestCase("+", TestName = "Symbol '+' is not a number")]
		[TestCase("+-12", TestName = "Minus are plus are not allowed together")]
		[TestCase("12.3.4", TestName = "Two points in number are not allowed")]
		[TestCase(".12", TestName = "Number can't start from point")]
		[TestCase("12.", TestName = "Number can't end on point")]
		[TestCase("12.,3", TestName = "Point and comma are not allowed together")]
		public void IsValidNumber_ReturnFalse_WhenInputInInvalidFormat(string input)
		{
			var validator = new NumberValidator(10, 5, true);

			validator.IsValidNumber(input).Should().BeFalse();
		}

		[TestCase(3, 0, true, "1234", TestName = "Number with integer part more than precision")]
		[TestCase(3, 2, true, "12.34", TestName = "Number with integer plus fraction parts more than precision")]
		[TestCase(5, 2, true, "0.123", TestName = "Number with fractional part more than scale")]
		[TestCase(5, 0, true, "-123", TestName = "Negative number when only positive allowed")]
		[TestCase(3, 0, true, "+123", TestName = "Number length with symbol '+' is more than precision")]
		[TestCase(3, 0, false, "-123", TestName = "Number length with symbol '-' is more than precision")]
		public void IsValidNumber_ReturnFalse_WhenNumberDoesNotMatchRequirements(
			int precision,
			int scale,
			bool positiveOnly,
			string input)
		{
			var validator = new NumberValidator(precision, scale, positiveOnly);

			validator.IsValidNumber(input).Should().BeFalse();
		}

		[TestCase(5,
			0,
			true,
			"123",
			TestName =
				"Number is integer and it has length less than precision")]
		[TestCase(5,
			0,
			true,
			"12345",
			TestName =
				"Number is integer and it has same length as precision")]
		[TestCase(5,
			2,
			true,
			"1.4",
			TestName =
				"Fractional part lenght is less than scale")]
		[TestCase(5,
			2,
			true,
			"1.45",
			TestName =
				"Fractional part lenght is same as scale")]
		[TestCase(5,
			2,
			true,
			"123.45",
			TestName =
				"Sum of Integer and Fractional parts lenght same as precision")]
		[TestCase(5,
			0,
			false,
			"-123",
			TestName =
				"Negative integer have size less than precision and negative numbers Allowed")]
		[TestCase(5,
			2,
			false,
			"+12.3",
			TestName =
				"Correct positive number when negative numbers allowed")]
		public void IsValidNumber_ReturnTrue_WhenInputMatchesRules(
			int precision,
			int scale,
			bool positiveOnly,
			string input)
		{
			var validator = new NumberValidator(precision, scale, positiveOnly);

			validator.IsValidNumber(input).Should().BeTrue();
		}



		//Нашел 2 сценария, на которых реализация возвращает неправильный ответ 
		/*[Test]
		public void IsValidNumber_ReturnTrue_WhenOnlyPositiveNumbersAllowedAndMinusZeroGiven()
		{
			var validator = new NumberValidator(10, 2, true);
			var input = "-0";
		
			validator.IsValidNumber(input).Should().BeTrue();
		}
		
		[Test]
		public void IsValidNumber_ReturnFalse_WhenNonZeroNumberStartsFromZero()
		{
			var validator = new NumberValidator(10, 5, true);
			var input = "0123";
		
			validator.IsValidNumber(input).Should().BeFalse();
		}*/
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