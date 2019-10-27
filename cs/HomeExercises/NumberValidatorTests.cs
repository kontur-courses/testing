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
		public void NumberValidator_PrecisionShouldBePositive()
		{
			Action action = () => new NumberValidator(0);
			action.Should().Throw<ArgumentException>();
			action = () => new NumberValidator(-1);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_ScaleShouldBeNonNegative()
		{
			Action action = () => new NumberValidator(2, -1);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_ScaleShouldBeLessThanPrecision()
		{
			Action action = () => new NumberValidator(1, 5);
			action.Should().Throw<ArgumentException>();
			Action action1 = () => new NumberValidator(1, 1);
			action1.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_PositiveScaleLessThanPrecision_GoesOK()
		{
			Action action2 = () => new NumberValidator(1, 0);
			action2.Should().NotThrow();
		}

		[Test]
		public void IsValidNumber_FalseWithNullInput()
		{
			new NumberValidator(2).IsValidNumber(null).Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_FalseWithNonNumberInput()
		{
			new NumberValidator(20).IsValidNumber("kajsdn.ajnqwk").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_FalseWithPrecisionNumberLessThanActual()
		{
			new NumberValidator(2).IsValidNumber("0.000").Should().BeFalse();
			new NumberValidator(3, 2, true).IsValidNumber("+1.23").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_FalseWithIncorrectSign()
		{
			new NumberValidator(2, 0, true).IsValidNumber("-0.00").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_FalseWithScaleLessThanActual()
		{
			new NumberValidator(17, 2, true).IsValidNumber("0.000").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_IntegerNumberGoesOK()
		{
			new NumberValidator(17, 2, true).IsValidNumber("0").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_SimpleFloatNumberGoesOK()
		{
			new NumberValidator(4, 2, true).IsValidNumber("+1.23").Should().BeTrue();
			new NumberValidator(4, 2).IsValidNumber("-1.23").Should().BeTrue();
		}

		[Test, Timeout(20)]
		public void IsValidNumber_PerformanceIsOK()
		{
			for (int i = 0; i < 500; i++)
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
				throw new ArgumentException("precision must be a non-negative number less than precision");
			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value)
		{
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).
			
			// EnableToFailPerformanceTest();
			
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

		private void EnableToFailPerformanceTest()
		{
			var performanceAnchor = new LinkedList<int>();
			for(int i=0;i<50000;i++)
				performanceAnchor.AddLast(i);
		}
	}
}