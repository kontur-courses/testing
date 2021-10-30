using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1, 0, TestName = "Scale is zero")]
		[TestCase(3, 2, TestName = "Precision and scale are positive")]
		public void Should_InitsCorrectly_When(int precision, int scale)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale));
		}

		[TestCase(-1, 2, TestName = "Negative precision")]
		[TestCase(0, 2, TestName = "Zero precision")]
		[TestCase(1, -2, TestName = "Negative scale")]
		[TestCase(1, 2, TestName = "Scale greater than precision")]
		[TestCase(1, 1, TestName = "Scale equals to precision")]
		public void Should_ThrowArgumentException_OnIncorrectInput(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
		}

		[TestCase(10, 2, "", TestName = "Empty string")]
		[TestCase(10, 2, "       ", TestName = "Whitespace string")]
		[TestCase(10, 2, "-", TestName = "Only sign")]
		[TestCase(10, 2, null, TestName = "Null")]
		[TestCase(10, 2, "abc", TestName = "Not a number")]
		[TestCase(10, 2, "a.bc", TestName = "Not a number with dot")]
		[TestCase(17, 2, "0.000", TestName = "FracPart greater than scale")]
		[TestCase(2, 1, "120.0", TestName = "IntPart greater than precision")]
		[TestCase(3, 2, "-.22", TestName = "Sign instead of intPart")]
		[TestCase(2, 1, "-0.0", TestName = "Minus sign with wrong precision")]
		public void IsValidNumber_Should_ReturnFalse_When(int precision, int scale, string expression)
		{
			var validator = new NumberValidator(precision, scale);

			var validationResult = validator.IsValidNumber(expression);

			validationResult.Should().BeFalse();
		}

		[TestCase(17, 2, "0.0", TestName = "Standard")]
		[TestCase(3, 1, "120", TestName = "No fracPart")]
		[TestCase(3, 1, "01.2", TestName = "Leading zero")]
		[TestCase(17, 3, "-0.0", TestName = "Minus sign")]
		public void IsValidNumber_Should_ReturnTrue_When(int precision, int scale, string expression)
		{
			var validator = new NumberValidator(precision, scale);

			var validationResult = validator.IsValidNumber(expression);

			validationResult.Should().BeTrue();
		}

		[TestCase(4, 1, "1.2", true, TestName = "Without sign")]
		[TestCase(4, 1, "+01.2", true, TestName = "Plus sign with right precision")]
		[TestCase(3, 1, "+01.2", false, TestName = "Plus sign with wrong precision")]
		[TestCase(4, 1, "-01.2", false, TestName = "Minus sign")]
		public void IsValidNumber_Should_WorkCorrectly_WithOnlyPositiveNumbers(int precision, int scale,
			string expression, bool expectedResult)
		{
			var validator = new NumberValidator(precision, scale, true);

			var validationResult = validator.IsValidNumber(expression);

			validationResult.Should().Be(expectedResult);
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