using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true)]
		[TestCase(0, 0, true)]
		public void Construct_WithIncorrectPrecision_Expected_ArgumentException(int precision, int scale,
			bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);

			action.Should().Throw<ArgumentException>().WithMessage("precision must be a positive number");
		}

		[TestCase(1, -2, true)]
		[TestCase(1, 2, true)]
		[TestCase(1, 1, true)]
		public void Construct_WithIncorrectScale_Expected_ArgumentException(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);

			action.Should().Throw<ArgumentException>()
				.WithMessage("scale must be a non-negative number less or equal than precision");
		}

		[TestCase(1, 0, true)]
		[TestCase(1, 0, false)]
		[TestCase(2, 1, true)]
		[TestCase(2, 1, false)]
		public void Construct_WithCorrectArguments_Expected_NoExceptions(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);

			action.Should().NotThrow<Exception>();
		}

		[TestCase(1, 0, true, "1", ExpectedResult = true, TestName = "Integer whithout sign")]
		[TestCase(2, 0, true, "+1", ExpectedResult = true, TestName = "Positive integer")]
		[TestCase(2, 0, false, "-1", ExpectedResult = true, TestName = "Negative integer")]
		[TestCase(2, 1, true, "1.1", ExpectedResult = true, TestName = "Decimal whithout sign")]
		[TestCase(3, 1, true, "+1.1", ExpectedResult = true, TestName = "Positive decimal")]
		[TestCase(3, 1, false, "-1.1", ExpectedResult = true, TestName = "Negative decimal")]
		[TestCase(2, 1, true, "1,1", ExpectedResult = true, TestName = "Decimal with comma as separator")]
		public bool IsValidNumber_Should_ValidateValue(int precision, int scale, bool onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			return validator.IsValidNumber(value);
		}

		[TestCase(3, 1, true, "1.11", ExpectedResult = false, TestName = "Scale less than fractional part")]
		[TestCase(3, 2, true, "11.11", ExpectedResult = false, TestName = "Precision less than digits")]
		[TestCase(3, 2, true, "+1.11", ExpectedResult = false, TestName = "Incorrect precision, sign didn't fit")]
		[TestCase(2, 0, true, "-1", ExpectedResult = false, TestName = "Negative number with onlyPositive == true")]
		[TestCase(2, 0, true, "+1.", ExpectedResult = false, TestName = "Decimal without fractional part")]
		[TestCase(3, 0, true, "1..1", ExpectedResult = false, TestName = "Decimal with same double separator")]
		[TestCase(3, 0, true, "1,.1", ExpectedResult = false, TestName = "Decimal with different double separator")]
		[TestCase(3, 2, true, "a.sd", ExpectedResult = false, TestName = "Non digital Decimal")]
		[TestCase(3, 0, true, "asd", ExpectedResult = false, TestName = "Non digital Integer")]
		[TestCase(4, 0, false, "-asd", ExpectedResult = false, TestName = "Non digital integer with sign")]
		[TestCase(2, 1, true, ".1", ExpectedResult = false, TestName = "Decimal without integer")]
		[TestCase(1, 0, true, "+.", ExpectedResult = false, TestName = "Only sign and separator in input")]
		public bool IsValidNumber_Should_NotValidateValue(int precision, int scale, bool onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			return validator.IsValidNumber(value);
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