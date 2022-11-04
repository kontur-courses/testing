using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, -2, TestName = "Precision must be positive")]
		[TestCase(2, 2, TestName = "Scale must be less than precision")]
		[TestCase(1, -1, TestName = "Scale must be non-negative")]
		public void NewNumberValidator_ShouldThrow(int precision, int scale)
		{
			NewValidatorAction(precision, scale, false).Should().Throw<ArgumentException>();
		}

		[TestCase(2, 0, true, "100", ExpectedResult = false, 
			TestName = "Ints are invalid when exceeding precision")]
		[TestCase(2, 1, true, "10.0", ExpectedResult = false, 
			TestName = "Floats are invalid when exceeding precision")]
		[TestCase(1, 0, true, "-0", ExpectedResult = false, 
			TestName = "Precision takes sign into account")]
		public bool PrecisionTestCases(int precision, int scale, bool onlyPositive, string value)
        {
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
        }

		[TestCase(4, 0, false, "+100", ExpectedResult = true,
			TestName = "Plus sign is allowed")]
		[TestCase(4, 0, false, "-100", ExpectedResult = true,
			TestName = "Minus sign is allowed without onlyPositive")]
		[TestCase(4, 0, true, "-100", ExpectedResult = false,
			TestName = "Minus sign is not allowed without onlyPositive")]
		public bool SignTestCases(int precision, int scale, bool onlyPositive, string value)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
		}

		[TestCase(10, 0, true, "0.1", ExpectedResult = false,
			TestName = "Floats are invalid when scale = 0")]
		[TestCase(10, 2, true, "0.111", ExpectedResult = false,
			TestName = "Floats are invalid when frac part exceeds scale")]
		public bool ScaleTestCases(int precision, int scale, bool onlyPositive, string value)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
		}

		[TestCase(10, 5, false, "a", ExpectedResult = false,
			TestName = "Letters are not allowed")]
		[TestCase(10, 5, false, "1e-5", ExpectedResult = false,
			TestName = "E-notation is not allowed")]
		[TestCase(10, 5, false, "1. 5", ExpectedResult = false,
			TestName = "Delimiting spaces are not allowed")]
		[TestCase(10, 5, false, "1.5 ", ExpectedResult = false,
			TestName = "Trailing spaces are not allowed")]
		public bool IsValidNumber_ShouldNotAllowInvalidCharacters(int precision, int scale, bool onlyPositive, string value)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
		}

		[TestCase(10, 0, true, "1234567890", ExpectedResult = true,
			TestName = "Integer")]
		[TestCase(10, 9, true, "1.234567890", ExpectedResult = true,
			TestName = "Float, point-separated")]
		[TestCase(10, 9, true, "1,234567890", ExpectedResult = true,
			TestName = "Float, comma-separated")]
		[TestCase(10, 0, false, "-1000", ExpectedResult = true,
			TestName = "Negative")]
		[TestCase(10, 0, true, "000001", ExpectedResult = true,
			TestName = "Leading zeros")]
		[TestCase(10, 5, true, "1.00000", ExpectedResult = true,
			TestName = "Trailing zeros")]
		public bool IsValidNumber_ShouldAllowValidNumbers(int precision, int scale, bool onlyPositive, string value)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
		}

		public Func<NumberValidator> NewValidatorAction(int precision, int scale, bool onlyPositive) 
		{
			return () => new NumberValidator(precision, scale, onlyPositive);
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
				throw new ArgumentException("scale must be a non-negative number less than precision");
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