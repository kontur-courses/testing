using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;

namespace HomeExercises
{
	public class NumberValidatorTests
	{		
		[TestCase(-1, 2, true, TestName= "NegativePrecision")]
		[TestCase(0, 1, true, TestName = "ZeroPrecision")]
        public void InitValidator_ThrowArgumentException_WhenIncorrectPrecision(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}

		[TestCase(2, -1, true, TestName = "NegativeScale")]
		[TestCase(2, 4, true, TestName = "HigherScaleThanPrecision")]
		[TestCase(2, 2, true, TestName = "TheSameScaleAsPrecision")]
        public void InitValidator_ThrowArgumentException_WhenIncorrectScale(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().Throw<ArgumentException>();
        }

		[TestCase(17, 4, true, TestName = "UnsignedNumber")]
		[TestCase(17, 4, false, TestName = "SignedNumber")]
		[TestCase(17, 0, true, TestName = "NumberWithoutFractionalPart")]
        public void InitValidator_NotThrowOnCorrectInput(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().NotThrow<ArgumentException>();
        }

		[TestCase(17, 4, true, "0.0", TestName = "ZeroWithFractionalPart")]
		[TestCase(17, 4, true, "0", TestName = "Zero")]
		[TestCase(17, 4, true, "0.000", TestName = "ZeroWithBigFractionalPart")]
		[TestCase(17, 4, true, "000.0", TestName = "ZeroWithBigIntegerPart")]
		[TestCase(17, 4, true, "17.24", TestName = "RandomValue")]
		[TestCase(17, 4, true, "+1488", TestName = "ValueWithPlusSign")]
		[TestCase(17, 4, false, "-0.1337", TestName = "NegativeValue")]
		[TestCase(4, 0, true, "+1000", TestName = "ValueWithPlusSignAndBigLength")] // Пропустил этот тест в прошлый раз. Судя по документации, он должен проходить, но он падает.
        public void IsValidNumber_OnCorrectInput(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(value)
				.Should().BeTrue("Given value is correct");
		}

		[TestCase(17, 4, true, "0.", TestName = "EmptyFractionalPart")]
		[TestCase(17, 4, true, ".0", TestName = "EmptyIntegerPart")]
		[TestCase(17, 4, true, "++0.0", TestName = "TwoSigns")]
		[TestCase(17, 4, true, "0..0", TestName = "TwoDots")]
		[TestCase(17, 4, true, "abcde.fg", TestName = "RandomString")]
		[TestCase(17, 4, true, "", TestName = "EmptyString")]
		[TestCase(17, 4, true, null, TestName = "NullString")]
        public void IsValidNumber_OnIncorrectInput_WhenWrongFormat(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(value)
				.Should().BeFalse("Given string is in wrong format");
        }


		[TestCase(2, 1, true, "00.0", TestName = "TooSmallPrecision")]
		[TestCase(10, 0, true, "00.0", TestName = "TooSmallScale")]
		[TestCase(17, 4, true, "-0.0", TestName = "NegativeZeroWhenOnlyPositive")]
        [TestCase(17, 4, true, "-17.4", TestName = "NegativeValueWhenOnlyPositive")]
		[TestCase(4, 3, true, "-17.40", TestName = "LengthWithSignMoreThanPrecision")]
        public void IsValidNumber_OnIncorrectInput_WhenWrongNumber(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(value)
				.Should().BeFalse("Given number is incorrect");
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