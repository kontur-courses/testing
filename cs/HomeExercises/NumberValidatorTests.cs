using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void Test()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			// Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			// Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
		}

		[TestCase(-1, 0, true,TestName = "When precision < 0, onlyPositive don't matter")]
		[TestCase(0, TestName = "When precision == 0, onlyPositive don't matter")]
		[TestCase(2, -1, TestName = "When scale < 0, onlyPositive don't matter")]
		[TestCase(2, 2, false, TestName = "When scale == precision, onlyPositive don't matter")]
		[TestCase(2, 3, true, TestName = "When scale > precision, onlyPositive don't matter")]
		public void СreateValidator_ThrowArgumentException_FlagOnlyPositiveDoesNotMatter(int precision, int scale = 0, 
			bool onlyPositive = false)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
		}
		
		[TestCase("", false,17, 2, 
			TestName = "False when value is empty")]
		[TestCase(null, false,2, 1, 
			TestName = "False when value is null")]
		[TestCase("aaa", false,5, 1, 
			TestName = "False when value consists only of letters")]
		[TestCase("\n\t\r", false,5, 1, 
			TestName = "False when value consists only of special symbols")]
		[TestCase("+aa", false,5, 2, 
			TestName = "False when value consists of + and letters")]
		[TestCase("-aa", false,5, 2, 
			TestName = "False when value consists of - and letters")]
		[TestCase("+1.0aa", false,5, 2, 
			TestName = "False when value consists of +, number and letters at the end")]
		[TestCase("ff+1.0", false,5, 2, 
			TestName = "False when value consists of letters at the beginning, + and number")]
		[TestCase("., -1.0.", false,5, 2, 
			TestName = "False when value consists of white space or other symbols, - and number")]
		[TestCase("-1.0", true,5, 2, 
			TestName = "True when value consists of -, number and . between int and fracPart, onlyPositive is false")]
		[TestCase("+1.0", true,5, 2, 
			TestName = "True when value consists of +, number and . between int and fracPart, onlyPositive is false")]
		[TestCase("-1,0", true,5, 2, 
			TestName = "True when value consists of -, number and , between int and fracPart, onlyPositive is false")]
		[TestCase("-12.00", false,3, 2, 
			TestName = "False when int+fracPart > precision, onlyPositive don't matter")]
		[TestCase("-14.00", false,4, 1, 
			TestName = "False when fracPart > scale, onlyPositive don't matter")]
		[TestCase("+55555.00", true,17, 2, true, 
			TestName = "True when value consists of + and onlyPositive is true")]
		[TestCase("55555.00", true,17, 2, true, 
			TestName = "True when value doesn't consist of + or - and onlyPositive is true")]
		[TestCase("55555.00", true,17, 2, 
			TestName = "True when value doesn't consist of + or - and onlyPositive is false")]
		[TestCase("-55555.00", true,17, 2, 
			TestName = "True when value consists of - and onlyPositive is false")]
		[TestCase("-55555.00", false,17, 2, true,
			TestName = "False when value consists of - and onlyPositive is true")]
		[TestCase("+0.12", true,17, 2, 
			TestName = "True when int+fracPart <= precision and positive is false")]
		[TestCase("+55.010", true,17, 3, true, 
			TestName = "True when int+fracPart < precision and " + "positive is true")]
		public void CheckValidNumber_TrueOrFalse(string value, bool actualResult, 
			int precision, int scale = 0, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			var expectedResult = validator.IsValidNumber(value);
			
			Assert.AreEqual(actualResult, expectedResult);
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