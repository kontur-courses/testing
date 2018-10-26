using System;
using System.Collections;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{

		[TestCase("0.0", TestName = "Fraction")]
		[TestCase("0", TestName = "Integer")]
		[TestCase("123456789123", TestName = "Long")]
		public void NumberValidator_WithBigPrecisionAndSmallScale_IsValid(string input)
		{
			var validator = new NumberValidator(17, 2, true);

			validator.IsValidNumber(input).Should().BeTrue();
		}

		[TestCase("00.00", TestName = "FractionLargerThanPrecision")]
		[TestCase("0.000", TestName = "FractionPartLargerThanScale")]
		[TestCase("+1.23", TestName = "NegativeFractionLargerThanPrecision")]
		[TestCase("-1.23", TestName = "PositiveFractionLargerThanPrecision")]
		[TestCase("a.sd", TestName = "Letters")]
		[TestCase("a.12", TestName = "FractionLettersWithDigits")]
		[TestCase("12.a", TestName = "FractionDigitsWithLetters")]
		[TestCase("-", TestName = "SignWithoutDigits")]
		[TestCase(null, TestName = "Null")]
		public void NumberValidator_SmallPrecisionAndSmallScale_IsNotValid(string input)
		{
			var validator = new NumberValidator(3, 2, true);

			validator.IsValidNumber(input).Should().BeFalse();
		}

		[TestCase("-1.23", ExpectedResult = true, TestName = "NegativeFraction")]
		[TestCase("+1.23", ExpectedResult = true, TestName = "PositiveFraction")]
		public bool NumberValidator_NotOnlyPositive_IsValid(string input)
		{
			var validator = new NumberValidator(5, 3);

			return validator.IsValidNumber(input);
		}

		[TestCase(-1, 2, TestName = "PrecisionNegative")]
		[TestCase(1, -1, TestName = "ScaleNegative")]
		[TestCase(2, 3, TestName = "ScaleLargerPrecision")]
		public void NumberValidator_WithBadArguments_ShouldThrowException(int precision, int scale)
		{
			Action action = () => { new NumberValidator(precision, scale, true); };

			action.ShouldThrow<ArgumentException>();
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