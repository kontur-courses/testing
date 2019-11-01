using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-123, 2, true, TestName = "Scale < 0")]
		[TestCase(0, 2, true, TestName = "Precision == 0")]
		[TestCase(-123, 2, true, TestName = "Precision < 0")]
		[TestCase(2, 2, true, TestName = "Precision == Scale")]
		[TestCase(1, 2, true, TestName = "Precision < Scale")]
		public void Constructor_IncorrectInput_ThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			Calling.ThisLambda(() => new NumberValidator(precision, scale, onlyPositive))
				.ShouldThrow<ArgumentException>();
		}

		[TestCase(3, 2, true, TestName = "Precision and scale not zero and scale < precision")]
		[TestCase(10, 0, true, TestName = "Scale zero in constructor")]
		public void Constructor_CorrectInput_NotThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			Calling.ThisLambda(() => new NumberValidator(precision, scale, onlyPositive))
				.ShouldNotThrow<ArgumentException>();
		}

		[TestCase("+1.23", TestName = "With plus symbol and fractional part")]
		[TestCase("-1.23", TestName = "With minus symbol and fractional part")]
		[TestCase("1,23", TestName = "Fractional part with comma")]
		[TestCase("-1,23", TestName = "Fractional part with comma and with minus symbol")]
		[TestCase("0", TestName = "Int part zero")]
		[TestCase("0.0", TestName = "Int and frac part zero")]
		[TestCase("1234567890", TestName = "All digit apply")]
		public void IsValidNumber_CorrectInputString_ReturnsTrue(string value) 
		{
			new NumberValidator(12, 2).IsValidNumber(value).Should().BeTrue();
		}

		[TestCase("", TestName = "Empty string")]
		[TestCase(null, TestName = "Null input")]
		[TestCase("sdk", TestName = "Other symbols input")]
		[TestCase("ad.s", TestName = "Other symbols input in frac and int parts")]
		[TestCase("0..0", TestName = "Double point")]
		[TestCase("0.,0", TestName = "Point and comma")]
		[TestCase("0.", TestName = "Empty frac part")]
		[TestCase(".0", TestName = "Empty int part")]
		[TestCase("--0.0", TestName = "Double sign symbol")]
		[TestCase("1,hi", TestName = "Fractional part with comma and other symbols")]
		[TestCase("sd,2", TestName = "Fractional part with comma and other symbols in int part")]
		public void IsValidNumber_WhenImpossibleParseInput_ReturnFalse(string value)
		{
			new NumberValidator(3,2).IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("000.00", TestName = "Length of frac and int parts more than precision")]
		[TestCase("00000", TestName = "Length more than precision without frac part")]
		[TestCase("-00.00", TestName = "Length with sign more than precision")]
		[TestCase("0.000", TestName = "Too big scale")]
		[TestCase("-12.0", TestName = "Negative value when only positive")]
		[TestCase("-0.0", TestName = "Negative zero when only positive")]
		public void IsValidNumber_WhenNumberIsBreakingRules_ReturnFalse(string value)
		{
			new NumberValidator(4, 2, true).IsValidNumber(value).Should().BeFalse();
		}
	}

	public static class Calling
	{
		public static Action ThisLambda(this Action act) => act;
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
				throw new ArgumentException("scale must be a non-negative number, less than precision");
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
