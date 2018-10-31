using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(17, 1, true, "0.0", ExpectedResult = true,
			TestName = "when value decimal length equals scale")]
		[TestCase(17, 2, true, "0.0", ExpectedResult = true,
			TestName = "when value decimal length less than scale")]
		[TestCase(17, 2, true, "0", ExpectedResult = true,
			TestName = "when value contains no decimals")]
		[TestCase(17, 2, true, "0,0", ExpectedResult = true,
			TestName = "when value contains decimal comma")]
		[TestCase(3, 2, false, "1.23", ExpectedResult = true,
			TestName = "when value length equals precision")]
		[TestCase(4, 2, false, "1.23", ExpectedResult = true,
			TestName = "when value length less than precision")]
		[TestCase(4, 2, false, "+1.23", ExpectedResult = true,
			TestName = "when value contains plus")]
		[TestCase(4, 2, false, "-1.23", ExpectedResult = true,
			TestName = "when value contains minus")]
		public bool IsValidNumber_True(int precision, int scale, bool onlyPositive, string entryValue)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(entryValue);
		}

		[TestCase(3, 2, false, null, ExpectedResult = false,
			TestName = "when value is null")]
		[TestCase(3, 2, false, "", ExpectedResult = false,
			TestName = "when value is empty string")]
		[TestCase(3, 2, false, "00.00", ExpectedResult = false,
			TestName = "when value length more than precision")]
		[TestCase(3, 2, false, "+1.23", ExpectedResult = false,
			TestName = "when value contains plus and value length bigger than precision")]
		[TestCase(3, 2, false, "-1.23", ExpectedResult = false,
			TestName = "when value contains minus and value length bigger than precision")]
		[TestCase(4, 2, true, "-0.00", ExpectedResult = false,
			TestName = "when value contains minus and onlyPositive=true")]
		[TestCase(3, 2, false, "w.ш%", ExpectedResult = false,
			TestName = "when value contains non-digits")]
		[TestCase(17, 2, false, "0.000", ExpectedResult = false,
			TestName = "when value decimal length more than scale")]
		[TestCase(17, 8, false, "++0.00", ExpectedResult = false,
			TestName = "when value contains two signs")]
		[TestCase(17, 8, false, "---0.00", ExpectedResult = false,
			TestName = "when value contains more than two signs")]
		[TestCase(17, 8, false, "0.0.0", ExpectedResult = false,
			TestName = "when value contains more than one decimal separator")]
		public bool IsValidNumber_False(int precision, int scale, bool onlyPositive, string entryValue)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(entryValue);
		}

		[TestCase(17, 0, false, TestName = "when scale is zero")]
		[TestCase(17, 2, false, TestName = "when scale is less than precision")]
		public void Constructor_DoesNotThrow(int precision, int scale, bool onlyPositive)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
		}

		[TestCase(-1, 2, false, TestName = "when precision is negative")]
		[TestCase(0, 2, false, TestName = "when precision is zero")]
		[TestCase(17, -2, false, TestName = "when scale is negative")]
		[TestCase(2, 3, false, TestName = "when scale is bigger than precision")]
		[TestCase(17, 17, true, TestName = "when scale equals precision")]
		public void Constructor_Throws_ArgumentException(int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
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