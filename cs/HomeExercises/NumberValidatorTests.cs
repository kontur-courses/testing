using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(7, 4, "000.0000", TestName = "Simple value")]
		[TestCase(7, 4, "000,0000", TestName = "Check comma instead dot")]
		[TestCase(1, 0, "1", TestName = "without fracPart")]
		[TestCase(4, 1, "1.1", TestName = "number < precision")]
		[TestCase(5, 4, "000.00", TestName = "fracPart < scale")]
		[TestCase(6, 2, "+000.00", TestName = "Number with sign+")]
		[TestCase(6, 2, "-000.00", TestName = "Number with sign-")]
		public void IsValidNumber_ReturnTrue_WhenCorrectArgsAndValue(int precision, int scale, string? value)
		{
			new NumberValidator(precision, scale).IsValidNumber(value!).Should().BeTrue();
		}

		[TestCase(1, 0, null, TestName = "Nullable number")]
		[TestCase(1, 0, "", TestName = "Empty number")]
		[TestCase(1, 0, "        ", TestName = "Number is whitespaces")]
		[TestCase(2, 0, " 1", TestName = "Whitespaces before number")]
		[TestCase(2, 0, "1 ", TestName = "Whitespaces after number")]
		[TestCase(2, 0, "/1", TestName = "Incorrect sign")]
		[TestCase(2, 0, "1+", TestName = "Correct sign in incorrect place")]
		[TestCase(5, 0, "12r34", TestName = "Incorrect char in number")]
		[TestCase(5, 0, "1..1", TestName = "Many dots")]
		[TestCase(5, 0, "+-1", TestName = "Many signs")]
		[TestCase(5, 0, "+", TestName = "Only sign")]
		[TestCase(2, 1, ".0", TestName = "Number without intPart")]
		[TestCase(2, 1, "0.", TestName = "Number wtih dot, but without fracPart")]
		[TestCase(2, 0, "111", TestName = "intPart > precision")]
		[TestCase(2, 1, "11.1", TestName = "Number > precision")]
		[TestCase(2, 1, "-1.1", TestName = "Number + sign > precision")]
		[TestCase(3, 1, "2.22", TestName = "fracPart > scale")]
		[TestCase(3, 1, "-1.1", true, TestName = "Negative number than OnlyPositive = true")]
		public void IsValidNumber_ReturnFalse_WhenCorrectArgsAndIncorrectValue(int precision, int scale, string? value, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value!).Should().BeFalse();
		}

		[TestCase(-1, TestName = "precision can't be negative")]
		[TestCase(0, TestName = "precision can't be zero")]
		[TestCase(1, 1, TestName = "precision can't be equal to scale")]
		[TestCase(2, -1, TestName = "scale can't be negative")]
		[TestCase(2, 3, TestName = "scale can't be bigger than precision")]
		public void Constructor_ThrowsException_WhenIncorrectArgs(int precision, int scale = 0)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}

		[TestCase(1, TestName = "positive precision")]
		[TestCase(1, 0, TestName = "scale = 0")]
		[TestCase(2, 1, TestName = "scale < precision")]
		public void Constructor_DoesNotThrowsException_WhenCorrectArgs(int precision, int scale = 0)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().NotThrow<ArgumentException>();
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