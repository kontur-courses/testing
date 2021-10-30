using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		/*
		[Test]
		public void Test()
		{
			//Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			//Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			//Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			//Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

			//Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			//Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			//Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			//Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			//Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));

			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			//Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
		}*/

		[Test]
		[TestCase(-1, 2, true, TestName = "Precision Is Negative")]
		public void InitNumberValidator_WhenPrecisionThrow(int precision, int scale, bool onlyPositive)
		{
			Action act = () =>  new NumberValidator(precision, scale, onlyPositive);

			act.Should().Throw<ArgumentException>()
				.WithMessage("precision must be a positive number");
		}

		[Test]
		[TestCase(2, -1, true, TestName = "Scale Is Negative")]
		[TestCase(1, 2, true, TestName = "Scale is More Precision")]
		[TestCase(2, 2, true, TestName = "Scale is Equals Precision")]
		public void InitNumberValidator_WhenScaleThrow(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);

			act.Should().Throw<ArgumentException>()
				.WithMessage("precision must be a non-negative number less or equal than precision");
		}

		[Test]
		[TestCase(17, 2, true, TestName = "InitNumberValidator is OK")]
		[TestCase(1, 0, true, TestName = "Scale is Zero")]
		public void InitNumberValidator_Сorrectly(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);

			act.Should().NotThrow();
		}

		[Test]
		[TestCase(17, 2, true, "0.0", TestName = "Input fractional number")]
		[TestCase(17, 2, true, "0", TestName = "Input integer")]
		[TestCase(4, 2, true, "+1.23", TestName = "Input plus sign")]
		[TestCase(4, 2, false, "-1.23", TestName = "Input Minus sign")]
		public void IsValidNumber_WhenReturnTrue(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
		}

		[Test]
		[TestCase(3, 2, true, "00.00", TestName = "IntPart more precision")]
		[TestCase(5, 2, true, "0.000", TestName = "FracPart more scale")]
		[TestCase(3, 2, true, "a.sd", TestName = "Input not number")]
		[TestCase(3, 2, true, "", TestName = "Input Empty")]
		[TestCase(3, 2, true, " ", TestName = "Input Space")]
		[TestCase(3, 2, true, ".", TestName = "Input Dot")]
		[TestCase(3, 2, true, null, TestName = "Input Null")]
		[TestCase(5, 2, true, "-2", TestName = "Input NegativeNumber")]
		public void IsValidNumber_WhenReturnFalse(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
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