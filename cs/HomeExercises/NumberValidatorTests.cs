using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestFixture]
		public class NumberValidatorConstructor_Should
		{
			[TestCase(-1, 2, TestName = "Throw Exteption With Negative Precision")]
			[TestCase(0, 2, TestName = "Throw Exteption With Zero Precision")]
			[TestCase(1, -2, TestName = "Throw Exteption With Negative Scale")]
			[TestCase(1, 2, TestName = "Throw Exteption With Scale Greater Than Precision")]
			public void test(int precision, int scale)
			{
				Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
			}
		}

		[TestFixture(TestName = "NumberValidator_IsValidNumber_Should")]
		public class NumberValidator_IsValidNumber_Should
		{
			[TestCase(2, 1, true, null, TestName = "Be false on null value", ExpectedResult = false)]
			[TestCase(2, 1, true, "", TestName = "Be false on empty value", ExpectedResult = false)]
			[TestCase(2, 1, true, "testData", TestName = "Be false on incorrect value", ExpectedResult = false)]
			[TestCase(2, 0, true, "111", TestName = "Be false when precision lower than value", ExpectedResult = false)]
			[TestCase(3, 1, true, "1.11", TestName = "Be false when scale slwer than value", ExpectedResult = false)]
			[TestCase(3, 0, true, "+111", TestName = "Be false when Precision Lower Than Value With Sign",
				ExpectedResult = false)]
			[TestCase(2, 0, true, "-1", TestName = "Be false With Negative Value and Positive Flag",
				ExpectedResult = false)]
			[TestCase(2, 0, false, "-1", TestName = "Be true With Negative Value and Not positive Flag",
				ExpectedResult = true)]
			[TestCase(3, 2, false, "1.1", TestName = "Be true With Correct Value", ExpectedResult = true)]
			public bool test(int precision, int scale, bool flag, string value)
			{
				return new NumberValidator(precision, scale, flag).IsValidNumber(value);
			}
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