using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		//Не знаю, как будет лучше: первый тест или остальные. Конечно, выглядят  они некрасиво, зато информативнее некуда
		[TestCase(17, 2, true, "0.0", ExpectedResult = true, TestName = "Should be true when correct input")]
		[TestCase(17, 2, true, "0", ExpectedResult = true, TestName = "Precision: 17; Scale: 2; Positive: true; value: 0")]
		[TestCase(3, 2, true, "00.00", ExpectedResult = false, TestName = "Precision: 3; Scale: 2; Positive: true; value: 00.00")]
		[TestCase(3, 2, true, "-0.00", ExpectedResult = false, TestName = "Precision: 3; Scale: 2; Positive: true; value: -0.00")]
		[TestCase(3, 2, true, "+0.00", ExpectedResult = false, TestName = "Precision: 3; Scale: 2; Positive: true; value: +0.00")]
		[TestCase(4, 2, true, "+1.23", ExpectedResult = true, TestName = "Precision: 4; Scale: 2; Positive: true; value: +1.23")]
		[TestCase(3, 2, true, "+1.23", ExpectedResult = false, TestName = "Precision: 3; Scale: 2; Positive: true; value: +1.23")]
		[TestCase(17, 2, true, "0.000", ExpectedResult = false, TestName = "Precision: 17; Scale: 2; Positive: true; value: 0.000")]
		[TestCase(3, 2, true, "-1.23", ExpectedResult = false, TestName = "Precision: 3; Scale: 2; Positive: true; value: -1.23")]
		[TestCase(3, 2, true, "a.sd", ExpectedResult = false, TestName = "Precision: 3; Scale: 2; Positive: true; value: a.sd")]
		[TestCase(4, 2, false, "-1.23", ExpectedResult = true, TestName = "Precision: 4; Scale: 2; Positive: false; value: -1.23")]
		[TestCase(4, 0, false, "-1.23", ExpectedResult = false, TestName = "Precision: 4; Scale: 0; Positive: false; value: -1.23")]
		[TestCase(4, 0, false, "1.23", ExpectedResult = false, TestName = "Precision: 4; Scale: 0; Positive: false; value: 1.23")]
		[TestCase(4, 1, false, "1.", ExpectedResult = false, TestName = "Precision: 4; Scale: 1; Positive: false; value: 1.")]
		public bool NumberValidation(int precision, int scale, bool onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			return validator.IsValidNumber(value);
		}
		
		[Test]
		public void ConstructorShouldThrowExceptionWhenPrecisionNonPositive()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1));
			Assert.Throws<ArgumentException>(() => new NumberValidator(0));
		}

		[Test]
		public void ConstructorShouldThrowExceptionWhenPrecisionPositive()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1));
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