using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(false, 2, 1, TestName = "Precision Greater Than Scale")]
		[TestCase(false, 2, 0, TestName = "Scale Equals Zero")]
		[TestCase(false, 2, 0, true, TestName = "Only Positive")]
		[TestCase(true, 0, 1, TestName = "Precision Equals Zero")]
		[TestCase(true, -1, 1, TestName = "Negative Precision")]
		[TestCase(true, 2, -1, TestName = "Negative Scale")]
		[TestCase(true, 2, 3, TestName = "Scale Greater Than Precision")]
		[TestCase(true, 2, 2, TestName = "Scale Equals Precision")]
		public void Creation(bool shouldThrow, int precision, int scale, bool onlyPositive = false)
		{
			Func<NumberValidator> creation = () => new NumberValidator(precision, scale, onlyPositive);
			var message = $"precision={precision}, scale={scale}";

			if (shouldThrow)
				creation.Should().Throw<ArgumentException>(message);
			else
				creation.Should().NotThrow(message);
		}
		
		[TestCase(true, "123", 3, 0, TestName = "Integer Number Length Equals Precision")]
		[TestCase(true, "123", 4, 0, TestName = "Integer Number Length Less Than Precision")]
		[TestCase(true, "12.3", 3, 2, TestName = "Fractional Number Length Equals Precision")]
		[TestCase(true, "12.3", 4, 2, TestName = "Fractional Number Length Less Than Precision")]
		[TestCase(true, "-12", 3, 0, TestName = "Negative Number Length Equals Precision")]
		[TestCase(true, "-12", 4, 0, TestName = "Negative Number Length Less Than Precision")]
		[TestCase(true, "-12.3", 4, 2, TestName = "Fractional Negative Number Length Equals Precision")]
		[TestCase(true, "-12.3", 5, 2, TestName = "Fractional Negative Number Length Less Than Precision")]
		[TestCase(true, "+12", 3, 0, TestName = "Positive Number Length Equals Precision")]
		[TestCase(true, "+12", 4, 0, TestName = "Positive Number Length Less Than Precision")]
		[TestCase(true, "+12.3", 4, 2, TestName = "Fractional Positive Number Length Equals Precision")]
		[TestCase(true, "+12.3", 5, 2, TestName = "Fractional Positive Number Length Less Than Precision")]
		[TestCase(true, "1.23", 10, 2, TestName = "Fractional Part Length Equals Scale")]
		[TestCase(true, "1.23", 10, 3, TestName = "Fractional Part Length Less Than Scale")]
		[TestCase(true, "1", 2, 0, true, TestName = "Positive Number With Only Positive Validator")]
		[TestCase(true, "1.2", 3, 2, true, TestName = "Positive Fractional Number With Only Positive Validator")]
		
		[TestCase(false, null, 5, 4, TestName = "Null String")]
		[TestCase(false, "", 5, 4, TestName = "Empty String")]
		[TestCase(false, "a.12", 5, 4, TestName = "NaN String")]
		[TestCase(false, "1.2.3", 5, 4, TestName = "String With Two Points")]
		[TestCase(false, "123", 2, 0, TestName = "Integer Number Length Greater Than Precision")]
		[TestCase(false, "12.3", 2, 1, TestName = "Fractional Number Length Greater Than Precision")]
		[TestCase(false, "1.23", 3, 1, TestName = "Fractional Part Length Greater Than Scale")]
		[TestCase(false, "-12", 2, 0, TestName = "Negative Number Length Greater Than Precision")]
		[TestCase(false, "+12", 2, 0, TestName = "Positive Number Length Greater Than Precision")]
		[TestCase(false, "-1", 3, 0, true, TestName = "Negative Number With Only Positive Validator")]
		public void TestIsValidNumber(bool isValid, string number, int precision, int scale, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var result = validator.IsValidNumber(number);
			var message = $"number={number}, precision={precision} scale={scale}";

			if (isValid)
				result.Should().BeTrue(message);
			else
				result.Should().BeFalse(message);
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