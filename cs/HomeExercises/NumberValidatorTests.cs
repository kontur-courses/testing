using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1, 0, TestName = "Positive precision, scale equal zero")]
		[TestCase(2, 1, TestName = "Positive precision, scale less than precision")]
		public void NumberValidatorConstructor_DoesntThrowException_OnCorrectParams(int precision, int scale = 0)
        {
			Action action = () => new NumberValidator(precision, scale);
			action.Should().NotThrow<ArgumentException>();
		}

		[TestCase(0, TestName = "Precision equal zero")]
		[TestCase(-10, TestName = "Negative precision")]
		[TestCase(10, 10, TestName = "Scale equal precision")]
		[TestCase(10, 11, TestName = "Scale more than precision")]
		[TestCase(10, -10, TestName = "Negative scale")]
		public void NumberValidatorConstructor_ThrowException_OnIncorrectParams(int precision, int scale = 0)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}

		[TestCase("1. 2", TestName = "Value contains space")]
		[TestCase("abcd", TestName = "Only letters")]
		[TestCase("1.2abcd", TestName = "Number and letters")]
		[TestCase("*/", TestName = "Only special symbols")]
		[TestCase("1.2*/", TestName = "Number and special symbols")]
		[TestCase("\t\n", TestName = "Only escape-subsequence")]
		[TestCase("1.2\t\n", TestName = "Number and escape-subsequence")]
		[TestCase("1..2", TestName = "Number contains two dots and a fractional part")]
		[TestCase("1..", TestName = "Number contains two dots and no fractional part")]
		[TestCase("1,,2", TestName = "Number contains two comma and a fractional part")]
		[TestCase("1,,", TestName = "Number contains two comma and no fractional part")]
		[TestCase(".2", TestName = "No integer part befor dot")]
		[TestCase(",2", TestName = "No integer part befor comma")]
		[TestCase("--12", TestName = "Two minus signs")]
		[TestCase("++12", TestName = "Two plus signs")]
		public void IsValidNumber_False_OnIncorrectValueFormat(string value)
		{
			new NumberValidator(10, 2).IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("100", 2, 1, TestName = "Length of integer part more than precision, and no sign")]
		[TestCase("+10", 2, 1, TestName = "Length of integer part with sign plus more than precision")]
		[TestCase("-10", 2, 1, TestName = "Length of integer part with sign minus more than precision")]
		[TestCase("1.2", 1, 0, TestName = "Length of integer part and fractional part more than precision, and no sign")]
		[TestCase("+1.2", 2, 1, TestName = "Length of integer part with sign plus and fractional part more than precision")]
		[TestCase("-1.2", 2, 1, TestName = "Length of integer part with sign minus and fractional part more than precision")]
		[TestCase("1.2222", 10, 3, TestName = "Length of fractional part more than scale")]
		public void IsValidNumber_False_InvalidValue(string value, int precision, int scale)
		{
			new NumberValidator(precision, scale).IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("-10", TestName = "Negative value and onlyPositive is true")]
		public void IsValidNumber_False_OnNegativeValue_WhenOnlyPositiveValueIsExpected(string value, bool onlyPositive = true)
		{
			new NumberValidator(5, 1, onlyPositive).IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("10.1", true, TestName = "Positive unsigned number and onlyPositive is true")]
		[TestCase("10.1", TestName = "Positive unsigned number and onlyPositive is false")]
		[TestCase("+10.1", true, TestName = "Positive number with plus sign and onlyPositive is true")]
		[TestCase("+10.1", TestName = "Positive number with plus sign and onlyPositive is false")]
		[TestCase("-10.1", TestName = "Nigative number and onlyPositive is false")]
		public void IsValidNumber_True_OnCorrectParams(string value, bool onlyPositive = false)
		{
			var validator = new NumberValidator(5, 1, onlyPositive);
			validator.IsValidNumber(value).Should().BeTrue();
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
				// В месседже была опечатка. Заменил "precision" на "scale", иначе утверждение имеет мало смысла
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