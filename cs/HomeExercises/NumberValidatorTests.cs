using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidatorConstructor_ThrowArgumentException_WhenPrecisionNegative()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
		}

		[Test]
		public void NumberValidatorConstructor_ThrowArgumentException_WhenScaleNegative()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(4, -1, true));
		}

		[Test]
		public void NumberValidatorConstructor_ThrowArgumentException_WhenScaleMorePrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 8, true));
		}

		[Test]
		public void NumberValidatorConstructor_ThrowArgumentException_WhenScaleEqualsToPrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 1, true));
		}

		[TestCase("2.4", 17, 2, TestName = "Valid string")]
		[TestCase("0.1", 2, 1, TestName = "Valid string less then 0")]
		[TestCase("2", 17, 0, TestName = "Valid string without dot")]
		[TestCase("-12.1", 17, 2, TestName = "Valid string with negative number")]
		[TestCase("12.1", 17, 2, true, TestName = "Valid string with positive number when only positive")]
		public void IsValidNumber_Valid(string line, int precision, int scale, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(line).Should().BeTrue();
		}

		[TestCase("00.00", 3, 2, TestName = "Number not valid when to long string")]
		[TestCase("-0.00", 5, 2, true, TestName = "Number not valid when negative value when only positive")]
		[TestCase("+0.00", 3, 2, TestName = "Number not valid when length more at plus sign")]
		[TestCase("0.000", 5, 2, TestName = "Number not valid when length more at non significant zeros")]
		[TestCase("a.sd", 5, 2, TestName = "Number not valid when only letters string")]
		[TestCase("4.44d", 8, 7, TestName = "Number not valid when string with letters")]
		[TestCase(null, 8, 7, TestName = "Number not valid when Null")]
		[TestCase("", 8, 7, TestName = "Number not valid when empty string")]
		public void IsValidNumber_NotValid(string line, int precision, int scale, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(line).Should().BeFalse();
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