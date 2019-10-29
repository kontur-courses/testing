using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, TestName = "Precision negative")]
		[TestCase(0, 2, TestName = "Precision zero")]
		[TestCase(1, -1, TestName = "Scale negative")]
		[TestCase(1, 2, TestName = "Scale greater than precision")]
		[TestCase(-2, -1, TestName = "Scale greater than precision and precision negative and scale negative")]
		public void Constructor_WithArgumentException(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.ShouldThrow<ArgumentException>();
		}
		
		[TestCase("0.00", true, TestName = "Zero with scale only positive")]
		[TestCase("0.00", false, TestName = "Zero with scale")]
		[TestCase("0", true, TestName = "Zero only positive")]
		[TestCase("0", false, TestName = "Zero")]
		[TestCase("+0", true, TestName = "Zero positive with only positive")]
		[TestCase("-0", false, TestName = "Zero negative")]
		[TestCase("1111", false, TestName = "Without scale")]
		[TestCase("1111.12", false, TestName = "Number positive with scale")]
		[TestCase("+1111.12", true, TestName = "Number positive with scale only positive")]
		[TestCase("-1111", false, TestName = "Number negative without scale")]
		public void IsValidNumber_ReturnTrue(string value, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(17, 4, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeTrue();
		}
		
		
		[TestCase(null, false, TestName = "Value is null")]
		[TestCase("", false, TestName = "Value is empty")]
		[TestCase("+ad", false, TestName = "Value isn't number")]
		[TestCase(" ", false, TestName = "Value is spaсe")]
		[TestCase("11111", false, TestName = "Precision more than set")]
		[TestCase("+1111", false, TestName = "Sign and number have precision more than set")]
		[TestCase("-11.1", true, TestName = "Number negative with only positive")]
		[TestCase("-0", true, TestName = "Zero negative with only positive")]
		[TestCase("1.111", false, TestName = "Scale more than set")]
		public void IsValidNumber_ReturnFalse(string value, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(4, 2, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
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