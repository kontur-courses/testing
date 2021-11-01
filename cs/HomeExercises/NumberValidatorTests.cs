using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		[TestCase(-1, 1, TestName = "With negative precision")]
		[TestCase(1, -1, TestName = "With negative scale")]
		[TestCase(0, 1, TestName = "With zero precision")]
		[TestCase(1, 2, TestName = "With scale greater than precision")]
        [TestCase(1, 1, TestName = "With scale equals precision")]
		public void NumberValidator_ConstructorThrows(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision, scale);

			act.Should().Throw<ArgumentException>();
		}

		[Test]
		[TestCase(2, 1, true, TestName = "With positive precision")]
		[TestCase(1, 0, false, TestName = "With default values")]
		[TestCase(1, 0, true, TestName = "When only positive is true and default values")]
		public void NumberValidator_ConstructorDoesNotThrows(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);

			act.Should().NotThrow<ArgumentException>();
		}

		[Test]
		[TestCase(10, 0, true, "0", ExpectedResult = true, TestName = "With integer")]
		[TestCase(10, 1, true, "0.0", ExpectedResult = true, TestName = "With real")]
		[TestCase(10, 1, true, "+0.0", ExpectedResult = true, TestName = "With plus at the beginnig")]
		[TestCase(10, 5, false, "+0.0", ExpectedResult = true, TestName = "With positive when not onlyPositive")]
		[TestCase(10, 5, true, null, ExpectedResult = false, TestName = "With null")]
		[TestCase(10, 5, true, "", ExpectedResult = false, TestName = "With empty string")]
		[TestCase(10, 5, true, "+a.b", ExpectedResult = false, TestName = "With not a num")]
		[TestCase(10, 1, true, "0.00", ExpectedResult = false, TestName = "With frac part more than scale")]
		[TestCase(3, 2, true, "00.00", ExpectedResult = false, TestName = "With int and frac parts more than precision")]
		[TestCase(10, 5, true, "-1.23", ExpectedResult = false, TestName = "With minus when onlyPositive")]
		public bool NumberValidator_IsValidNumber_CustomParams(int precision, int scale, bool onlyPositive, string num)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			return validator.IsValidNumber(num);
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