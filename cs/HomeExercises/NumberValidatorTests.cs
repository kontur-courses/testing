using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(0, TestName = "precision equal to zero")]
		[TestCase(-1, TestName = "precision less than zero")]
		public void Constructor_WhenInvalidPrecision_ThrowsException(int precision)
		{
			var exceptionMessage = "precision must be a positive number";
			Action action = () => new NumberValidator(precision);
			action
				.Should()
				.Throw<ArgumentException>()
				.WithMessage(exceptionMessage);
		}

		[TestCase(1, -1, TestName = "scale less than zero")]
		[TestCase(1, 1, TestName = "scale equals precision")]
		[TestCase(1, 2, TestName = "scale greater than precision")]
		public void Constructor_WhenInvalidScale_ThrowsException(int precision, int scale)
		{
			var exceptionMessage = "scale must be a non-negative number less than precision";
			Action action = () => new NumberValidator(precision, scale);
			action
				.Should()
				.Throw<ArgumentException>()
				.WithMessage(exceptionMessage);
		}

		[Test]
		public void Constructor_WhenDefaultConstructor_NotThrowsException()
		{
			Action action = () => new NumberValidator(1);
			action
				.Should()
				.NotThrow<ArgumentException>();
		}

		[TestCase(null, TestName = "null value")]
		[TestCase("", TestName = "empty value")]
		public void IsValidNumber_WhenNullOrEmptyNumber_ReturnsFalse(string value)
		{
			new NumberValidator(17, 2)
				.IsValidNumber(value)
				.Should()
				.BeFalse();
		}

		[TestCase(3, 2, "10.00", TestName = "too long positive rational")]
		[TestCase(1, 0, "10", TestName = "too long positive integer")]
		[TestCase(1, 0, "-10", TestName = "too long negative integer")]
		[TestCase(3, 2, "-0.00", TestName = "too long negative rational with sign")]
		[TestCase(3, 2, "+0.00", TestName = "too long positive rational with sign")]
		[TestCase(17, 2, "17.000", TestName = "too long fractional part of positive rational")]
		[TestCase(17, 2, "-17.000", TestName = "too long fractional part of negative rational with sign")]
		[TestCase(17, 2, "+17.000", TestName = "too long fractional part of positive rational with sign")]
		public void IsValidNumber_WhenTooLongIntegerPartOrFractionalPart_ReturnsFalse(int precision, int scale,
			string value)
		{
			new NumberValidator(precision, scale)
				.IsValidNumber(value)
				.Should()
				.BeFalse();
		}

		[TestCase(2, 0, "-1", TestName = "negative integer")]
		[TestCase(3, 1, "-1.1", TestName = "negative rational")]
		public void IsValidNumber_WhenNegativeNumberAsOnlyPositive_False(int precision, int scale, string value)
		{
			new NumberValidator(precision, scale, true)
				.IsValidNumber(value)
				.Should()
				.BeFalse();
		}

		[TestCase(3, 0, "+42", TestName = "positive integer")]
		[TestCase(2, 0, "-1", TestName = "negative integer")]
		[TestCase(2, 0, "42", TestName = "positive integer without sign")]
		[TestCase(4, 2, "+1.23", TestName = "positive rational")]
		[TestCase(3, 1, "-1.1", TestName = "negative rational")]
		[TestCase(2, 1, "0.0", TestName = "positive rational without sign")]
		[TestCase(4, 2, "+1,23", TestName = "positive rational with comma separator")]
		[TestCase(3, 1, "-1,1", TestName = "negative rational with comma separator")]
		[TestCase(2, 1, "0,0", TestName = "positive rational without sign with comma separator")]
		[TestCase(4, 0, "00", TestName = "double zero at integer part")]
		public void IsValidNumber_WhenCorrectNumber_ReturnsTrue(int precision, int scale, string value)
		{
			new NumberValidator(precision, scale)
				.IsValidNumber(value)
				.Should()
				.BeTrue();
		}

		[TestCase("a.sd", TestName = "invalid string")]
		[TestCase("1.a", TestName = "invalid fractional part format")]
		[TestCase("a.1", TestName = "invalid integer part format")]
		[TestCase("1..0", TestName = "invalid separator (double dot)")]
		[TestCase("1.0.0", TestName = "invalid separator")]
		[TestCase("1;0", TestName = "invalid separator char")]
		[TestCase(".0", TestName = "empty integer part")]
		[TestCase("0.", TestName = "empty fractional part")]
		[TestCase(".", TestName = "empty integer and fractional part")]
		[TestCase("++1", TestName = "double positive sign")]
		[TestCase("--1", TestName = "double negative sign")]
		[TestCase("1+", TestName = "positive sign after integer part")]
		[TestCase("1-", TestName = "negative sign after integer part")]
		[TestCase("1.0+", TestName = "positive sign after fractional part")]
		[TestCase("1.+0", TestName = "positive sign after separator")]
		[TestCase("1.0-", TestName = "negative sign after fractional part")]
		[TestCase("1.-0", TestName = "negative sign after separator")]
		public void IsValidNumber_WhenInvalidFormat_ReturnsFalse(string value)
		{
			new NumberValidator(17, 2)
				.IsValidNumber(value)
				.Should()
				.BeFalse();
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
				throw new ArgumentException("scale must be a non-negative number less than precision");

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
			var intPart = match.Groups[1]
				.Value.Length + match.Groups[2]
				.Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4]
				.Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1]
				.Value == "-")
				return false;

			return true;
		}
	}
}