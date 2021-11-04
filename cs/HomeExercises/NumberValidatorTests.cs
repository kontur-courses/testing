using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(0, 2, true, TestName = "precision is zero")]
		[TestCase(-1, 2, true, TestName = "precision is negative")]
		[TestCase(1, 2, true, TestName = "scale is more than precision")]
		[TestCase(2, 2, true, TestName = "scale equals precision")]
		[TestCase(1, -2, true, TestName = "scale is negative")]
		public void Constructor_ThrowsException(int precision, int scale, bool onlyPositives)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositives);
			act.Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, true, TestName = "scale is zero and precision is positive")]
		[TestCase(2, 1, true, TestName = "scale and precision are positive")]
		public void Constructor_NotThrowsException(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().NotThrow<ArgumentException>();
		}

		[TestCase(17, 2, true, null, TestName = "number is null")]
		[TestCase(17, 2, true, "", TestName = "number is empty string")]
		[TestCase(17, 2, true, "lol", TestName = "number is invalid string")]
		[TestCase(17, 2, true, " ", TestName = "number is whitespace")]
		[TestCase(17, 2, true, "   ", TestName = "number is many whitespaces")]
		[TestCase(17, 2, true, "+", TestName = "number doesnt contain digits")]
		[TestCase(17, 2, true, "-0", TestName = "number is minus zero and validator is only positive")]
		[TestCase(3, 2, true, "000.000", TestName = "number is longer than precision")]
		[TestCase(17, 2, true, "000.000", TestName = "fractional part is longer than scale")]
		[TestCase(17, 2, true, "-1", TestName = "validator is only positive and number is negative")]
		public void IsValidNumber_ReturnsFalse(int precision, int scale, bool onlyPositive, string numberToCheck)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(numberToCheck).Should().Be(false);
		}

		[TestCase(17, 2, false, "-1.0", TestName = "number is valid number with dot")]
		[TestCase(17, 2, true, "1.0", TestName = "number is valid number without plus or minus")]
		[TestCase(17, 2, true, "1", TestName = "number is valid number without fractional part")]
		[TestCase(17, 2, true, "1,0", TestName = "number is valid number with comma")]
		[TestCase(17, 2, true, "+1,0", TestName = "number is positive number with plus at the beginning")]
		public void IsValidNumber_ReturnsTrue(int precision, int scale, bool onlyPositive, string numberToCheck)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(numberToCheck).Should().Be(true);
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
			if (precision <= 0) throw new ArgumentException("precision must be a positive number");
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
			if (string.IsNullOrEmpty(value)) return false;
			var match = numberRegex.Match(value);
			if (!match.Success) return false;

			// Знак и целая часть
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4].Value.Length;
			if (intPart + fracPart > precision || fracPart > scale) return false;
			if (onlyPositive && match.Groups[1].Value == "-") return false;
			return true;
		}
	}
}