using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private NumberValidator validator;

		[OneTimeSetUp]
		public void Initialization()
		{
			validator = new NumberValidator(5, 2, true);
		}
		
		[TestCase(2, 1, true, TestName = "Scale less than precision")]
		[TestCase(1, 0, true, TestName = "Scale is zero")]
		public void Constructor_ShouldCreateNumberValidator(int precision, int scale, bool onlyPositive)
		{
			FluentActions.Invoking(
					() => new NumberValidator(precision, scale, onlyPositive))
				.Should().NotThrow();
		}

		[TestCase(-1, 2, true, TestName = "Negative precision")]
		[TestCase(1, -1, true, TestName = "Negative scale")]
		[TestCase(1, 2, true, TestName = "Scale greater than precision")]
		[TestCase(0, 1, true, TestName = "Precision is zero")]
		public void Constructor_ShouldThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			FluentActions.Invoking(
					() => new NumberValidator(precision, scale, onlyPositive))
				.Should().Throw<ArgumentException>();;
		}

		[TestCase("", TestName = "Value is empty string")]
		[TestCase(" ", TestName = "Value is white space")]
		[TestCase(null, TestName = "Value is null")]
		[TestCase("a.sd", TestName = "Value is not a number")]
		[TestCase("", TestName = "Value is not a number")]
		[TestCase("-0.0", TestName = "NumberValidator is onlyPositive but value is negative")]
		[TestCase("0.000", TestName = "Fractional part grater then scale")]
		[TestCase(".0", TestName = "Value without int part")]
		[TestCase("0.", TestName = "Value without fractional part but with separator")]
		[TestCase("+000.00", TestName = "Int part plus fractional part greater than precision")]
		public void IsValid_ShouldBeFalse(string value)
		{
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(true, "0,0", TestName = "Separator is comma")]
		[TestCase(true,"+0.0", TestName = "Value is positive")]
		[TestCase(false, "-0.0", TestName = "Value is negative")]
		public void IsValid_ShouldBeTrue(bool onlyPositive, string value)
		{
			new NumberValidator(3,2, onlyPositive).IsValidNumber(value).Should().BeTrue();
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