using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidator_PrecisionIsZero_ThrowsException()
		{
			Action action = () => new NumberValidator(0);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_PrecisionIsNegative_ThrowsException()
		{
			Action action = () => new NumberValidator(-1);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_ScaleIsNegative_ThrowsException()
		{
			Action action = () => new NumberValidator(2, -1);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_ScaleEqualToPrecision_ThrowsException()
		{
			Action action = () => new NumberValidator(1, 1);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_ScaleBiggerThanPrecision_ThrowsException()
		{
			Action action = () => new NumberValidator(1, 5);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_PositiveScaleLessThanPrecision_GoesOK()
		{
			Action action2 = () => new NumberValidator(2, 1);
			action2.Should().NotThrow();
		}

		[Test]
		public void IsValidNumber_NullInput_ReturnsFalse()
		{
			new NumberValidator(2).IsValidNumber(null).Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_NonNumberInput_ReturnsFalse()
		{
			new NumberValidator(20).IsValidNumber("this is number(no)").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_PrecisionNumberLessThanActual_ReturnsFalse()
		{
			new NumberValidator(3, 2, true).IsValidNumber("+4.20").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_NumberWithIncorrectSign_ReturnsFalse()
		{
			new NumberValidator(17, 0, true).IsValidNumber("-4.20").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ScaleLessThanActual_ReturnsFalse()
		{
			new NumberValidator(17, 2, true).IsValidNumber("0.000").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_PositiveIntegerNumber_ReturnsTrue()
		{
			new NumberValidator(17, 0, true).IsValidNumber("42").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_NegativeIntegerNumber_ReturnsTrue()
		{
			new NumberValidator(17, 0).IsValidNumber("-42").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_PositiveFloatNumber_ReturnsTrue()
		{
			new NumberValidator(4, 2, true).IsValidNumber("+1.23").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_NegativeFloatNumber_ReturnsTrue()
		{
			new NumberValidator(4, 2).IsValidNumber("-1.23").Should().BeTrue();
		}

		[Test]
		[Timeout(20)]
		public void IsValidNumber_PerformanceIsOK()
		{
			for (var i = 0; i < 500; i++)
				new NumberValidator(17, 2, true).IsValidNumber("42");
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
				throw new ArgumentException("scale must be a non-negative number less than precision");
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