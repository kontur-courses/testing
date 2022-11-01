using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		[TestCase(4, 2, true, "-0.00",
			TestName = "NegativeNumberWhenPositiveValidator")]
		[TestCase(3, 2, false, "+0.00",
			TestName = "CharacterCountInDoubleValueMoreThenPrecisionBecauseOfSign")]
		[TestCase(17, 2, true, "0.000",
			TestName = "FracPartMoreThanScale")]
		[TestCase(3, 2, true, "00.00",
			TestName = "DoubleAllLenghtMoreThanPrecision")]
		[TestCase(3, 0, true, "0000",
			TestName = "IntegerAllLenghtMoreThanPrecision")]
		[TestCase(2, 1, true, "00.00",
			TestName = "AllLenghtMoreThanPrecisionFracPartMoreThanScale")]
		[TestCase(3, 2, true, "",
			TestName = "EmptyString")]
		[TestCase(3, 2, true, null,
			TestName = "NullInput")]
		[TestCase(3, 2, true, "a.sd",
			TestName = "EnglishLettersAreNotDoubleRegexPattern")]
		[TestCase(3, 2, true, "asd",
			TestName = "EnglishLettersAreNotIntegerRegexPattern")]
		[TestCase(3, 2, true, "и.пр",
			TestName = "RussianLettersAreNotDoubleRegexPattern")]
		[TestCase(3, 2, true, "ипр",
			TestName = "RussianLettersAreNotIntegerRegexPattern")]
		[TestCase(3, 2, true, "&^:;($)#@!~`*_-[]{}",
			TestName = "StrangeSymbols")]
		public void IsValidNumber_False(int precision, int scale, bool onlyPositive,
			string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value)
				.Should().BeFalse();
		}

		[Test]
		[TestCase(4, 2, false, "-1.23",
			TestName = "DoubleWithMinus")]
		[TestCase(4, 2, false, "+1.23",
			TestName = "DoubleWithPlus")]
		[TestCase(4, 2, false, "1.23",
			TestName = "DoubleWithoutSign")]
		[TestCase(4, 2, true, "0",
			TestName = "IntegerWithoutSign")]
		[TestCase(4, 2, false, "-0",
			TestName = "IntegerWithMinus")]
		[TestCase(4, 2, false, "+0",
			TestName = "IntegerWithPlus")]
		[TestCase(4, 2, true, "0.0",
			TestName = "PointInDouble")]
		[TestCase(4, 2, true, "0,0",
			TestName = "CommaInDouble")]
		public void IsValidNumber_True(int precision, int scale, bool onlyPositive,
			string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value)
				.Should().BeTrue();
		}

		[Test]
		[TestCase(-1, 2, true, TestName = "NegativePrecision")]
		[TestCase(0, 2, true, TestName = "PrecisionIsZero")]
		[TestCase(5, -1, true, TestName = "NegativeScale")]
		[TestCase(1, 2, true, TestName = "ScaleMoreThanPrecision")]
		[TestCase(1, 1, true, TestName = "ScaleEqualsPrecision")]
		public void CreateNumberValidator_ArgumentException(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().Throw<ArgumentException>();
		}

		[Test]
		[TestCase(1, 0, true,
			TestName = "PositiveIntegerValidatorPrecisionMoreThanScale")]
		[TestCase(1, 0, false,
			TestName = "ValidatorIntegerPrecisionMoreThanScale")]
		[TestCase(2, 1, true,
			TestName = "PositiveDoubleValidatorPrecisionMoreThanScale")]
		[TestCase(2, 1, false,
			TestName = "ValidatorDoublePrecisionMoreThanScale")]
		public void CreateNumberValidator_NotThrow(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
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