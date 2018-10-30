using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(5, -1, TestName = "WhenScaleIsNegativeNumber")]
		[TestCase(5, 6, TestName = "WhenScaleIsGreaterThenPrecision")]
		[TestCase(-1, TestName = "WhenPrecisionIsNegativeNumber")]
		[TestCase(0, TestName = "WhenPrecisionIsNegativeNumber")]
		public void ThrowArgumentException(int precision, int scale = 0)
		{
			Action act = () => new NumberValidator(precision, scale);
			act.Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, TestName = "WhenCreateWithValidArguments")]
		public void DoesNotThrowException(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision, scale, true);
			act.Should().NotThrow();
        }

		[TestCase(17, 2, "0.0", TestName = "WhenNumberWithPointIsCorrect")]
		[TestCase(17, 2, "0", TestName = "WhenNumberWithoutFractionIsCorrect")]
		[TestCase(4, 2, "+1.23", TestName = "WhenNumberWithPlusSignIsEqualsToPrecision")]
		[TestCase(17, 2, "0,0", TestName = "WhenNumberWithCommaIsCorrect")]
		[TestCase(17, 0, "0", TestName = "WhenScaleIsZero_AndNumberIsInteger")]
		public void ReturnTrue(int precision, int scale, string value)
		{
			var result = new NumberValidator(precision, scale).IsValidNumber(value);
			result.Should().BeTrue();
        }

		[TestCase(3, 2, "00.00", TestName = "WhenNumberIsGreaterThanPrecision")]
		[TestCase(5, 2, "-0.00", true, TestName = "WhenNegativeNumbersAreForbidden_AndNumberIsNegative")]
		[TestCase(3, 2, "+0.00", true, TestName = "WhenNumberWithPlusSignIsGreaterThenPrecision")]
		[TestCase(3, 2, "-1.23", true, TestName = "WhenNumberWithMinusSignIsGreaterThenPrecision")]
		[TestCase(17, 2, "0.000", true, TestName = "WhenFractionIsGreaterThenAllowed")]
		[TestCase(3, 2, "a.sd", true, TestName = "WhenValueIsNotANumber")]
		[TestCase(17, 2, "  0,0", true, TestName = "WhenValueStartsWithWhitespaces")]
		[TestCase(17, 2, "0,0  ", true, TestName = "WhenValueEndsWithWhitespaces")]
		[TestCase(17, 2, " 0 , 0  ", true, TestName = "WhenValueHaveWhitespaces")]
		[TestCase(17, 2, null, true, TestName = "WhenValueIsNull")]
		[TestCase(17, 2, "", true, TestName = "WhenValueIsEmpty")]
		public void ReturnFalse(int precision, int scale, string value, bool onlyPositive = false)
		{
			var result = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
			result.Should().BeFalse();
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