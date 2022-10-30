using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(17, 2, true, "0.000", Description = "number scale greater than specified scale")]
		[TestCase(17, 2, true, "0000000000000000000",
			Description = "count of digits in number greater than specified precision")]
		[TestCase(17, 2, true, "-1", Description = "number is negative when accepted only positive numbers")]
		[TestCase(17, 2, true, "одна целая и пять десятых", Description = "number is not match regular number format")]
		public void IsValidNumber_ShouldReturnsFalse_WhenCallingWithTheseParameters(int precision, int scale,
			bool onlyPositive, string number)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeFalse();
		}


		[TestCase(17, 2, true, "0.00")]
		[TestCase(17, 2, true, "0")]
		[TestCase(17, 2, true, "00000000000000000")]
		[TestCase(17, 2, true, "0,01")]
		[TestCase(4, 2, true, "+1.23")]
		[TestCase(4, 2, false, "-1.23")]
		public void IsValidNumber_ShouldReturnsTrue_WhenCallingWithTheseParameters(int precision, int scale,
			bool onlyPositive, string number)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeTrue();
		}

		[Test]
		public void ShouldThrows_WhenCreatingWithScaleEqualToPrecision()
		{
			Action action = () => _ = new NumberValidator(1, 1);
			const string expectedMessage = "precision must be a non-negative number less or equal than precision";
			action.Should().ThrowExactly<ArgumentException>()
				.WithMessage(expectedMessage);
		}

		[Test]
		public void ShouldThrows_WhenCreatingWithNegativeScale()
		{
			Action action = () => _ = new NumberValidator(1, -1);
			const string expectedMessage = "precision must be a non-negative number less or equal than precision";
			action.Should().ThrowExactly<ArgumentException>()
				.WithMessage(expectedMessage);
		}

		[Test]
		public void ShouldThrows_WhenCreatingWithZeroPrecision()
		{
			Action action = () => _ = new NumberValidator(0);
			const string expectedMessage = "precision must be a positive number";
			action.Should().ThrowExactly<ArgumentException>()
				.WithMessage(expectedMessage);
		}

		[Test]
		public void ShouldThrows_WhenCreatingWithNegativePrecision()
		{
			Action action = () => _ = new NumberValidator(-1);
			const string expectedMessage = "precision must be a positive number";
			action.Should().ThrowExactly<ArgumentException>()
				.WithMessage(expectedMessage);
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