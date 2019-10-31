using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 0, true, TestName = "IfPrecisionLessThanZero")]
		[TestCase(0, 0, true, TestName = "IfPrecisionIsEqualToZero")]
		[TestCase(17, -1, true, TestName = "IfScaleLessThanZero")]
		[TestCase(17, 17, true, TestName = "IfScaleIsEqualToPrecision")]
		[TestCase(17, 18, true, TestName = "IfScaleBiggerThanPrecision")]
		public void NumberValidator_ShouldThrowArgumentException_OnTheseInputArguments(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);

			action
				.ShouldThrow<ArgumentException>();
		}

		[TestCase(17, 16, true, TestName = "IfScaleLessThanPrecision")]
		public void NumberValidator_ShouldNotThrowArgumentException_OnTheseInputArguments(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);

			action
				.ShouldNotThrow<ArgumentException>();
		}

		[TestCase(17, 2, true, "123", TestName = "IfNumberScaleIsZero")]
		[TestCase(17, 2, true, "123.1", TestName = "IfNumberScaleLessThanValidatorScale")]
		[TestCase(17, 2, true, "123.12", TestName = "IfNumberScaleIsEqualToValidatorScale")]
		[TestCase(5, 2, true, "123,12", TestName = "IfSeparatorIsСomma")]
		[TestCase(5, 2, false, "-12.12", TestName = "IfNegativeNumberAndNotOnlyPositiveValidator")]
		[TestCase(5, 3, true, "12.1", TestName = "IfNumberPrecisionLessThanValidatorPrecision")]
		[TestCase(5, 3, true, "123.12", TestName = "IfNumberPrecisionIsEqualToValidatorPrecision")]
		public void IsValidNumber_ShouldBeTrue_OnTheseInputArguments(int precision, int scale, bool onlyPositive, string numberLine)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numberLine).Should().BeTrue();
		}

		[TestCase(17, 2, true, "123.123", TestName = "IfNumberScaleBiggerThanValidatorScale")]
		[TestCase(5, 2, true, "-12.12", TestName = "IfNegativeNumberAndOnlyPositiveValidator")]
		[TestCase(5, 3, true, "1234.12", TestName = "IfNumberPrecisionBiggerThanValidatorPrecisionAndIntegerPartHasIncreased")]
		[TestCase(5, 3, true, "123.123", TestName = "IfNumberPrecisionBiggerThanValidatorPrecisionAndFractionalPartHasIncreased")]
		[TestCase(5, 3, true, "-123.12", TestName = "IfNumberPrecisionWithMinusBiggerThanValidatorPrecision")]
		[TestCase(5, 3, true, "+123.12", TestName = "IfNumberPrecisionWithPlusBiggerThanValidatorPrecision")]
		[TestCase(5, 3, true, "aaa.12", TestName = "IfIntegerPartContainsNotDigitSymbols")]
		[TestCase(5, 3, true, "123.aa", TestName = "IfFractionalPartContainsNotDigitSymbols")]
		[TestCase(5, 3, true, "123a12", TestName = "IfSeparatorIsNotDotOrComma")]
		public void IsValidNumber_ShouldBeFalse_OnTheseInputArguments(int precision, int scale, bool onlyPositive, string numberLine)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numberLine).Should().BeFalse();
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