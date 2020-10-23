using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true, TestName = "ThrowException_WhenNotPositivePrecision")]
		[TestCase(0, 2, true, TestName = "ThrowException_WhenZeroPrecision")]
		[TestCase(0, -1, true, TestName = "ThrowException_WhenNegativeScale")]
		[TestCase(5, 5, true, TestName = "ThrowException_WhenScaleEqualPrecision")]
		[TestCase(4, 5, true, TestName = "ThrowException_WhenScaleBiggerThenPrecision")]
		public void ThrowException(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().Throw<ArgumentException>();
		}

		[TestCase(4, 2, true, TestName = "DoesNotThrowException_WhenPrecisionBiggerThenScale")]
		[TestCase(2, 0, true, TestName = "DoesNotThrowException_WhenZeroScale")]
		public void DoesNotThrowException(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().NotThrow<ArgumentException>();
		}

		[TestCase(4, 2, false, "0,1", TestName = "IsValidNumber_WhenFractNumberWithСomma")]
		[TestCase(4, 2, false, "0.1", TestName = "IsValidNumber_WhenFractNumberWithDot")]
		[TestCase(4, 2, true, "+0.0", TestName = "IsValidNumber_WhenPositiveFractNumber")]
		[TestCase(4, 2, true, "+0", TestName = "IsValidNumber_WhenPositiveIntNumber")]
		[TestCase(4, 2, false, "-0.0", TestName = "IsValidNumber_WhenNegativeFractNumber")]
		[TestCase(4, 2, false, "-0", TestName = "IsValidNumber_WhenNegativeIntNumber")]
		[TestCase(17, 2, false, "99999999999999", TestName = "IsValidNumber_WhenLargeIntNumber")]
		[TestCase(30, 17, false, "99999999999999,9999999999999", TestName = "IsValidNumber_WhenLargeFractNumber")]
		public void IsValidNumber(int precision, int scale, bool onlyPositive, string input)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(input).Should().BeTrue();
		}

		[TestCase(3, 2, true, " ", TestName = "IsNotValidNumber_WhenEmptyInput")]
		[TestCase(3, 2, true, null, TestName = "IsNotValidNumber_WhenNullInput")]
		[TestCase(3, 2, true, "   0.1", TestName = "IsNotValidNumber_WhenEmptySpaceBeforeNumber")]
		[TestCase(3, 2, true, "0.1   ", TestName = "IsNotValidNumber_WhenEmptySpaceAfterNumber")]
		[TestCase(3, 2, true, "+ 0.1", TestName = "IsNotValidNumber_WhenEmptySpaceBetweenSignAndNumber")]
		[TestCase(3, 2, true, "0.1af", TestName = "IsNotValidNumber_WhenInputContainsCharacters")]
		[TestCase(3, 2, true, "ABCdeeeeeeffFFFff", TestName = "IsNotValidNumber_WhenTextInput")]
		[TestCase(3, 1, true, "+0.00", TestName = "IsNotValidNumber_WhenInputBiggerThenPrecision")]
		[TestCase(6, 2, true, "00.000", TestName = "IsNotValidNumber_WhenFracPartBiggerThenScale")]
		[TestCase(6, 2, true, "-0.00", TestName = "IsNotValidNumber_WhenOnlyPositiveButInputIsNegativeNumber")]
		[TestCase(6, 2, true, "++1.23", TestName = "IsNotValidNumber_WhenMoreThanOneSign")]
		[TestCase(6, 2, true, "1,23,23", TestName = "IsNotValidNumber_WhenMoreThanOneComma")]
		[TestCase(6, 2, true, "1.23.23", TestName = "IsNotValidNumber_WhenMoreThanOneDot")]
		[TestCase(6, 2, true, "1.", TestName = "IsNotValidNumber_WhenIntNumberWithDot")]
		[TestCase(6, 2, true, "1,", TestName = "IsNotValidNumber_WhenIntNumberWithComma")]
		public void IsNotValidNumber(int precision, int scale, bool onlyPositive, string input)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(input).Should().BeFalse();
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