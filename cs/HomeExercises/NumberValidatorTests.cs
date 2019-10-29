using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[Test]
		[TestCase(17, 2, true, "0.0", ExpectedResult = true,
			Description = "unsigned valid number with fractional part")]
		[TestCase(17, 2, true, "0", ExpectedResult = true,
			Description = "unsigned valid number without fractional part")]
		[TestCase(4, 2, true, "+1.23", ExpectedResult = true,
			Description = "signed valid number")]
		[TestCase(17, 2, true, "0.000", ExpectedResult = false,
			Description = "fractional part more than possible")]
		[TestCase(3, 2, true, "00.00", ExpectedResult = false,
			Description = "integer part is not valid number")]
		[TestCase(3, 2, true, "+1.23", ExpectedResult = false,
			Description = "positive signed number is longer than possible")]
		[TestCase(3, 2, true, "-1.23", ExpectedResult = false,
			Description = "only a positive number is possible")]
		[TestCase(3, 2, true, "a.sd", ExpectedResult = false,
			Description = "invalid characters in number")]
		public bool IsValidNumber(int precision, int scale, bool onlyPositive, string number)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number);
		}


		/// <summary>
		/// the body of the test should be divided into blocks:
		/// Arrange, Act and Assert, but in these cases it is not principle important
		/// </summary>
		[Test]
		public void CreateNumberValidator_WithNegativePrecision_ShouldThrowException()
		{
			Action act = () => new NumberValidator(-1, 2, true);
			act.Should().Throw<ArgumentException>();
		}

		[Test]
		public void CreateNumberValidator_WithValidArguments_ShouldNotThrowException()
		{
			Action act = () => new NumberValidator(1, 0, true);
			act.Should().NotThrow();
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