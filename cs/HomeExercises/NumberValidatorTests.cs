using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(17, 2, true, "0.0", ExpectedResult = true,
			TestName = "Zero with specified precision", Category = "zero precision")]		
		[TestCase(17, 2, true, "0,0", ExpectedResult = true,
			TestName = "Zero with specified by comma precision", Category = "zero precision")]
		[TestCase(17, 2, true, "0", ExpectedResult = true,
			TestName = "Zero without precision", Category = "zero precision")]
		
		[TestCase(17, 2, true, "01", ExpectedResult = true, 
			TestName = "Leading zeros", Category = "leading/trailing zeros")]
		[TestCase(17, 2, true, "00.10", ExpectedResult = true, 
			TestName = "Trailing zeros", Category = "leading/trailing zeros")]
		[TestCase(2, 1, true, "0.10", ExpectedResult = true, 
			TestName = "Trailing zeros doesn't affect on precision", Category = "leading/trailing zeros")]
		[TestCase(1, 0, true, "01", ExpectedResult = true, 
			TestName = "Leading zeros doesn't affect on precision", Category = "leading/trailing zeros")]
		
		[TestCase(17, 2, true, "1.23", ExpectedResult = true, 
			TestName = "Number with max scale", Category = "boundary limit")]
		[TestCase(4, 0, true, "1234", ExpectedResult = true, 
			TestName = "Number with max precision", Category = "boundary limit")]
		[TestCase(4, 2, true, "12.34", ExpectedResult = true, 
			TestName = "Number with max precision and max scale", Category = "boundary limit")]
		
		[TestCase(1, 0, true, "11", ExpectedResult = false, 
			TestName = "Actual number precision more then specified precision", Category = "out of boundary number")]
		[TestCase(17, 2, true, "1.234", ExpectedResult = false, 
			TestName = "Actual number scale more then specified scale", Category = "out of boundary number")]
		
		[TestCase(17, 2, false, "+0", ExpectedResult = true, 
			TestName = "Positive signed number with false onlyPositive parameter", Category = "onlyPositive cases")]
		[TestCase(17, 2, true, "+0", ExpectedResult = true, 
			TestName = "Positive signed number with true onlyPositive parameter", Category = "onlyPositive cases")]
		public bool Should_CorrectParse_OnCorrectData(int precision, int scale, bool isOnlyPositive, string input)
		{
			return new NumberValidator(precision, scale, isOnlyPositive).IsValidNumber(input);
		}

		[TestCase(17, 2, false, "-0", ExpectedResult = false, 
			TestName = "Negative signed number with false onlyPositive parameter", Category = "onlyPositive cases")]
		[TestCase(17, 2, true, "-0", ExpectedResult = false, 
			TestName = "Negative signed number with true onlyPositive parameter", Category = "onlyPositive cases")]
		
		[TestCase(17, 2, false, "++0", ExpectedResult = false,
			TestName="double positive signed number", Category = "multiple signs")]
		[TestCase(17, 2, false, "--0", ExpectedResult = false,
			TestName="double negative signed number", Category = "multiple signs")]
		[TestCase(17, 2, false, "+-0", ExpectedResult = false,
			TestName="double signed number with mixed signs", Category = "multiple signs")]
		
		[TestCase(17, 2, false, "0.,1", ExpectedResult = false,
			TestName="dot and comma separated numbers", Category = "border cases")]
		[TestCase(17, 2, false, "0.", ExpectedResult = false,
			TestName = "Number with dot but without scale", Category = "border cases")]
		[TestCase(17, 2, false, "abc", ExpectedResult = false,
			TestName = "Not decimal string for parsing", Category = "border cases")]
		[TestCase(17, 2, false, "abc.1", ExpectedResult = false,
			TestName = "Not decimal string mixed with decimal for parsing")]
		[TestCase(17, 2, false, "", ExpectedResult = false,
			TestName = "Empty string for parsing", Category = "border cases")]
		[TestCase(17, 2, false, null, ExpectedResult = false,
			TestName = "Null string for parsing", Category = "border cases")]
		public bool Should_CorrectParse_OnInvalidData(int precision, int scale, bool isOnlyPositive, string input)
		{
			return new NumberValidator(precision, scale, isOnlyPositive).IsValidNumber(input);
		}

		[Test]
		public void Should_ThrowArgumentException_OnNegativePrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
		}
		
		[Test]
		public void Should_ThrowArgumentException_OnNegativeScale()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(2, -1, true));
		}
		
		[Test]
		public void Should_ThrowArgumentException_OnPrecisionEqualThanScale()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 1, true));
		}
		
		[Test]
		public void Should_ThrowArgumentException_OnPrecisionLessThanScale()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
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
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по
			// телекоммуникационным каналам связи: Формат числового значения указывается в виде N(m.к), где m –
			// максимальное количество знаков в числе, включая знак (для отрицательного числа), целую и дробную часть
			// числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. Если число
			// знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

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