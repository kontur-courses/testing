using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(5, -1, TestName = "when scale is negative number")]
		[TestCase(5, 6, TestName = "when scale is greater then precision")]
		[TestCase(5, 5, TestName = "when scale is equal to precision")]
		[TestCase(-1, TestName = "when precision is negative number")]
		[TestCase(0, TestName = "when precision is zero")]
		public void ThrowArgumentException(int precision, int scale = 0)
		{
			Action act = () => new NumberValidator(precision, scale);
			act.Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, TestName = "when create with valid arguments")]
		public void DoesNotThrowException(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision, scale, true);
			act.Should().NotThrow();
        }

		[TestCase(17, 2, "0.0", TestName = "when number with point is correct")]
		[TestCase(17, 2, "0", TestName = "when number without fraction is correct")]
		[TestCase(4, 2, "+1.23", TestName = "when number with plus sign is equals to precision")]
		[TestCase(17, 2, "0,0", TestName = "when number with comma is correct")]
		[TestCase(17, 0, "0", TestName = "when scale is zero and number is integer")]
		public void ReturnTrue(int precision, int scale, string value)
		{
			var result = new NumberValidator(precision, scale).IsValidNumber(value);
			result.Should().BeTrue();
        }

		[TestCase(3, 2, "00.00", TestName = "when number is greater than precision")]
		[TestCase(5, 2, "-0.00", true, TestName = "when negative numbers are forbidden and number is negative")]
		[TestCase(3, 2, "+0.00", true, TestName = "when number with plus sign is greater then precision")]
		[TestCase(3, 2, "-1.23", true, TestName = "when number with minus sign is greater then precision")]
		[TestCase(17, 2, "0.000", true, TestName = "when fraction is greater then allowed")]
		[TestCase(3, 2, "a.sd", true, TestName = "when value is not a number")]
		[TestCase(17, 2, "  0,0", true, TestName = "when value starts with whitespaces")]
		[TestCase(17, 2, "0,0  ", true, TestName = "when value ends with whitespaces")]
		[TestCase(17, 2, " 0 , 0  ", true, TestName = "when value have whitespaces")]
		[TestCase(17, 2, null, true, TestName = "when value is null")]
		[TestCase(17, 2, "", true, TestName = "when value is empty")]
		public void ReturnFalse(int precision, int scale, string value, bool onlyPositive = false)
		{
			var result = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
			result.Should().BeFalse();
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