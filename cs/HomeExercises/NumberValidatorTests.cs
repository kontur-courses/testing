using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1)]
		[TestCase(2, -1)]
		[TestCase(2, 3)]
		[Test]
		public void TestThrowsExceptions(int precision, int scale = 0, bool onlyPositive = false)
		{
			FluentActions.Invoking(() =>
				new NumberValidator(precision, scale, onlyPositive)).Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, false, null, false)]
		[TestCase(1, 0, false, "", false)]
		[TestCase(1, 0, false, "        ", false)]
		[TestCase(2, 0, false, " 1", false)]
		[TestCase(2, 0, false, "1 ", false)]
		[TestCase(2, 0, false, "/1", false)]
		[TestCase(2, 0, false, "1+", false)]
		[TestCase(5, 0, false, "12r34", false)]
		[TestCase(5, 0, false, "12 34", false)]
		[TestCase(2, 0, false, "111", false)]
		[TestCase(2, 0, false, "-1.1", false)]
		[TestCase(3, 1, false, "2.22", false)]
		[TestCase(3, 1, true, "-1.1", false)]
		[TestCase(3, 1, false, "-1.1", true)]
		[TestCase(7, 4, true, "000,0000", true)] // Проверка на запятую
		[TestCase(10, 4, true, "000.0000", true)]
		[TestCase(6, 2, false, "+000.00", true)]
		[TestCase(6, 2, false, "-000.00", true)]
		[Test]
		public void TestFunctional(int precision, int scale, bool onlyPositive,
			string? value, bool result)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value!).Should().Be(result);
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