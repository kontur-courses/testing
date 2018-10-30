using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1, 0, Description = "Zero scale")]
        [TestCase(2, 1, Description = "Scale less than precision")]

        public void Constructor_CorrectParameters_ToCreate(int precision, int scale)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale));
		}
		
		[TestCase(-1, 0, Description = "Negative precision")]
		[TestCase(0, 0, Description = "Zero precision")]
		[TestCase(1, -1, Description = "Negative scale")]
		[TestCase(2, 4, Description = "Scale is greater than precision")]
		[TestCase(2, 2, Description = "Scale is equal to precision")]
		public void Constructor_IncorrectParameters_ThrowsException(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
		}
		
		[TestCase(10, 2, true, "10", Description = "Integer number")]
        [TestCase(10, 0, true, "10", Description = "Integer number with 0 scale")]
        [TestCase(10, 5, true, "10.10", Description = "Fractional number")]
        [TestCase(10, 5, true, "10,00", Description = "Comma instead of point")]
        [TestCase(10, 5, false, "-10.10", Description = "Negative number")]
		[TestCase(10, 5, false, "+10.10", Description = "Plus at the beginning")]
        [TestCase(10, 0, false, "-0", Description = "Negative zero")]
        [TestCase(10, 0, false, "+0", Description = "Positive zero")]
        [TestCase(4, 2, true, "00.00", Description = "Non-significant Zeros")]

        public void IsValidNumber_CorrectNumber_True(int precision, int scale, bool onlyPositive, string number)
		{
			Assert.IsTrue(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number));
		}
		
		[TestCase(4, 0, true, "25565", Description = "Too long integer number")]
        [TestCase(4, 1, true, "10.24", Description = "Too long fractional part")]
        [TestCase(4, 2, true, "-1.0", Description = "Only positive number allowed")]
        [TestCase(4, 2, false, null, Description = "Null is not correct")]
        [TestCase(4, 2, false, " 10 . 3 ", Description = "Spaces not allowed")]
        [TestCase(4, 2, false, "", Description = "Empty string")]
        [TestCase(3, 2, false, "+1.23", Description = "Too long integer part with plus")]
        [TestCase(3, 2, false, "-1.23", Description = "Too long integer part with minus")]
        [TestCase(10, 5, false, "ws.ad", Description = "Only numbers allowed")]
        [TestCase(10, 5, false, "100%", Description = "Percentage isn't correct number")]
        [TestCase(10, 5, false, "2/3", Description = "Non decimal fraction")]
        [TestCase(10, 5, false, ".0", Description = "No integer part")]
        [TestCase(10, 5, false, "0.", Description = "No fractional part with point", TestName = "fsd")]
        [TestCase(10, 5, false, "0b1001", Description = "Binary number")]
        public void IsValidNumber_IncorrectNumber_False(int b, int scale, bool onlyPositive, string number)
		{
			Assert.IsFalse(new NumberValidator(b, scale, onlyPositive).IsValidNumber(number));
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