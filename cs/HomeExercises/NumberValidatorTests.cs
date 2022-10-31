using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidator_PrecisionLessThanOrEqualToZero_ThrowException()
		{
			Action action = () => new NumberValidator(-1, 2, true);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_NegativeScale_ThrowException()
		{
			Action action = () => new NumberValidator(1, -2, true);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_ScaleGreaterThanOrEqualToPrecision_ThrowException()
		{
			Action action = () => new NumberValidator(1, 2, true);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_WithValidParameters_DoesNotThrowException()
		{
			Action action = () => new NumberValidator(1, 0, true);
			action.Should().NotThrow();
		}

		[TestCase(null)]
		[TestCase("")]
		public void IsValidNumber_NullOrEmptyValue_ReturnFalse(string value)
		{
			var numberValidator = new NumberValidator(1, 0, true);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCase(3, 2, true, "1..23")]
		[TestCase(3, 2, true, "1,.23")]
		[TestCase(3, 2, true, "1,ab")]
		[TestCase(3, 2, true, "a.23")]
		[TestCase(3, 2, true, "a.bc")]
		[TestCase(5, 2, true, "++1.23")]
		[TestCase(5, 2, false, "-+1.23")]
		[TestCase(5, 3, false, "-a,bc.1")]
		public void IsValidNumber_IncorrectValueFormat_ReturnFalse(
			int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(3, 2, true, "00.00")]
		[TestCase(3, 2, true, "+1.23")]
		[TestCase(3, 2, false, "-1.23")]
		public void IsValidNumber_SignAndIntegerPartGreaterThanPrecision_ReturnFalse(
			int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCase(4, 2, true, "0.000")]
		[TestCase(4, 2, true, "1.234")]
		[TestCase(5, 2, true, "00.000")]
		[TestCase(5, 2, true, "+1.234")]
		[TestCase(11, 8, false, "-1.234567890")]
		public void IsValidNumber_FractionalPartGreaterThanScale_ReturnFalse(
			int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(3, 2, false, "+1.23")]
		[TestCase(3, 2, true, "-1.23")]
		public void IsValidNumber_IncorrectSign_ReturnFalse(
			int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(17, 1, true, "0")]
		[TestCase(2, 1, true, "0.0")]
		[TestCase(4, 2, true, "+1.23")]
		[TestCase(4, 2, false, "-0.00")]
		[TestCase(5, 3, false, "-1.234")]
		
		public void IsValidNumber_CorrectValue_ReturnTrue(
			int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeTrue();
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