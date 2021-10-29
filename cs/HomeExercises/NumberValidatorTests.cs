using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidator_Should
	{
		[TestCase(-1, 0, true, TestName = "precision is less than zero")]
		[TestCase(0, 0, true, TestName = "precision equals zero")]
		[TestCase(1, -1, true, TestName = "scale is less than zero")]
		[TestCase(1, 1, true, TestName = "scale equals precision")]
		[TestCase(1, 2, true, TestName = "scale is greater than precision")]
		public void ThrowArgumentExceptionWhen(int precision, int scale, bool onlyPositive)
		{
			Action act = () => { new NumberValidator(0, 0, true); };

			act.Should().Throw<ArgumentException>();
		}

		[TestCase(5, 4, true, "a.sd", TestName = "value contains letters")]
		[TestCase(5, 4, true, "1$.0d", TestName = "value contains random symbols")]
		[TestCase(5, 4, true, ".00", TestName = "integer is missed but separator is not")]
		[TestCase(5, 4, true, "1.", TestName = "fraction is missed but separator is not")]
		[TestCase(5, 4, true, "", TestName = "value is empty")]
		[TestCase(5, 4, true, null, TestName = "value is null")]
		[TestCase(5, 4, true, "11111.0000", TestName = "value length is bigger than precision")]
		[TestCase(5, 2, true, "1.0000", TestName = "fraction length is bigger than scale")]
		[TestCase(5, 4, true, "-1.00", TestName = "value should be positive as onlyPositive parameter is true")]
		[TestCase(5, 0, true, "1.00", TestName = "fraction length should be equal zero")]
		public void FailWhen(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, true);

			var validationResult = numberValidator.IsValidNumber(value);

			validationResult.Should().BeFalse();
		}

		[TestCase(3, 2, true, "1.0", TestName = "value length equals precision")]
		[TestCase(3, 2, true, "1.00", TestName = "fractional part of value equal scale")]
		[TestCase(15, 14, true, "3.14159265", TestName = "fractional part of value less than scale")]
		[TestCase(17, 2, true, "111.00", TestName = "value length less than precision")]
		[TestCase(17, 2, true, "111", TestName = "there is integer part of value only")]
		[TestCase(3, 2, true, "+1.00", TestName = "precision doesn't consider the plus")]
		[TestCase(4, 2, false, "-1.00", TestName = "value contains the minus")]
		public void PassWhen(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			var validationResult = numberValidator.IsValidNumber(value);

			validationResult.Should().BeTrue();
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
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном
			// виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к),
			// где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки,
			// k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое),
			// то формат числового значения имеет вид N(m).

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