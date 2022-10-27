using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(0, TestName = "Zero precision", Category = "Precision")]
		[TestCase(-1, TestName = "Negative precision", Category = "Precision")]
		public void Constructor_ThrowsArgumentException_OnNonPositivePrecision(int precision)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, 2, true));
		}
		
		[TestCase(Category = "Precision")]
		public void Constructor_DoesNotThrow_OnRightPrecision()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		}

		[TestCase(1, TestName = "Scale >= precision", Category = "Scale")]
		[TestCase(-1, TestName = "Negative scale", Category = "Scale")]
		public void Constructor_ThrowsArgumentException_OnWrongScale(int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, scale, true));
		}
		
		[TestCase(0, TestName = "Scale == 0", Category = "Scale")]
		[TestCase(1, TestName = "Scale > 0 && scale < precision", Category = "Scale")]
		public void Constructor_DoesNotThrow_OnRightScale(int scale)
		{
			Assert.DoesNotThrow(() => new NumberValidator(2, scale, true));
		}
		
		[TestCase(3, 2, "12.34", TestName = "Precision: actual > expected", Category = "Precision")]
		[TestCase(3, 2, "00.00", TestName = "Precision: leading and trailing zero is included", Category = "Precision")]
		[TestCase(3, 2, "+1.23", TestName = "Precision: +/- is included", Category = "Precision")]
		[TestCase(4, 2, "1.123", TestName = "Scale: actual > expected", Category = "Scale")]
		[TestCase(4, 2, "0.000", TestName = "Scale: trailing zero is included", Category = "Scale")]
		[TestCase(3, 2, "-1.2", TestName = "Sign: negative input is not accepted when only positive is true", Category = "Format")]
		[TestCase(3, 2, "=1.2", TestName = "Sign is only '+' and '-'", Category = "Format")]
		[TestCase(4, 2, "+-1.2", TestName = "Can't enter several signs", Category = "Format")]
		[TestCase(3, 1, " 1.1", TestName = "Leading spaces is not accepted", Category = "Format")]
		[TestCase(4, 1, "1 1", TestName = "Separator is only '.' or ','", Category = "Format")]
		[TestCase(2, 1, "1.1 ", TestName = "Space after number is not accepted", Category = "Format")]
		[TestCase(4, 1, "+ 1.2", TestName = "Space between sign and number is not accepted", Category = "Format")]
		[TestCase(4, 2, "+a.sd", TestName = "Not a number", Category = "Format")]
		[TestCase(4, 2, "1s.2d", TestName = "Not only digits", Category = "Format")]
		[TestCase(1, 0, "", TestName = "Empty string is invalid", Category = "Format")]
		[TestCase(1, 0, null, TestName = "Null string is invalid", Category = "Format")]
		[TestCase(1, 0, "1.", TestName = "Fractional part is followed by a delimiter", Category = "Format")]
		[TestCase(1, 0, ".0", TestName = "There is integer part is always exists", Category = "Format")]
		public void IsValidNumber_False_OnIncorrectValue(int precision, int scale, string value)
		{
			new NumberValidator(precision, scale, true).IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCase(17, 1, "100.1", TestName = "Precision: expected > actual", Category = "Precision")]
		[TestCase(4, 2, "100.1", TestName = "Scale: expected > actual", Category = "Scale")]
		[TestCase(3, 2, "1.34", TestName = "Scale: fractional part length == scale", Category = "Scale")]
		[TestCase(3, 0, "146", TestName = "Scale: without fractional part", Category = "Scale")]
		[TestCase(3, 1, "-1.7", TestName = "Accept negative numbers if only positive is false", Category = "Format")]
		[TestCase(2, 1, "1,1", TestName = "',' is accepted instead of '.'", Category = "Format")]
		public void IsValidNumber_True_OnCorrectValue(int precision, int scale, string value)
		{
			new NumberValidator(precision, scale).IsValidNumber(value).Should().BeTrue();
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
				throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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