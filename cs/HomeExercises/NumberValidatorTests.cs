using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void Constructor_ShouldThrowException_WhenPrecisionIsNegative()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
		}

		[Test]
		public void Constructor_ShouldThrowException_WhenScaleLargerThanPrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
		}

		[Test]
		public void Constructor_CreatesNewValidator()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1));
		}

		[TestCase(3, 2, true, "a.sd", false, TestName = "RegexTest wrong symbols in value")]
		[TestCase(3, 1, true, "+.0", false, TestName = "RegexTest empty part")]
		[TestCase(17, 2, true, "", false, TestName = "Empty value Test")]
		[TestCase(17, 2, true, null, false, TestName = "Null value Test")]
		[TestCase(3, 2, true, "00.00", false, TestName = "Precision Overflow Test")]
		[TestCase(5, 2, true, "9.111", false, TestName = "Scale Overflow Test")]
		[TestCase(3, 2, true, "-0.2", false, TestName = "Negative value and only-positive flag Test")]
		[TestCase(15, 8, true, "4.24242", true, TestName = "Normal test")]
		[TestCase(5, 1, false, "-122,2", true, TestName = "Normal test with sign")]
		public void TestValidator(int precision, int scale, bool onlyPositive, string value, bool expected)
		{
			(new NumberValidator(precision, scale, onlyPositive)).IsValidNumber(value).Should().Be(expected, 
													"[precision: {0}, scale: {1}, onlyPositive: {2}, value: {3}]", precision, scale, onlyPositive, value);
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