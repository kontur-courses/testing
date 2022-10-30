using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void IsValidNumber_CountMinus_False()
		{
			new NumberValidator(3, 2, true).IsValidNumber("-0.00")
				.Should().BeFalse("валидатор суммирует - к длине ");
		}
		[Test]
		public void IsValidNumber_CountPlus_False()
		{
			new NumberValidator(3, 2, true).IsValidNumber("+0.00")
				.Should().BeFalse("валидатор суммирует + к длине ");
		}
		[Test]
		public void IsValidNumber_NegativeNumWhenPositiveValidator_False()
		{
			new NumberValidator(4, 2, true).IsValidNumber("-0.00")
				.Should().BeFalse("валидатор принимает положительные числа");
		}
		
		[Test]
		public void IsValidNumber_NumberWithMinus_True()
		{
			new NumberValidator(4, 2).IsValidNumber("-1.23")
				.Should().BeTrue("число с минусом, общая длина без точки равна precision");
		}
		[Test]
		public void IsValidNumber_NumberWithPlus_True()
		{
			new NumberValidator(4, 2, true).IsValidNumber("+1.23")
				.Should().BeTrue("число с плюсом, общая длина без точки равна precision");
		}
		
		[Test]
		public void IsValidNumber_IntegerNumber_True()
		{
			new NumberValidator(17, 2, true).IsValidNumber("0")
				.Should().BeTrue("целое число, общая длина < precision");
		}
		[Test]
		public void IsValidNumber_ThereIsFracPart_True()
		{
			new NumberValidator(17, 2, true).IsValidNumber("0.0")
				.Should().BeTrue("общая длина без точки < precision, дробная часть < scale");

		}
		[Test]
		public void IsValidNumber_FracPartMoreThanScale_False()
		{
			new NumberValidator(17, 2, true).IsValidNumber("0.000")
				.Should().BeFalse("длина дробной части больше scale");

		}
		[Test]
		public void IsValidNumber_NotRegexPattern_False()
		{
			new NumberValidator(3, 2, true).IsValidNumber("a.sd")
				.Should().BeFalse("не соответствует формату числа");

		}
		
		[Test]
		public void IsValidNumber_AllLenghtMoreThanPrecision_False()
		{
			new NumberValidator(3, 2, true).IsValidNumber("00.00")
				.Should().BeFalse("общая длина без точки больше precision");

		}
		
		[Test]
		public void IsValidNumber_EmptyString_False()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber(""));
			
		}
		
		[Test]
		public void IsValidNumber_NullString_False()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber(null));
			
		}
		

		[Test]
		public void CreateNV_NegativePrecision_ArgumentException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
		}
		
		[Test]
		public void CreateNV_PrecisionIsZero_ArgumentException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(0, 2, true));
		}
		
		[Test]
		public void CreateNV_NegativeScale_ArgumentException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(5, -1, true)); 
		}
		
		[Test]
		public void CreateNV_ScaleMoreThanPrecision_ArgumentException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
			
		}

		[Test]
		public void CreateNV_DoesNotThrowException()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
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