using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1, 0,"0", TestName = "int zero")]
		[TestCase(2, 0,"+0", TestName = "positive int zero")]
		[TestCase(2, 0,"-0", false , TestName = "negative int zero")]
		[TestCase(2, 1,"0.0", TestName = "zero with fractional part")]
		[TestCase(3, 2,"1.23", TestName = "positive non zero without sign with fractional part")]
		[TestCase(4, 2,"+1.23", TestName = "positive non zero with sign and fractional part")]
		[TestCase(4, 2,"-1.23", false, TestName = "negative non zero with sign and fractional part")]
		public void IsValid(int precision, int scale, string validatingString, bool onlyPositive = true)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(validatingString)
				.Should()
				.BeTrue();
		}
		
		[TestCase(2, 1,"0.00", TestName = "zero intPart + fracPart should be less than precesion")]
		[TestCase(3, 2, "+0.00", TestName = "zero intPart + fracPart + \"+\" should be less than precesion")]
		[TestCase(3, 2, "-0.00", false, TestName = "zero intPart + fracPart + \"-\" should be less than precesion")]
		[TestCase(3, 2,"+1.23", TestName = "positive non zero intPart + fracPart + \"+\" should be less than precesion")]
		[TestCase(3, 2,"-1.23", false, TestName = "negative non zero intPart + fracPart + \"+\" should be less than precesion")]
		[TestCase(3, 2, "a.sd", TestName = "non digit symbols")]
		[TestCase(2, 1, ".0", TestName = "must have digits before point")]
		[TestCase(1, 0, "0.", TestName = "must have digits after point (if exist)")]
		public void IsNotValid(int precision, int scale, string validatingString, bool onlyPositive = true)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(validatingString)
				.Should()
				.BeFalse();
		}

		[TestCase(-1, 1, TestName = "negative precision")]
		[TestCase(1, -1, TestName = "negative scale")]
		[TestCase(-1, -1, TestName = "negative precision and scale")]
		[TestCase(1, 1, TestName = "precision equals scale")]
		[TestCase(1, 2, TestName = "precision less than scale")]
		public void ShouldThrow(int precision, int scale, bool onlyPositive = true)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().Throw<ArgumentException>();
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