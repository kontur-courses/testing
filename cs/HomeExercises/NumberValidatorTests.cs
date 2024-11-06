using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Primitives;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(2, 3, true)]
		[TestCase(-1, 2, false)]
		public void Constructor_IncorrectArguments_ShouldThrowArgumentException(int precision, int scale,
			bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, true)]
		public void Constructor_CorrectArguments_ShouldNotThrowArgumentException(int precision, int scale,
			bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().NotThrow();
		}

		[TestCase(3, 2, true, "a.sd")]
		[TestCase(3, 2, true, "0+1.0.1")]
		public void IsValidNumber_WrongInputType_ShouldReturnFalse(int precision, int scale, bool onlyPositive,
			string input)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(input).Should().BeFalse();
		}
		
		[TestCase(3, 2, true, "00.00")]
		public void IsValidNumber_WrongPrecision_ShouldReturnFalse(int precision, int scale, bool onlyPositive,
			string input)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(input).Should().BeFalse();
		}
		
		[TestCase(17, 1, true, "00.00")]
		public void IsValidNumber_WrongScale_ShouldReturnFalse(int precision, int scale, bool onlyPositive,
			string input)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(input).Should().BeFalse();
		}
		
		[TestCase(4, 3, true, " ")]
		public void IsValidNumber_NullOrWhitespaces_ShouldReturnFalse(int precision, int scale, bool onlyPositive,
			string input)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(input).Should().BeFalse();
		}
		
		[TestCase(1, 0, false, "-0")]
		public void IsValidNumber_NegativeNumberWhenNotAllowed_ShouldReturnFalse(int precision, int scale, bool onlyPositive,
			string input)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(input).Should().BeFalse();
		}
		
		[TestCase(17, 2, true, "0")]
		public void IsValidNumber_CorrectInput_ShouldReturnTrue(int precision, int scale, bool onlyPositive,
			string input)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(input).Should().BeTrue();
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