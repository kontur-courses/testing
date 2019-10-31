using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[Category("ConstructorTests")]
		[TestCase(1, 0, true, TestName = "OnPositivePrecisionAndNotNegativeScale")]
		public void Constructor_ShouldNotThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.ShouldNotThrow<ArgumentException>();
		}

		[Category("ConstructorTests")]
		[TestCase(-1, 1, true, TestName = "OnNegativePrecision")]
		[TestCase(0, 1, true, TestName = "OnZeroPrecision")]
		[TestCase(1, -1, true, TestName = "OnNegativeScale")]
		[TestCase(1, 1, true, TestName = "OnPrecisionEqualToScale")]
		[TestCase(1, 2, true, TestName = "OnPrecisionSmallerThanScale")]
		public void Constructor_ShouldThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.ShouldThrow<ArgumentException>();
		}

		[Category("IsValidNumberTests")]
		[TestCase(1, 0, true, "a", ExpectedResult = false,
			TestName = "ReturnsFalse_OnNotNumberArgument")]
		[TestCase(3, 2, true, "0.0.0", ExpectedResult = false,
			TestName = "ReturnsFalse_OnWrongNumberFormat")]
		[TestCase(3, 1, true, "-0.0", ExpectedResult = false,
			TestName = "ReturnsFalse_OnOnlyPositiveAndNegativeArgument")]
		[TestCase(1, 0, true, "00", ExpectedResult = false,
			TestName = "ReturnsFalse_OnNumberBiggerThanPrecision")]
		[TestCase(2, 1, true, "+0.0", ExpectedResult = false,
			TestName = "ReturnsFalse_OnNumberWithSignBiggerThanPrecision")]
		[TestCase(2, 0, true, "0.0", ExpectedResult = false,
			TestName = "ReturnsFalse_OnNumberFractionalPartBiggerThanScale")]
		[TestCase(2, 1, true, null, ExpectedResult = false,
			TestName = "ReturnsFalse_OnNullString")]
		[TestCase(2, 1, true, "", ExpectedResult = false,
			TestName = "ReturnsFalse_OnEmptyString")]
		[TestCase(2, 1, true, "0.0", ExpectedResult = true,
			TestName = "ReturnsTrue_OnValidNumberWithFractionalPart")]
		[TestCase(1, 0, true, "0", ExpectedResult = true,
			TestName = "ReturnsTrue_OnValidNumberWithoutFractionalPart")]
		[TestCase(2, 1, true, "0,0", ExpectedResult = true,
			TestName = "ReturnsTrue_OnValidNumberWithComma")]
		[TestCase(3, 1, false, "-0.0", ExpectedResult = true,
			TestName = "ReturnsTrue_OnValidNegativeNumber")]
		public bool IsValidNumber(int precision, int scale, bool onlyPositive, string number)
		{
			var nv = new NumberValidator(precision, scale, onlyPositive);
			return nv.IsValidNumber(number);
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