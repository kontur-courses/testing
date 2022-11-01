using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(2, TestName = "precision is positive")]
		[TestCase(2, 0, TestName = "scale is zero")]
		[TestCase(2, 1, TestName = "scale is positive and less then precision")]
		[TestCase(2, 1, true, TestName = "onlyPositive is true")]
		[TestCase(2, 1, false, TestName = "onlyPositive is false")]
		public void Constructor_OnValidParameters_DoesNotThrowException(int precision, int scale = 0, bool onlyPositive = true)
		{
			Action creation = () => new NumberValidator(precision, scale, onlyPositive);
			creation.Should().NotThrow();
		}

		[TestCase(-1, TestName = "precision is negative")]
		[TestCase(0, TestName = "precision is zero")]
		[TestCase(1, -1, TestName = "scale is negative")]
		[TestCase(1, 1, TestName = "scale is equal to precision")]
		[TestCase(1, 2, TestName = "scale is greater then precision")]
		public void Constructor_OnInvalidParameters_ThrowsArgumentException(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action creation = () => new NumberValidator(precision, scale, onlyPositive);
			creation.Should().Throw<ArgumentException>();
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