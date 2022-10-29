using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, TestName = "precision < 0")]
		[TestCase(0, 2, TestName = "precision == 0")]
		[TestCase(1, -2, TestName = "scale < 0")]
		[TestCase(1, 2, TestName = "scale > precision")]
		[TestCase(1, 1, TestName = "scale == precision")]
		public void Constructor_OnIncorrectParams_ThrowArgumentException(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}


		[TestCase(17, 2, false, null, TestName = "string is null")]
		[TestCase(17, 2, false, "", TestName = "string is empty")]
		public void IsValidNumber_EmptyOrNullString_False(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
		}


		[TestCase(17, 10, false, "abc", TestName = "only letters without dot and comma")]
		[TestCase(17, 10, false, "a.bc", TestName = "only letters and dot")]
		[TestCase(17, 10, false, "a,bc", TestName = "only letters and comma")]
		[TestCase(17, 10, false, "1.2.3", TestName = "many dots")]
		[TestCase(17, 10, false, "1,2,3", TestName = "many commas")]
		[TestCase(17, 10, false, "1..1", TestName = "many dots in a row")]
		[TestCase(17, 10, false, "1,,1", TestName = "many commas in a row")]
		[TestCase(17, 10, false, ".1", TestName = "start with dot")]
		[TestCase(17, 10, false, "1.", TestName = "ends with dot")]
		[TestCase(17, 10, false, ",1", TestName = "start with comma")]
		[TestCase(17, 10, false, "1,", TestName = "ends with comma")]
		[TestCase(17, 10, false, "+-12", TestName = "many +-")]
		public void IsValidNumber_NaN_False(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
		}


		[TestCase(17, 2, false, "1.234", TestName = "scale > 0 and < number scale")]
		[TestCase(17, 0, false, "1.0", TestName = "scale == 0 and < number scale")]
		[TestCase(4, 2, false, "123.34", TestName = "precision < number precision, scale > 0")]
		[TestCase(5, 0, false, "123456", TestName = "precision < number precision, scale == 0")]
		public void IsValidNumber_BadScaleOrPrecision_False(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
		}


		[TestCase(2, 0, true, "+1", ExpectedResult = true)]
		[TestCase(1, 0, true, "1", ExpectedResult = true)]
		[TestCase(2, 0, false, "-1", ExpectedResult = true)]
		[TestCase(17, 2, false, "123.34", ExpectedResult = true)]
		[TestCase(6, 0, true, "123456", ExpectedResult = true)]
		[TestCase(17, 2, true, "-123.34", ExpectedResult = false)]
		[TestCase(6, 0, true, "-10", ExpectedResult = false)]
		public bool IsValidNumber_WithOrWithoutSign(int precision, int scale, bool onlyPositive, string value)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
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