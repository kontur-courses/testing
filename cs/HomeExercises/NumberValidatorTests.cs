using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[Test]
		public void TestExceptions()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		}

		[Test]
		[TestCase(17, 2, true, "0.0", true)]
		[TestCase(17, 2, true, "0", true)]
		[TestCase(17, 2, true, "00.00", true)]
		[TestCase(4, 2, true, "+1.23", true)]
		[TestCase(3, 2, true, "+1.23", false)]
		[TestCase(3, 2, true, "-1.23", false)]
		[TestCase(3, 2, true, "00.00", false)]
		[TestCase(3, 2, true, "a.sd", false)]
		[TestCase(3, 2, true, "-0.00", false)]
		[TestCase(3, 2, true, "+0.00", false)]
		[TestCase(3, 2, true, " 1.23", false)]
		[TestCase(3, 2, true, "1. 23", false)]
		[TestCase(3, 2, true, "1.23 ", false)]
		[TestCase(3, 2, true, null, false)]
		[TestCase(3, 2, true, " ", false)]
		[TestCase(3, 1, true, "1.24", false)]
		[TestCase(2, 2, true, "1.24", false)]
		[TestCase(4, 2, true, "-1.24", false)]
		public void TestNumbers(int precision, int scale, bool onlyPositive, string numString, bool expected)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numString).Should().Be(expected);
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
			if (scale < 0 || scale > precision)
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