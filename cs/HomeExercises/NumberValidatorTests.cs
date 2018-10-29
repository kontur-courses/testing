using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, TestName = "throws when precision is negative")]
		[TestCase(0, 1, TestName = "throws when precision is zero")]
		[TestCase(1, -1, TestName = "throws when scale is negative")]
		[TestCase(2, 3, TestName = "throws when scale is less than precision")]
		[TestCase(2, 3, TestName = "throws when scale equals precision")]
		public void ConstructorShouldThrow(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
		}

		[TestCase(3, 2, TestName = "does not throw when precision > scale > 0")]
		[TestCase(3, 0, TestName = "does not throw when precision > scale = 0")]
		public void ConstructorShouldNotThrow(int precision, int scale)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale));
        }

		[TestCase("", ExpectedResult = false, TestName = "false if empty")]
		[TestCase(null, ExpectedResult = false, TestName = "false if null")]
		[TestCase("123", ExpectedResult = true, TestName = "true on integer within precision")]
		[TestCase("0.1", ExpectedResult = true, TestName = "true on float within scale and precision")]
		[TestCase("-1.1", ExpectedResult = true, TestName = "true on negative number when onlyPositive is false")]
		[TestCase("+1.1", ExpectedResult = true, TestName = "true on number with plus sign")]
        [TestCase("123456", ExpectedResult = false, TestName = "false when out of precision")]
		[TestCase("123.45", ExpectedResult = true, TestName = "does not include point to scale")]
		[TestCase("-12345", ExpectedResult = false, TestName = "false when digits with sign is out of precision")]
		[TestCase("0.123", ExpectedResult = false, TestName = "false when fraction more than scale")]
		[TestCase("12,34", ExpectedResult = true, TestName = "true with comma instead of point")]
		[TestCase("a.b", ExpectedResult = false, TestName = "false when has letters")]
		[TestCase("1.0.2", ExpectedResult = false, TestName = "false when more than one dot")]
		[TestCase("+-1", ExpectedResult = false, TestName = "false when move than one sign")]
		[TestCase(".", ExpectedResult = false, TestName = "false when no digits and point")]
		[TestCase("1.", ExpectedResult = false, TestName = "false when no digits after point")]
		[TestCase(".1", ExpectedResult = false, TestName = "false when no digits before point")]
		[TestCase("000000", ExpectedResult = false, TestName = "false on too many redundant zeros")]
		public bool TestIsValidNumber_WithOnlyPositiveFlagOff(string value)
		{
			return new NumberValidator(5, 2).IsValidNumber(value);
		}

		[TestCase("+1", ExpectedResult = true, TestName = "true on positive with sign")]
		[TestCase("-1", ExpectedResult = false, TestName = "false on negative")]
		[TestCase("1", ExpectedResult = true, TestName = "true when no sign")]
		public bool TestIsValidNumber_WithOnlyPositiveFlagOn(string value)
		{
			return new NumberValidator(5, 2, true).IsValidNumber(value);
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