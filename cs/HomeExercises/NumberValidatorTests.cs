using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(3, 2, true, "1.0d", false, TestName = "Correct number's form with letter => False")]
		[TestCase(1, 0, true, "", false, TestName = "Empty string => False")]
		[TestCase(1, 0, true, " ", false, TestName = "Whitespace => False")]
//		[TestCase(2, 0, true, "1\n", false, TestName = "Correct number and whitespace => False")] - падает
		[TestCase(1, 0, true, null, false, TestName = "Null => False")]
		[TestCase(3, 0, true, "001", true, TestName = "Zero before integer part => True")]
		[TestCase(2, 0, true, "+11", false, TestName = "Number's chars count grater than precision => False")]
		[TestCase(2, 1, true, "1.01", false, TestName = "Fractional part grater than scale => False")]
		[TestCase(2, 0, true, "-1", false, TestName = "Negative number when must be positive only => False ")]
		[TestCase(2, 1, true, "+.1", false, TestName = "Integer part contains plus only => False")]
		[TestCase(2, 0, false, "-1", true, TestName = "Work with negative number when onlyPositive=False => True")]
		[TestCase(1, 0, true, "1.", false, TestName = "Number with point and without fractional part => False")]
		public void IsValidNumber(int precision, int scale, bool onlyPositive, string value, bool expectedResult)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var actualResult = validator.IsValidNumber(value);
			actualResult.Should().Be(expectedResult);
		}

		[TestCase(-1, 0, TestName = "Negative precision")]
		[TestCase(0, 0, TestName = "Zero precision")]
		[TestCase(1, 1, TestName = "Scale equals to precision")]
		[TestCase(1, 2, TestName = "Scale is grater than precision")]
		[TestCase(1, -1, TestName = "Negative scale")]
		public void NumberValidatorInitialise_ThrowsException(int precision, int scale)
		{
			Action initialiseValidator = () => new NumberValidator(precision, scale);
			initialiseValidator.ShouldThrow<ArgumentException>();
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