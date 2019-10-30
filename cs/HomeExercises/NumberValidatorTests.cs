using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[Test]
		[TestCase(1, 0, false, TestName = "WhenPositivePrecision")]
		[TestCase(2, 1, false, TestName = "WhenPrecisionGreaterScale")]
		[TestCase(1, 0, true, TestName = "WhenNotOnlyPositive")]
		public void Constructor_DoesNotThrow(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().NotThrow();
		}

		[Test]
		[TestCase(0, 2, true, TestName = "WhenNonPositivePrecision")]
		[TestCase(2, -1, true, TestName = "WhenNegativeScale")]
		[TestCase(1, 2, true, TestName = " WhenScaleGreaterPrecision")]
		public void Constructor_ThrowsArgumentException(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		[TestCase(3, 2, true, "11.11", TestName = "WhenActualPrecisionGreaterExpected")]
		[TestCase(3, 2, true, "-2.34", TestName = "WhenOnlyPositiveExpectedButWasNegative")]
		[TestCase(17, 2, true, "0.000", TestName = "WhenActualScaleGreaterExpected")]
		[TestCase(17, 2, true, "a.sd", TestName = "WhenNotANumber")]
		[TestCase(10, 2, true, null, TestName = "WhenNullValue")]
		[TestCase(10, 2, true, "", TestName = "WhenEmptyString")]
		[TestCase(3, 2, false, "-1.23", TestName = "WhenSignTakenIntoAccountPrecision")]
		[TestCase(3, 2, true, "++1", TestName = "WhenSeveralSigns")]
		[TestCase(3, 2, true, "1..2", TestName = "WhenSeveralSeparators")]
		[TestCase(3, 2, true, "1.", TestName = "WhenEndsOnDot")]
		[TestCase(3, 2, true, ".1", TestName = "WhenStartsOnDot")]
		public void IsValidNumber_ReturnsFalse(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		[TestCase(10, 2, false, "-1.23", TestName = "WhenNegativeNumber")]
		[TestCase(4, 2, true, "+1.23", TestName = "WhenPositiveNumber")]
		[TestCase(4, 1, true, "1234", TestName = "WhenValidatorPrecisionSameAsValue")]
		[TestCase(3, 2, true, "1.23", TestName = "WhenValidatorScaleSameAsValue")]
		[TestCase(3, 1, true, "1", TestName = "WhenInteger")]
		[TestCase(3, 2, true, "1,23", TestName = "WhenSeparatorIsNotPoint")]
		public void IsValidNumber_ReturnsTrue(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
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