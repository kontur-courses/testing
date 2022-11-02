using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase("0.0", 17, 2)]
		[TestCase("0", 17, 2)]
		[TestCase("+1.23", 4, 2)]
		[TestCase("-1.23", 4, 2, false)]
		[TestCase("0", 4, 2, false)]
		public void Number_Is_Valid_Test(
			string validatingNumber,
			int precision,
			int scale,
			bool onlyPositive = true)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(validatingNumber)
				.Should()
				.BeTrue();
		}

		[TestCase("0.000", 17, 2)]
		[TestCase("-1.23", 3, 2)]
		[TestCase("00.00", 3, 2)]
		[TestCase("-0.00", 3, 2)]
		[TestCase("a.sd", 3, 2)]
		[TestCase(".0", 3, 2)]
		[TestCase("0.", 3, 2)]
		[TestCase("+1.23", 3, 2, false)]
		[TestCase("+0.00", 3, 2, false)]
		public void Number_Is_Not_Valid_Test(
			string validatingNumber,
			int precision,
			int scale,
			bool onlyPositive = true)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(validatingNumber)
				.Should()
				.BeFalse();
		}

		[TestCase(-1, 2)]
		[TestCase(1, -2)]
		[TestCase(-1, -2)]
		[TestCase(1, 1, true)]
		[TestCase(1, 2, true)]
		public void Should_Throw_ArgumentException(
			int precision,
			int scale,
			bool onlyPositive = false)
		{
			new Action(() =>
					new NumberValidator(precision, scale, onlyPositive)
				)
				.Should()
				.Throw<ArgumentException>();
		}

		[TestCase(3, 2)]
		[TestCase(1, 0)]
		[TestCase(1, 0, false)]
		public void Should_Not_Throw_ArgumentException(
			int precision,
			int scale,
			bool onlyPositive = true)
		{
			new Action(() =>
					new NumberValidator(precision, scale, onlyPositive)
				)
				.Should()
				.NotThrow();
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