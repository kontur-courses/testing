using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, TestName = "Precision is negative")]
		[TestCase(0, 2, TestName = "Precision is zero")]
		[TestCase(1, -1, TestName = "Scale is negative")]
		[TestCase(1, 2, TestName = "Scale greater than precision")]
		[TestCase(1, 1, TestName = "Scale equals precision")]
		public void Constructor_ThrowsArgumentException(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision, scale);
			act.ShouldThrow<ArgumentException>();
		}

		[TestCase(1, 0, TestName = "Precision is positive")]
		[TestCase(2, 1, TestName = "Scale less than precision and positive")]
		public void Constructor_DoesNotThrow(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision, scale);
			act.ShouldNotThrow();
		}

		[TestCase(1, 0, false, null, TestName = "Value is Null")]
		[TestCase(1, 0, false, "", TestName = "Value is empty")]
		[TestCase(3, 2, false, "a.sd", TestName = "Value is not representation of number")]
		[TestCase(3, 2, false, "00.00", TestName = "Int part and frac part greater than precision")]
		[TestCase(3, 2, true, "+1.23", TestName = "Sign and int part and frac part greater than precision")]
		[TestCase(17, 1, false, "0.00", TestName = "Frac part greater than scale")]
		[TestCase(17, 2, true, "-1.23", TestName = "Only positive and have negative sign")]
		[TestCase(5, 3, false, ".5", TestName = "Without int part")]
		[TestCase(5, 3, false, "+-0.3", TestName = "Two signs")]
		[TestCase(6, 0, false, "five", TestName = "Word representation of number")]
		[TestCase(20, 10, false, "5 + 3", TestName = "Sum of numbers")]
		[TestCase(20, 10, false, "7 - 4", TestName = "Subtract of numbers")]
		[TestCase(5, 2, false, "2.550", TestName = "Number with trailing zeros does not suits scale")]
		[TestCase(5, 2, false, "2.", TestName = "Delimiter without frac part")]
		[TestCase(12, 0, false, "1 000 000", TestName = "Spaces in number representation")]
		[TestCase(12, 5, false, "1,000.25", TestName = "Thousands delimiter")]
		public void IsValidNumber_ReturnFalse(
			int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(17, 2, false, "+1.23", TestName = "Have positive sign")]
		[TestCase(5, 2, false, "-5.43", TestName = "Have negative sign")]
		[TestCase(17, 10, true, "+1.4563", TestName = "Only positive and have positive sign")]
		[TestCase(4, 0, false, "0000", TestName = "Without frac part")]
		[TestCase(17, 2, false, "98765432,1", TestName = "With comma delimiter")]
		[TestCase(5, 4, false, "-0.60", TestName = "Valid number with trailing zeros")]
		public void IsValidNumber_ReturnTrue(
			int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeTrue();
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