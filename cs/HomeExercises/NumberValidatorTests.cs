using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true, TestName = "PrecisionIsNegative")]
		[TestCase(5, -5, true, TestName = "ScaleIsNegative")]
		[TestCase(3, 5, true, TestName = "ScaleGreaterPrecision")]
		public void NumberValidator_ThrowException(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.ShouldThrow<ArgumentException>();
		}
		
		[Test] 
		public void NumberValidator_ValidValues_WithoutException()
		{
			Action act = () => new NumberValidator(1, 0, true);
			act.ShouldNotThrow();
		}
		
		
		[TestCase(10, 2, true, null, TestName = "Null")]
		[TestCase(10, 2, true, "", TestName = "EmptyLine")]
		[TestCase(17, 2, true, "-1.0", TestName = "FloatNegativeNumberOnlyPositive")]
		[TestCase(17, 2, true, "-5", TestName = "IntNegativeNumberOnlyPositive")]
		[TestCase(3, 2, true, "a.sd", TestName = "LineWithLetters")]
		[TestCase(3, 2, true, "00.00", TestName = "LengthGreaterPrecision")]
		[TestCase(3, 2, false, "-0.00", TestName = "LengthWitMinusGreaterPrecision")]
		[TestCase(3, 2, true, "+0.00", TestName = "LengthWithPlusGreaterPrecision")]
		[TestCase(17, 2, true, "0.000", TestName = "LengthFractionalPartGreaterScale")]
		public void IsValidNumber_IsFalse(int precision, int scale, bool onlyPositive, string value)
		{
			var isValidNumber = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
			isValidNumber.Should().BeFalse();
		}
		
		[TestCase(17, 2, true, "0.0", TestName = "FloatPositiveNumberOnlyPositive")]
		[TestCase(17, 2, true, "0", TestName = "IntPositiveNumberOnlyPositive")]
		[TestCase(4, 2, true, "+1.23", TestName = "FloatWithPlusOnlyPositive")]
		[TestCase(4, 2, true, "+1", TestName = "IntWithPlusOnlyPositive")]
		[TestCase(17, 2, false, "-1.5", TestName = "FloatNegativeNumberNotOnlyPositive")]
		[TestCase(17, 2, false, "-123", TestName = "IntNegativeNumberNotOnlyPositive")]
		public void IsValidNumber_IsTrue(int precision, int scale, bool onlyPositive, string value)
		{
			var isValidNumber = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
			isValidNumber.Should().BeTrue();
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