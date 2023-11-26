﻿using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void Fails_NegativePrecision()
		{
			var action1 = new Func<NumberValidator>(() => new NumberValidator(-1, 2, true));
			action1.Should().Throw<ArgumentException>();

			var action2 = new Func<NumberValidator>(() => new NumberValidator(-1, 2));
			action2.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NotFails_WhenFine()
		{
			var action = new Func<NumberValidator>(() => new NumberValidator(1));
			action.Should().NotThrow();
		}

		[Test]
		public void Fails_NegativeScale()
		{
			var action = new Func<NumberValidator>(() => new NumberValidator(17, -1, true));
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void Fails_ScaleGreaterOrEqualThanPrecision()
		{
			var action1 = new Func<NumberValidator>(() => new NumberValidator(3, 4, true));
			action1.Should().Throw<ArgumentException>();
			
			var action2 = new Func<NumberValidator>(() => new NumberValidator(10, 10, true));
			action2.Should().Throw<ArgumentException>();
		}
		
		[Test]
		public void BadRegex()
		{
			new NumberValidator(17, 2, true).IsValidNumber("ы").Should().BeFalse();
		}
		
		[Test]
		public void NullString()
		{
			new NumberValidator(17, 2, true).IsValidNumber(null!).Should().BeFalse();
		}
		
		[Test]
		public void EmptyString()
		{
			new NumberValidator(17, 2, true).IsValidNumber("").Should().BeFalse();
		}
		
		[Test]
		public void MinusAndOnlyPositive()
		{
			new NumberValidator(17, 2, true).IsValidNumber("-0.0").Should().BeFalse();
		}
		
		[Test]
		public void FracPartLongerThenScale()
		{
			new NumberValidator(17, 0, true).IsValidNumber("0.0").Should().BeFalse();
			new NumberValidator(17, 2, true).IsValidNumber("0.000").Should().BeFalse();
		}
		
		[Test]
		public void SymbolsMoreThenPrecision()
		{
			new NumberValidator(2, 1, true).IsValidNumber("+0.0").Should().BeFalse();
			new NumberValidator(2, 1, true).IsValidNumber("-0.0").Should().BeFalse();
			new NumberValidator(2, 1, true).IsValidNumber("00.0").Should().BeFalse();
			new NumberValidator(4, 3, true).IsValidNumber("00.000").Should().BeFalse();
			new NumberValidator(2, 0, true).IsValidNumber("189").Should().BeFalse();
		}
		
		[Test]
		public void CorrectValidation()
		{
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
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