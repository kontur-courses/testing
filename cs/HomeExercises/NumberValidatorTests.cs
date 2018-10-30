using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 0, "precision must be a positive number", TestName = "negative precision")]
		[TestCase(1, -1, "scale must be a non-negative number less or equal than precision", TestName = "negative scale")]
		[TestCase(1, 2, "scale must be a non-negative number less or equal than precision", TestName = "scale greater than precision")]
		public void Constructor_OnInvalidInput_ThrowsException(int precision, int scale, string message)
		{
			Action act = () => new NumberValidator(precision, scale);

			act.ShouldThrow<ArgumentException>()
				.WithMessage(message);
		}

		[TestCase(1, TestName = "positive precision")]
		[TestCase(2, 1, TestName = "custom scale")]
		[TestCase(2, 1, true, TestName = "onlyPositive")]
		public void Constructor_OnValidInput_NotThrowsException(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);

			act.ShouldNotThrow<ArgumentException>();
		}

		[TestCase(17, 2, false, "", TestName = "empty string")]
		[TestCase(17, 2, false, null, TestName = "null")]
		[TestCase(17, 2, false, "0.", TestName = "dot without fractional part")]
		[TestCase(3, 2, false, "+1.23", TestName = "signs count greater than precision")]
		[TestCase(17, 2, false, "0.000", TestName = "frac part signs count greater than scale")]
		[TestCase(3, 2, false, "a.$%", TestName = "not numbers")]
		[TestCase(3, 2, true, "-1.23", TestName = "negative number when onlyPositive flag")]
		[TestCase(4, 2, false, " 1.2 ", TestName = "beginner and trailing whitespaces")]
		[TestCase(4, 2, false, "1.,0", TestName = "too many dividers")]
		[TestCase(4, 2, false, "-+1.0", TestName = "too many signs")]
		public void IsValidNumber_OnInvalidInput_Fails(int precision, int scale, bool onlyPositive, string value)
		{
			var act =  new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);

			act.Should().BeFalse();
		}

		[TestCase(3, 0, false, "0", TestName = "integer")]
		[TestCase(10, 5, false, "12345.67890", TestName = "number which contain all digits")]
		[TestCase(3, 2, false, "-0.5", TestName = "negative number")]
		[TestCase(3, 2, false, "0.5", TestName = "positive number")]
		[TestCase(3, 2, false, "+1.0", TestName = "signed positive number")]
		[TestCase(3, 2, false, "0.0", TestName = "float delimited by point")]
		[TestCase(3, 2, false, "0,0", TestName = "float delimited by comma")]
		[TestCase(3, 2, true, "5.0", TestName = "only positive mode")]
		[TestCase(10, 5, false, "2", TestName = "frac part signs count less than scale")]
		[TestCase(10, 5, false, "4.20", TestName = "signs count less than precision")]
		public void IsValidNumber_OnValidInput_Passes(int precision, int scale, bool onlyPositive, string value)
		{
			var act = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);

			act.Should().BeTrue();
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
				throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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