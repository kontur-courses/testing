using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(4, 2, true, "-1.23",
			TestName = "NegativeNumberWhenPositiveValidator")]
		[TestCase(3, 0, true, "1234",
			TestName = "LengthIntegerMoreThanPrecisionPositiveValidator")]
		[TestCase(3, 0, false, "1234",
			TestName = "LengthIntegerMoreThanPrecision")]
		[TestCase(3, 0, true, "+123",
			TestName = "LengthIntegerWithSignMoreThanPrecisionPositiveValidator")]
		[TestCase(3, 0, false, "+123",
			TestName = "LengthIntegerWithSignMoreThanPrecisionPositiveValidator")]
		[TestCase(3, 2, true, "10.23",
			TestName = "LengthDoubleMoreThanPrecisionPositiveValidator")]
		[TestCase(3, 2, false, "10.23",
			TestName = "LengthDoubleMoreThanPrecision")]
		[TestCase(3, 2, true, "+1.23",
			TestName = "LengthDoubleWithSignMoreThanPrecisionPositiveValidator")]
		[TestCase(3, 2, false, "+1.23",
			TestName = "LengthDoubleWithSignMoreThanPrecision")]
		[TestCase(1, 0, false, "-0",
			TestName = "NegativeZeroIntegerLengthMoreThanPrecision")]
		[TestCase(2, 1, false, "-0.0",
			TestName = "NegativeZeroDoubleLengthMoreThanPrecision")]
		[TestCase(1, 0, false, "00",
			TestName = "PositiveZeroIntegerLengthMoreThanPrecision")]
		[TestCase(3, 2, false, "00.00",
			TestName = "PositiveZeroDoubleLengthMoreThanPrecision")]
		[TestCase(1, 0, true, "00",
			TestName = "PositiveZeroIntegerLengthMoreThanPrecisionPositiveValidator")]
		[TestCase(2, 1, true, "00.0",
			TestName = "PositiveZeroDoubleLengthMoreThanPrecisionPositiveValidator")]
		[TestCase(3, 2, false, "-123",
			TestName = "LengthNegativeIntegerMoreThanPrecision")]
		[TestCase(3, 2, false, "-12.3",
			TestName = "LengthNegativeDoubleMoreThanPrecision")]
		[TestCase(4, 1, true, "10.23",
			TestName = "PositiveDoubleFractionalPartMoreThanScalePositiveValidator")]
		[TestCase(4, 1, false, "10.23",
			TestName = "PositiveDoubleFractionalPartMoreThanScale")]
		[TestCase(4, 1, true, "+1.23",
			TestName = "PositiveDoubleWithSignFractionalPartMoreThanScalePositiveValidator")]
		[TestCase(4, 1, false, "+1.23",
			TestName = "PositiveDoubleWithSignFractionalPartMoreThanScale")]
		[TestCase(3, 0, false, "-0.0",
			TestName = "NegativeZeroDoubleFractionalPartMoreThanScale")]
		[TestCase(3, 1, false, "0.00",
			TestName = "PositiveZeroDoubleFractionalPartMoreThanScale")]
		[TestCase(3, 0, true, "00.0",
			TestName = "PositiveZeroDoubleFractionalPartMoreThanScalePositiveValidator")]
		[TestCase(2, 1, true, "12.34",
			TestName = "AllLengthMoreThanPrecisionFracPartMoreThanScale")]
		[TestCase(3, 1, true, "10.23",
			TestName = "PositiveDoubleLengthMoreThanPrecisionFractionalPartMoreThanScalePositiveValidator")]
		[TestCase(3, 1, false, "10.23",
			TestName = "PositiveDoubleLengthMoreThanPrecisionFractionalPartMoreThanScale")]
		[TestCase(3, 1, true, "+1.23",
			TestName = "PositiveDoubleWithSignLengthMoreThanPrecisionFractionalPartMoreThanScalePositiveValidator")]
		[TestCase(3, 1, false, "+1.23",
			TestName = "PositiveDoubleWithSignLengthMoreThanPrecisionFractionalPartMoreThanScale")]
		[TestCase(2, 0, false, "-0.0",
			TestName = "NegativeZeroDoubleLengthMoreThanPrecisionFractionalPartMoreThanScale")]
		[TestCase(2, 0, false, "0.00",
			TestName = "PositiveZeroDoubleLengthMoreThanPrecisionFractionalPartMoreThanScale")]
		[TestCase(2, 0, true, "00.0",
			TestName = "PositiveZeroDoubleLengthMoreThanPrecisionFractionalPartMoreThanScalePositiveValidator")]
		[TestCase(3, 0, false, "-12.3",
			TestName = "NegativeDoubleFractionalPartMoreThanScale")]
		[TestCase(3, 2, true, "",
			TestName = "EmptyString")]
		[TestCase(3, 2, true, null,
			TestName = "NullInput")]
		[TestCase(3, 2, true, "ㅤ",
			TestName = "InvisibleCharacter")]
		[TestCase(3, 2, true, "ㅤ",
			TestName = "Space")]
		[TestCase(3, 2, true, "a.sd",
			TestName = "EnglishLettersAreNotDoubleRegexPattern")]
		[TestCase(3, 2, true, "asd",
			TestName = "EnglishLettersAreNotIntegerRegexPattern")]
		[TestCase(3, 2, true, "и.пр",
			TestName = "RussianLettersAreNotDoubleRegexPattern")]
		[TestCase(3, 2, true, "ипр",
			TestName = "RussianLettersAreNotIntegerRegexPattern")]
		[TestCase(30, 2, true, "&^:;($)#@!~`*_-[]{}",
			TestName = "StrangeSymbols")]
		[TestCase(3, 2, true, ",1",
			TestName = "StartsWithComma")]
		[TestCase(3, 2, true, ".1",
			TestName = "StartsWithDot")]
		[TestCase(10, 5, true, ".1.1",
			TestName = "NumberHasTwoDots")]
		[TestCase(10, 5, false, "--1.2",
			TestName = "StartsWithTwoMinuses")]
		[TestCase(10, 5, false, "~1.2",
			TestName = "StartsWithTilda")]
		[TestCase(10, 5, false, "#1.2",
			TestName = "StartsWithLattice")]
		[TestCase(10, 5, false, "*1.2",
			TestName = "StartsWithAsterisk")]
		[TestCase(10, 5, false, "¾",
			TestName = "Fractions")]
		[TestCase(10, 5, false, "XVI",
			TestName = "Roman")]
		[TestCase(10, 5, false, "εʹ",
			TestName = "Greek")]
		public void IsValidNumber_ShouldBeFalse(int precision, int scale, bool onlyPositive,
			string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value)
				.Should().BeFalse();
		}


		[TestCase(4, 2, false, "+1.23",
			TestName = "DoubleWithPlus")]
		[TestCase(4, 2, false, "1.23",
			TestName = "DoubleWithoutSign")]
		[TestCase(4, 2, true, "+1.23",
			TestName = "DoubleWithPlusPositiveValidator")]
		[TestCase(4, 2, true, "1.23",
			TestName = "DoubleWithoutSignPositiveValidator")]
		[TestCase(4, 2, false, "+1,23",
			TestName = "DoubleWithPlusAndComma")]
		[TestCase(4, 2, false, "1,23",
			TestName = "DoubleWithoutSignAndWithComma")]
		[TestCase(4, 2, true, "+1,23",
			TestName = "DoubleWithPlusAndCommaPositiveValidator")]
		[TestCase(4, 2, true, "1,23",
			TestName = "DoubleWithoutSignAndWithCommaPositiveValidator")]
		[TestCase(4, 0, false, "+123",
			TestName = "IntegerWithPlus")]
		[TestCase(4, 0, false, "123",
			TestName = "IntegerWithoutSign")]
		[TestCase(4, 0, true, "+123",
			TestName = "IntegerWithPlusPositiveValidator")]
		[TestCase(4, 0, true, "123",
			TestName = "IntegerWithoutSignPositiveValidator")]
		[TestCase(4, 2, true, "0",
			TestName = "ZeroWithoutSignPositiveValidator")]
		[TestCase(4, 2, false, "0",
			TestName = "ZeroWithoutSign")]
		[TestCase(4, 2, false, "-0",
			TestName = "ZeroWithMinus")]
		[TestCase(4, 2, false, "+0",
			TestName = "ZeroWithPlus")]
		[TestCase(4, 2, true, "+0",
			TestName = "ZeroWithPlusPositiveValidator")]
		[TestCase(4, 2, true, "0.0",
			TestName = "ZeroWithDotPositiveValidator")]
		[TestCase(4, 2, false, "0.0",
			TestName = "ZeroWithDot")]
		[TestCase(4, 2, true, "0,0",
			TestName = "ZeroWithCommaPositiveValidator")]
		[TestCase(4, 2, false, "0,0",
			TestName = "ZeroWithComma")]
		[TestCase(4, 2, false, "-0.0",
			TestName = "NegativeZeroWithDot")]
		[TestCase(4, 2, false, "-0,0",
			TestName = "NegativeZeroWithComma")]
		[TestCase(4, 2, false, "-1.23",
			TestName = "NegativeDoubleAndDot")]
		[TestCase(4, 2, false, "-1,23",
			TestName = "NegativeDoubleAndComma")]
		[TestCase(4, 0, false, "-123",
			TestName = "NegativeInteger")]
		[TestCase(100, 0, false, "1234567891234556789867578793567456456323245",
			TestName = "VeryBigInteger")]
		[TestCase(100, 0, true, "1234567891234556789867578793567456456323245",
			TestName = "VeryBigIntegerPositiveValidator")]
		[TestCase(101, 100, false,
			"1234567891234556789867578793567456456323245.1234567891234556789867578793567456456323245",
			TestName = "VeryBigDouble")]
		[TestCase(101, 100, true,
			"1234567891234556789867578793567456456323245.1234567891234556789867578793567456456323245",
			TestName = "VeryBigDoublePositiveValidator")]
		[TestCase(4, 0, false, "010",
			TestName = "IntegerStartsWithZero")]
		[TestCase(4, 0, false, "-010",
			TestName = "NegativeIntegerStartsWithZero")]
		[TestCase(4, 1, false, "01.0",
			TestName = "IntegerPartOfDoubleStartsWithZero")]
		[TestCase(4, 1, false, "-01.0",
			TestName = "IntegerPartOfNegativeDoubleStartsWithZero")]
		public void IsValidNumber_ShouldBeTrue(int precision, int scale, bool onlyPositive,
			string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value)
				.Should().BeTrue();
		}

		[TestCase(-1, 2, true, TestName = "NegativePrecision")]
		[TestCase(0, 2, true, TestName = "PrecisionIsZero")]
		[TestCase(5, -1, true, TestName = "NegativeScale")]
		[TestCase(1, 2, true, TestName = "ScaleMoreThanPrecision")]
		[TestCase(1, 1, true, TestName = "ScaleEqualsPrecision")]
		public void CreateNumberValidator_ShouldThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, true,
			TestName = "PositiveIntegerValidatorPrecisionMoreThanScale")]
		[TestCase(1, 0, false,
			TestName = "ValidatorIntegerPrecisionMoreThanScale")]
		[TestCase(2, 1, true,
			TestName = "PositiveDoubleValidatorPrecisionMoreThanScale")]
		[TestCase(2, 1, false,
			TestName = "ValidatorDoublePrecisionMoreThanScale")]
		public void CreateNumberValidator_ShouldCreate(int precision, int scale, bool onlyPositive)
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