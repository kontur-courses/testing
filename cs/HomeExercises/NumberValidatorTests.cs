using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		[TestCase(1, 0)]
		[TestCase(2, 1)]
		public void Should_Initialize_WhenParameters_Correct(int p, int s)
		{
			Assert.DoesNotThrow(() => new NumberValidator(p, s));
		}

		[Test]
		[TestCase(-1, 0)]
		[TestCase(0, 0)]
		[TestCase(1, -1)]
		[TestCase(2, 4)]
		[TestCase(2, 2)]
		public void Should_ThrowException_WhenParameters_Incorrect(int p, int s)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(p, s));
		}

		[Test]
		[TestCase(10, 2, true, "10")]
		[TestCase(10, 0, true, "10")]
		[TestCase(10, 5, true, "10.10")]
		[TestCase(10, 5, true, "10,00")]
		[TestCase(10, 5, false, "-10.10")]
		[TestCase(10, 0, false, "-0")]
		[TestCase(10, 0, false, "+0")]
		[TestCase(4, 2, true, "00.00")]
		[TestCase(4, 2, false, "+1.23")]
		public void Should_Valid_CorrectNumbers(int p, int s, bool rule, string number)
		{
			Assert.IsTrue(new NumberValidator(p, s, rule).IsValidNumber(number));
		}

		[Test]
		[TestCase(4, 0, true, "25565")]
		[TestCase(4, 1, true, "10.24")]
		[TestCase(4, 2, true, "-1.0")]
		[TestCase(4, 2, false, null)]
		[TestCase(4, 2, false, " 10 . 3 ")]
		[TestCase(4, 2, false, "")]
		[TestCase(3, 2, false, "+1.23")]
		[TestCase(3, 2, false, "-1.23")]
		[TestCase(10, 5, false, "ws.ad")]
		public void Should_NotValid_IncorrectNumbers(int b, int s, bool rule, string number)
		{
			Assert.IsFalse(new NumberValidator(b, s, rule).IsValidNumber(number));
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