using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private const string InvalidPrecisionMessage = "precision must be a positive number";

		private const string InvalidScaleMessage = "scale must be a non-negative number less or equal than precision";
		
		[TestCase(-1, 0, InvalidPrecisionMessage, TestName = "Negative precision")]
		[TestCase(0, 0, InvalidPrecisionMessage, TestName = "Zero precision")]
		[TestCase(0, -1, InvalidScaleMessage, TestName = "Negative scale")]
		[TestCase(1, 1, InvalidScaleMessage, TestName = "Scale equals precision")]
		[TestCase(1, 2, InvalidScaleMessage, TestName = "Scale greater then precision")]
		public void ClassConstructor_ThrowArgumentException_OnInvalidParameters(int precision, int scale,
			string message)
		{
			Action constructor = () => new NumberValidator(precision, scale);
			constructor.Should().Throw<ArgumentException>().WithMessage(message);
		}

		[TestCase(2, 1, TestName = "Scale less then precision")]
		[TestCase(1, 0, TestName = "Zero scale")]
		[TestCase(1, 0, true, TestName = "Only positive")]
		[TestCase(1, 0, false, TestName = "Not only positive")]
		[TestCase(int.MaxValue, 0, TestName = "Max value precision")]
		[TestCase(int.MaxValue, int.MaxValue - 1, TestName = "Max value scale")]
		public void ClassConstructor_NotThrowArgumentException_OnValidParameters(int precision,
			int scale, bool onlyPositive = false)
		{
			Action constructor = () => new NumberValidator(precision, scale, onlyPositive);
			constructor.Should().NotThrow<ArgumentException>();
		}

		[TestCase(null, TestName = "Null string")]
		[TestCase("", TestName = "Empty string")]
		[TestCase(" ", TestName = "Space string")]
		[TestCase("lette.rs", TestName = "String with letters")]
		[TestCase("+", TestName = "Only plus sign")]
		[TestCase("-", TestName = "Only minus sign")]
		[TestCase("0.", TestName = "With dot but without fractional part")]
		[TestCase("0,", TestName = "With comma but without fractional part")]
		[TestCase(".0", TestName = "With dot but without integer part")]
		[TestCase(",0", TestName = "With comma but without integer part")]
		public void IsValidNumber_ReturnFalse_OnNotNumber(string number)
		{
			new NumberValidator(10, 2).IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("000", 2, 0, TestName = "Too long number without fractional part")]
		[TestCase("0.000", 3, 1, TestName = "Too long number without sign")]
		[TestCase("+0.00", 3, 2, TestName = "Too long number with plus sign")]
		[TestCase("-0.00", 3, 2, TestName = "Too long number with minus sign")]
		[TestCase("0.00", 3, 1, TestName = "Too long fractional part")]
		[TestCase("0.00", 3, 0, TestName = "Fractional part and integer validator")]
		[TestCase("-0.00", 4, 2, true, TestName = "Negative Number and only positive validator")]
		public void IsValidNumber_ReturnFalse__OnIncorrectParameters(
			string number, int precision, int scale, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("0", 1, 0, TestName = "Zero")]
		[TestCase("1", 1, 0, TestName = "Integer number")]
		[TestCase("123", 3, 2, TestName = "Integer number and not integer validator")]
		[TestCase("1", 10, 2, TestName = "Precision greater then number length")]
		[TestCase("1.23", 3, 2, TestName = "Fractional number with dot")]
		[TestCase("1,23", 3, 2, TestName = "Fractional number with comma")]
		[TestCase("1.23", 11, 10, TestName = "Fractional number and big scale")]
		[TestCase("+1.23", 4, 2, TestName = "Positive number with sign")]
		[TestCase("-1.23", 4, 2, TestName = "Negative number with sign and not only positive validator")]
		[TestCase("+1.23", 4, 2, true, TestName = "Positive number with sign and only positive validator")]
		public void IsValidNumber_ReturnTrue_OnCorrectParameters(
			string number, int precision, int scale, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeTrue();
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