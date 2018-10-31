using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1)]
		[TestCase(0)]
		[TestCase(-3)]
		[TestCase(-100)]
		public void NumberValidatorConstructorWillThrowExceptionIfPrecisionLessOrEqualZero(int precision)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, 2, true));
		}

		[TestCase(-1)]
		[TestCase(-23)]
		public void NumberValidatorConstructorWillThrowExceptionIfScaleLessThenZero(int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(10, scale, true));
		}
		
		[Test]
		public void NumberValidatorConstructorWillNotThrowExceptionIfScaleEqualsToZero()
		{
			Assert.DoesNotThrow(() => new NumberValidator(10,0,false));
		}

		[TestCase(10, 11)]
		[TestCase(10, 10)]
		[TestCase(1, 2)]
		[TestCase(1, 10)]
		public void NumberValidatorConstructorWillThrowExceptionIfScaleGraterThanPrecision(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, true));
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		[TestCase("     ")]
		public void NumberValidatorWillFailOnNullOrEmptyInput(string input)
		{
			Assert.IsFalse(new NumberValidator(10,9,false).IsValidNumber(input));
		}
		
		[TestCase("-23.1")]
		[TestCase("-10.1")]
		[TestCase("-11.1")]
		[TestCase("-123.23")]
		public void PositiveNumberValidatorWillFailOnNegativeInput(string input)
		{
			Assert.IsFalse(new NumberValidator(10,9,true).IsValidNumber(input));
		}

		[TestCase("asd.ddt")]
		[TestCase("WierdChinesSymbols.exe")]
		[TestCase("1111a11.322")]
		public void NumberValidatorWillFailOnNotNumberInput(string input)
		{
			Assert.IsFalse(new NumberValidator(100,99,false).IsValidNumber(input));
		}

		[TestCase("0.23")]
		[TestCase("0.000")]
		[TestCase("0.0")]
		public void NumberValidatorWillFailWithScaleOverflow(string input)
		{
			Assert.IsFalse(new NumberValidator(10,0,false).IsValidNumber(input));
		}
		
		[TestCase("0000")]
		[TestCase("1234566")]
		[TestCase("+1")]
		public void NumberValidatorWillFailWithPrecisionOverflow(string input)
		{
			Assert.IsFalse(new NumberValidator(1,0,false).IsValidNumber(input));
		}

		[TestCase("+000")]
		[TestCase("-000.0")]
		[TestCase("+123.32")]
		[TestCase("+222.23")]
		public void NumberValidatorCountSignAsPrecision(string input)
		{
			Assert.IsFalse(new NumberValidator(3,2,false).IsValidNumber(input));
		}

		[TestCase("++32.0")]
		[TestCase("+-32.0")]
		[TestCase("-+32.0")]
		public void NumberValidatorFailIfInputHasMoreThanTwoSigns(string input)
		{
			Assert.IsFalse(new NumberValidator(30,29).IsValidNumber(input));
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