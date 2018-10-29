using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
	public class NumberValidatorTests
	{
		[TestCase(true)]
		[TestCase(false)]
		public void Throws_WhenPrecision_IsNegative(bool positiveOnly)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, positiveOnly));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void Throws_WhenPrecision_IsZero(bool positiveOnly)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(0, 2, positiveOnly));
		}


		[TestCase(true)]
		[TestCase(false)]
		public void DoesNotThrow_WhenPrecision_IsPositive(bool positiveOnly)
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, positiveOnly));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void Throws_WhenScale_IsNegative(bool positiveOnly)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, -2, positiveOnly));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void Throws_WhenScale_IsGreaterThanPrecision(bool positiveOnly)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(5, 8, positiveOnly));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void Validates_Numbers_WithLessSymbols(bool positiveOnly)
		{
			Assert.IsTrue(new NumberValidator(17, 2, positiveOnly).IsValidNumber("0.0"));
		}


		[TestCase(true)]
		[TestCase(false)]
		public void Validates_Integer_Numbers(bool positiveOnly)
		{
			Assert.IsTrue(new NumberValidator(17, 2, positiveOnly).IsValidNumber("0"));
		}

		[TestCase("000.00")]
		[TestCase("0.000")]
		[TestCase("+00.00")]
		[TestCase("-00.00")]
		public void DoesNot_Validate_Numbers_WithTooManySymbols(string number)
		{
			Assert.IsFalse(new NumberValidator(4, 2).IsValidNumber(number));
		}

		[TestCase("-0.0")]
		[TestCase("-1.0")]
		[TestCase("-1.23")]
		public void PosOnly_DoesNot_Validate_Negative_Numbers(string number)
		{
			Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber(number));
		}

		[TestCase("0.0")]
		[TestCase("+0.0")]
		[TestCase("1.0")]
		[TestCase("+1.23")]
		public void PosOnly_Validates_NonNegative_Numbers(string number)
		{
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber(number));
		}

		[TestCase("a.sd")]
		public void DoesNot_Validate_NonNumber(string number)
		{
			Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber(number));
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