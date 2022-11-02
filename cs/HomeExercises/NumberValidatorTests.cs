using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(3, 0, true, "123",ExpectedResult = true, TestName = "number without fractional part")]
		[TestCase(3, 2, true, "00.00", ExpectedResult = false, TestName = "precision cannot be less than fractionPart + intPart")]
		[TestCase(3, 2, true, null, ExpectedResult = false, TestName = "Null value")]
		[TestCase(3, 2, true,  "", ExpectedResult = false, TestName = "Empty string")]
		[TestCase(3, 2, true, " ", ExpectedResult = false, TestName = "WhiteSpace")]
		[TestCase(4, 2, true, "ab.cs", ExpectedResult = false, TestName = "string does not match Regex")]
		[TestCase(4, 2, true, "0.000", ExpectedResult = false, TestName = "scale cannot be less than fractionPart")]
		[TestCase(3, 1, true, "-1.0", ExpectedResult = false, TestName = "value cannot be negative if onlyPositive = true")]
		[TestCase(3, 1, false, "-1.0", ExpectedResult = true, TestName = "symbol - are part of precision")]
		[TestCase(3, 1, false, "+1.0", ExpectedResult = true, TestName = "symbol + are part of precision")]
		[TestCase(5, 3, true, "11.03", ExpectedResult = true, TestName = "validation test with correct parameters")]
		[TestCase(5, 3, true, "11,03" , ExpectedResult = true, TestName = "we can use a comma to separate fraction and integer parts")]

		public bool isValidNumber_Test(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			return numberValidator.IsValidNumber(value);
		}

		[TestCase(-1, 0, TestName = "throws when precision is negative")]
		[TestCase(0, 0, TestName = "throws when precision is zero")]
		[TestCase(2, 3, TestName = "throws when scale greater than precision")]
		[TestCase(2, -1, TestName = "throws when scale is non - positive")]
		public void NumberValidatorConstructor_Fail(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);

			action.Should().Throw<ArgumentException>();
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