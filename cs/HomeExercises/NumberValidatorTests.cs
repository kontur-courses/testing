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
		public void Should_Valid_Integer()
		{
			Assert.IsTrue(new NumberValidator(10, 2, true).IsValidNumber("10"));
		}

		[Test]
		public void Should_Valid_OnlyInteger()
		{
			Assert.IsTrue(new NumberValidator(10, 0, true).IsValidNumber("10"));
		}
		
		[Test]
		public void Should_Valid_Fractional()
		{
			Assert.IsTrue(new NumberValidator(16, 8, true).IsValidNumber("10.5060"));
		}

		[Test]
		public void Should_NotValid_ToLongIntegerPart()
		{
			Assert.IsFalse(new NumberValidator(4, 0, true).IsValidNumber("10240"));
		}

		[Test]
		public void Should_NotValid_ToLongFactionalPart()
		{
			Assert.IsFalse(new NumberValidator(4, 1, true).IsValidNumber("10.24"));
		}

		[Test]
		public void Should_Be_CultureIndependent()
		{
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0,0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
		}

		[Test]
		public void Should_Valid_NotSignificantZeros()
		{
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("00.00"));
		}

		[Test]
		public void Should_NotValid_NullOrWhiteSpace()
		{
			var numberValidator = new NumberValidator(3, 2, true);
			Assert.IsFalse(numberValidator.IsValidNumber("  "));
			Assert.IsFalse(numberValidator.IsValidNumber(null));
		}

		[Test]
		public void Should_Valid_OnlyPositive()
		{
			var numberValidator = new NumberValidator(3, 2, true);
			Assert.IsTrue(numberValidator.IsValidNumber("1.0"));
			Assert.IsFalse(numberValidator.IsValidNumber("-1.0"));
		}

		[Test]
		public void Should_Count_PrecisionWithSign()
		{
			var numberValidator = new NumberValidator(3, 2, false);
			Assert.IsFalse(numberValidator.IsValidNumber("+1.23"));
			Assert.IsFalse(numberValidator.IsValidNumber("-1.23"));

			numberValidator = new NumberValidator(4, 2, false);
			Assert.IsTrue(numberValidator.IsValidNumber("+1.23"));
			Assert.IsTrue(numberValidator.IsValidNumber("-1.23"));
		}

		[Test]
		public void Should_Valid_OnlyNumbers()
		{
			Assert.IsFalse(new NumberValidator(10, 4, false).IsValidNumber("a.sd"));
		}

		[Test]
		public void Should_Valid_PositiveOrNegativeZero()
		{
			var numberValidator = new NumberValidator(4, 2, false);
			Assert.IsTrue(numberValidator.IsValidNumber("+0"));
			Assert.IsTrue(numberValidator.IsValidNumber("-0"));
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