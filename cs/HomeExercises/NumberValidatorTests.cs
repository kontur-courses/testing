using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{

		[TestCase("00.00", TestName = "Fraction")]
		[TestCase("123", TestName = "Integer")]
		[TestCase("-1.23", TestName = "NegativeFraction")]
		[TestCase("-1", TestName = "NegativeInteger")]
		[TestCase("+1.23", TestName = "PositiveFraction")]
		[TestCase("1,23", TestName = "CommaDelimeter")]
		[TestCase("0.12", TestName = "IntPartZeroFracPartTwoDigits")]
		[TestCase("+2.12", 5, 3, true, TestName = "PositiveFractionWhenOnlyPositiveTrue")]
		[TestCase("1234567890123", 13, 3, true, TestName = "LongInt")]
		public void IsValidNumber_ReturnTrue
			(string input, int precision = 5, int scale = 3, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			validator.IsValidNumber(input).Should().BeTrue();
		}

		[TestCase("00.00", TestName = "FractionLargerThanPrecision")]
		[TestCase("0.000", TestName = "FractionPartLargerThanScale")]
		[TestCase("+1.23", TestName = "PositiveFractionLargerThanPrecision")]
		[TestCase("-1.23", 5, 3, true, TestName = "NegativeFractionWithOnlyPositiveTrue")]
		[TestCase("a.sd", TestName = "Letters")]
		[TestCase("a.12", TestName = "FractionLettersWithDigits")]
		[TestCase("12.a", TestName = "FractionDigitsWithLetters")]
		[TestCase("-", TestName = "SignWithoutDigits")]
		[TestCase(null, TestName = "Null")]
		public void IsValidNumber_ReturnFalse
			(string input, int precision = 3, int scale = 2, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			validator.IsValidNumber(input).Should().BeFalse();
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