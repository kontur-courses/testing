using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		class ConstructorTests
		{
			[TestCase(-10, 10, false, TestName = "When precision < 0")]
			[TestCase(0, 10, false, TestName = "When precision = 0")]
			public void Should_ThrowArgumentException_OnIncorrectPrecision(int precision, int scale, bool onlyPositive)
			{
				Action act = () => new NumberValidator(precision, scale, onlyPositive);
				act.Should().Throw<ArgumentException>().WithMessage("precision must be a positive number");
			}
			[TestCase(1, 10, true, TestName = "When scale > precision")]
			[TestCase(10, 10, true, TestName = "When scale = precision")]
			[TestCase(1, -10, false, TestName = "When scale < 0")]
			public void Should_ThrowArgumentException_OnIncorrectScale(int precision, int scale, bool onlyPositive)
			{
				Action act = () => new NumberValidator(precision, scale, onlyPositive);
				act.Should().Throw<ArgumentException>().WithMessage("precision must be a non-negative number less or equal than precision");
			}
		
			[TestCase(10, 2, false, TestName = "When scale > 0")]
			[TestCase(10, 0, false, TestName = "When scale = 0")]
			public void Should_NotThrowArgumentException_OnCorrectArguments(int precision, int scale, bool onlyPositive)
			{
				Action act = () => new NumberValidator(precision, scale, onlyPositive);
				act.Should().NotThrow<ArgumentException>();
			}
		}
		class IsValidNumberTests
		{
			[TestCase(4,2,true, "", TestName = "When string is empty")]
			[TestCase(4,2, true, null, TestName = "When string = null")]
			public void Should_ReturnFalse_When_InputIsNullOrEmpty(int precision, int scale, bool onlyPositive, string value)
			{
				new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
			}
			
			[TestCase(3,2,true, ".11", TestName = "When starts with point")]
			[TestCase(4,2,true, "1.1.1.1", TestName = "When count of points > 1")]
			[TestCase(4,2,true, "a.sd", TestName = "When contains unresolved symbols")]
			[TestCase(2,0,true, "11.", TestName = "When ends with point")]
			[TestCase(1,0,false, "-", TestName = "When contains only sing")]
			[TestCase(3,2,false, "-.11", TestName = "When starts with sing and point")]
			[TestCase(4,2,false, "1.-11", TestName = "When sing isn't ahead")]
			public void Should_ReturnFalse_When_InputDoesNotMatchRegex(int precision, int scale, bool onlyPositive, string value)
			{
				new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
			}

			[TestCase(3,2,false, "11.23", TestName = "When input doesn't contain sign")]
			[TestCase(3,2,false, "+0.00", TestName = "When input contains sign")]
			public void Should_ReturnFalse_When_InputLongerThanPrecision(int precision, int scale, bool onlyPositive, string value)
			{
				new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
			}
			
			[Test]
			public void Should_ReturnFalse_When_FracPartLongerThanScale()
			{
				new NumberValidator(5, 2, true).IsValidNumber("11.111").Should().BeFalse();
			}
			
			[Test]
			public void Should_ReturnTrue_When_FlagOnlyPositiveIsFalseAndInputIsPositive()
			{
				new NumberValidator(5, 2, false).IsValidNumber("11.11").Should().BeFalse();
			}
			
			[Test]
			public void Should_ReturnFalse_When_FlagOnlyPositiveIsTrueAndInputIsNegative()
			{
				new NumberValidator(5, 2, true).IsValidNumber("-11.11").Should().BeFalse();
			}
			
			[TestCase(5,2,false, "-10,15", TestName = "When num is negative and \",\" instead \".\"")]
			[TestCase(2,0,false, "-0", TestName = "When num = -0")]
			[TestCase(17,2,true, "0.0", TestName = "When input shorter than precision and scale > frac part")]
			[TestCase(17,2,true, "0", TestName = "When input shorter than precision, scale > 0 and frac part is missing")]
			public void Should_ReturnTrue_When_InputIsCorrect(int precision, int scale, bool onlyPositive, string value)
			{
				new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
			}
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