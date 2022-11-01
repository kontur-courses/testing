using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[Test]
		[TestCase("0.0", true, 17, 2, true)]
		[TestCase("0", true, 17, 2, true)]
		[TestCase("0.000", false, 17, 2, true)]
		[TestCase("+1.23", true, 4, 2, true)]
		[TestCase("+1.23", false, 3, 2, true)]
		[TestCase("-1.23", false, 3, 2, true)]
		[TestCase("00.00", false, 3, 2, true)]
		[TestCase("-0.00", false, 3, 2, true)]
		[TestCase("+0.00", false, 3, 2, true)]
		[TestCase("a.sd", false, 3, 2, true)]
		[TestCase(".0", false, 3, 2, true)]
		[TestCase("0.", false, 3, 2, true)]
		public void Test(
			string validatingNumber, 
			bool isValid, 
			int precision, 
			int scale = 0, 
			bool onlyPositive = false)
		{
			Assert.AreEqual(isValid, new NumberValidator(precision, scale, onlyPositive).IsValidNumber(validatingNumber));
		}

		[Test]
		[TestCase(-1, 2, false)]
		[TestCase(1, -2, false)]
		[TestCase(-1, -2, false)]
		[TestCase(1, 1, true)]
		[TestCase(1, 2, true)]
		public void Should_Throw_ArgumentException(
			int precision, 
			int scale = 0, 
			bool onlyPositive = false)
		{
			new Action(() => 
				new NumberValidator(precision, scale, onlyPositive)
				)
				.Should()
				.Throw<ArgumentException>();
		}
		
		[Test]
		[TestCase(1, 0, true)]
		[TestCase(1, 0, false)]
		[TestCase(3, 2, true)]
		public void Should_Not_Throw_ArgumentException(
			int precision, 
			int scale = 0, 
			bool onlyPositive = false)
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