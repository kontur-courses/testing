using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true)]
		[TestCase(0, 2, false)]
		[TestCase(2, -1, true)]
		[TestCase(1, 2, false)]
		public static void AssertThrowsException(int precision, int scale, bool onlyPositive)
		    => Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));

		[TestCase(1, 0, true)]
		public static void AssertDoesNotThrowEcxeption(int precision, int scale, bool onlyPositive)
		    => Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));

		[TestCase(17, 2, true, "0.0")]
		[TestCase(17, 2, true, "0")]
		[TestCase(4, 2, true, "+1.23")]
		public static void AssertNotFalls_OnCorrectValues(int precision, int scale, bool onlyPositive, string value)
		    => Assert.IsTrue(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));

		[TestCase(1, 0, true, "")]
		[TestCase(1, 0, true, "   ")]
		[TestCase(1, 0, true, null)]
		[TestCase(3, 2, true, "00.00")]
		[TestCase(3, 2, true, "-0.00")]
		[TestCase(3, 2, true, "+0.00")]
		[TestCase(3, 2, true, "+1.23")]
		[TestCase(17, 2, true, "0.000")]
		[TestCase(3, 2, true, "-1.23")]
		[TestCase(17, 2, true, "-000.00")]
		[TestCase(3, 2, true, "a.sd")]
		public static void AssertFalls_OnIncorrectValues(int precision, int scale, bool onlyPositive, string value)
		    => Assert.IsFalse(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));        
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
