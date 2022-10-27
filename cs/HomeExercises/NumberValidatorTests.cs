using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(30, 10, false, null, ExpectedResult = false)]
		[TestCase(30, 10, false, "", ExpectedResult = false)]
		[TestCase(30, 10, false, "    ", ExpectedResult = false)]
		[TestCase(30, 10, false, "abc", ExpectedResult = false)]
		[TestCase(30, 10, false, "0.00?", ExpectedResult = false)]
		[TestCase(30, 10, false, "0,12", ExpectedResult = true)]
		[TestCase(30, 10, false, "0 12", ExpectedResult = false)]
		[TestCase(30, 10, false, "0:12", ExpectedResult = false)]
		[TestCase(30, 10, false, "0-12", ExpectedResult = false)]
		[TestCase(30, 10, false, "0.12.0", ExpectedResult = false)]
		[TestCase(30, 10, false, "aaa.bcd", ExpectedResult = false)]
		[TestCase(17,2, false, "0.0", ExpectedResult = true)]
		[TestCase(17, 2, false, "0", ExpectedResult = true)]
		[TestCase(4, 2, true, "+1.23", ExpectedResult = true)]
		[TestCase(4, 2, true, "-1.23", ExpectedResult = false)]
		[TestCase(3, 2, true, "+1.23", ExpectedResult = false)]
		[TestCase(3, 2, true, "00.00", ExpectedResult = false)]
		[TestCase(3, 2, true, "-0.00", ExpectedResult = false)]
		[TestCase(3, 2, true, "+0.00", ExpectedResult = false)]

		public bool Test_IsValidNumber(int precision, int scale, bool onlyPositive, string number)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number);
		}

		[TestCase(-1,2,true)]
		[TestCase(-1, 2, false)]
		[TestCase(2, 3, false)]
		[TestCase(1, -1, false)]
		[TestCase(1, -1, true)]
		[TestCase(0, 0, true)]
		public void Test_ExceptionOnInitialization_ShouldBeThrownException(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().Throw<ArgumentException>();
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