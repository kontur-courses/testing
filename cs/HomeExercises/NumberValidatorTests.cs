using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{

		[TestCase(-1, TestName = "Pprecision < 0")]
		[TestCase(0, TestName = "Precision == 0")]
		public void Construct_WithIncorrectPrecision_ShouldThrow_ArgumentException(int precision)
		{
			Action action = () => new NumberValidator(precision);

			action.Should().Throw<ArgumentException>()
				.WithMessage("precision must be a positive number");
		}

		[TestCase(1, -2, TestName = "Scale < 0")]
		[TestCase(1, 2, TestName = "Scale > precision")]
		[TestCase(1, 1, TestName = "Scale == precision")]
		public void Construct_WithIncorrectScale_ShouldThrow_ArgumentException(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);

			action.Should().Throw<ArgumentException>()
				.WithMessage("scale must be a non-negative number less or equal than precision");
		}

		[TestCase(1, 0, TestName = "Correct precision")]
		[TestCase(2, 1, TestName = "Scale < precision and not negative")]
		public void Construct_WithCorrectArguments_ShouldNotThrow_Exceptions(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);

			action.Should().NotThrow<Exception>();
		}

		[Test]
		public void Construct_WithBothDefaultParameters_ShouldNotThrow_Exceptions()
		{
			Action action = () => new NumberValidator(1);

			action.Should().NotThrow<Exception>();
		}

		[TestCase(1, "1", ExpectedResult = true, TestName = "Precision equals integer length")]
		[TestCase(2, "1", ExpectedResult = true, TestName = "Precision more than digits")]
		[TestCase(1, "11", ExpectedResult = false, TestName = "Integer longer than precision")]
		public bool IsValidNumber_AddingInteger(int precision, string value)
		{
			var validator = new NumberValidator(precision);

			return validator.IsValidNumber(value);
		}

		[TestCase(3, 2, "1.11", ExpectedResult = true, TestName = "Exact scale and precision")]
		[TestCase(5, 2, "1.11", ExpectedResult = true, TestName = "Exact scale and precision more than digits")]
		[TestCase(3, 1, "1.11", ExpectedResult = false, TestName = "Scale less than fractional part")]
		[TestCase(3, 2, "11.11", ExpectedResult = false, TestName = "Precision less than total digits")]
		[TestCase(2, 1, "1,1", ExpectedResult = true, TestName = "Comma as separator")]
		public bool IsValidNumber_AddingDecimal(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale);

			return validator.IsValidNumber(value);
		}

		[TestCase(3, 2, true, "+1.11", ExpectedResult = false, TestName = "Incorrect precision, sign didn't fit")]
		[TestCase(2, 0, true, "+1", ExpectedResult = true, TestName = "Positive with sign")]
		[TestCase(2, 0, false, "-1", ExpectedResult = true, TestName = "Negative integer")]
		[TestCase(3, 1, false, "-1.1", ExpectedResult = true, TestName = "Negative decimal")]
		[TestCase(2, 0, true, "-1", ExpectedResult = false, TestName = "Negative number with onlyPositive == true")]
		public bool IsValidNumber_AddingNumbersWithSign(int precision, int scale, bool onlyPositive,
			string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			return validator.IsValidNumber(value);
		}

		[TestCase(2, 0, true, "+1.", TestName = "Decimal without fractional part")]
		[TestCase(3, 0, true, "1..1", TestName = "Decimal with same double separator")]
		[TestCase(3, 0, true, "1,.1", TestName = "Decimal with different double separators")]
		[TestCase(3, 2, true, "a.sd", TestName = "Non digital Decimal")]
		[TestCase(3, 2, true, "1.sd", TestName = "Non digital fractional part")]
        [TestCase(3, 0, true, "asd", TestName = "Non digital Integer")]
		[TestCase(4, 0, false, "-asd", TestName = "Non digital integer with sign")]
		[TestCase(2, 1, true, ".1", TestName = "Decimal without integer")]
		[TestCase(1, 0, true, "+.", TestName = "Only sign and separator in input")]
		[TestCase(2, 0, true, "+-", TestName = "Two signs")]
		[TestCase(2, 0, true, "..", TestName = "Two separators")]
		[TestCase(1, 0, true, "", TestName = "Empty string")]
		[TestCase(3, 0, true, "++1", TestName = "Two signs with number")]
		[TestCase(1, 0, true, "+", TestName = "Only sign")]
		[TestCase(1, 0, true, ".", TestName = "Only divider")]
		[TestCase(4, 3, true, ".1+3", TestName = "Misplaced sign and divider")]
		public void IsValidNumber_AddingNonNumericValues_Should_ReturnFalse(int precision, int scale, bool onlyPositive,
			string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			var result = validator.IsValidNumber(value);

			result.Should().BeFalse();
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