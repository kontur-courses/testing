using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(2, 1, TestName = "PrecisionGreaterThanScale")]
		[TestCase(2, 0, TestName = "NonNegativeScale")]
		[TestCase(2, 1, true, TestName = "OnlyPositiveNumberValidator")]
		public void Creation_DoesNotThrowException(int precision, int scale, bool onlyPositive = false)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
		}
		
		[TestCase(0, 1, TestName = "PrecisionEqualsZero")]
		[TestCase(-1, 1, TestName = "NegativePrecision")]
		[TestCase(2, -1, TestName = "NegativeScale")]
		[TestCase(2, 3, TestName = "ScaleGreaterThanPrecision")]
		[TestCase(2, 2, TestName = "ScaleEqualsPrecision")]
		public void Creation_ThrowsArgumentException(int precision, int scale, bool onlyPositive = false)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
		}
		
		[TestCase("123", 3, 0, TestName = "IntegerNumberLengthEqualsPrecision")]
		[TestCase("123", 4, 0, TestName = "IntegerNumberLengthLessThanPrecision")]
		[TestCase("12.3", 3, 2, TestName = "FractionalNumberLengthEqualsPrecision")]
		[TestCase("12.3", 4, 2, TestName = "FractionalNumberLengthLessThanPrecision")]
		[TestCase("-12", 3, 0, TestName = "NegativeNumberLengthEqualsPrecision")]
		[TestCase("-12", 4, 0, TestName = "NegativeNumberLengthLessThanPrecision")]
		[TestCase("-12.3", 4, 2, TestName = "FractionalNegativeNumberLengthEqualsPrecision")]
		[TestCase("-12.3", 5, 2, TestName = "FractionalNegativeNumberLengthLessThanPrecision")]
		[TestCase("+12", 3, 0, TestName = "PositiveNumberLengthEqualsPrecision")]
		[TestCase("+12", 4, 0, TestName = "PositiveNumberLengthLessThanPrecision")]
		[TestCase("+12.3", 4, 2, TestName = "FractionalPositiveNumberLengthEqualsPrecision")]
		[TestCase("+12.3", 5, 2, TestName = "FractionalPositiveNumberLengthLessThanPrecision")]
		[TestCase("1.23", 10, 2, TestName = "FractionalPartLengthEqualsScale")]
		[TestCase("1.23", 10, 3, TestName = "FractionalPartLengthLessThanScale")]
		[TestCase("1", 2, 0, true, TestName = "PositiveNumberWithOnlyPositiveValidator")]
		[TestCase("1.2", 3, 2, true, TestName = "PositiveFractionalNumberWithOnlyPositiveValidator")]
		public void IsValidNumber_ShouldBeTrue(string number, int precision, int scale, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(number).Should().BeTrue();
		}
		
		[TestCase(null, 5, 4, TestName = "NullString")]
		[TestCase("", 5, 4, TestName = "EmptyString")]
		[TestCase("a.12", 5, 4, TestName = "NaNString")]
		[TestCase("1.2.3", 5, 4, TestName = "StringWithTwoPoints")]
		[TestCase("123", 2, 0, TestName = "IntegerNumberLengthGreaterThanPrecision")]
		[TestCase("12.3", 2, 1, TestName = "FractionalNumberLengthGreaterThanPrecision")]
		[TestCase("1.23", 3, 1, TestName = "FractionalPartLengthGreaterThanScale")]
		[TestCase("-12", 2, 0, TestName = "NegativeNumberLengthGreaterThanPrecision")]
		[TestCase("+12", 2, 0, TestName = "PositiveNumberLengthGreaterThanPrecision")]
		[TestCase("-1", 3, 0, true, TestName = "NegativeNumberWithOnlyPositiveValidator")]
		public void IsValidNumber_ShouldBeFalse(string number, int precision, int scale, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(number).Should().BeFalse();
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