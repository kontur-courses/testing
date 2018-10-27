using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private const string ExceptionCheckCategory = "ArgumentExceptionCheck";
		private const string ValidCategory = "ShouldBeValid";
		private const string InvalidCategory = "ShouldBeInvalid";

		[Test]
		[Category(ExceptionCheckCategory)]
        public void ThrowArgumentException_WhenPrecisionIsNegative()
		{
			Action action = () => new NumberValidator(-1, 1);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		[Category(ExceptionCheckCategory)]
        public void ThrowArgumentException_WhenPrecisionIsZero()
		{
			Action action = () => new NumberValidator(0, 1);
			action.Should().Throw<ArgumentException>();
		}

        [Test]
        [Category(ExceptionCheckCategory)]
        public void ThrowArgumentException_WhenScaleIsNegative()
		{
			Action action = () => new NumberValidator(1, -1);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		[Category(ExceptionCheckCategory)]
        public void ThrowArgumentException_WhenScaleIsMoreThenPrecision()
		{
			Action action = () => new NumberValidator(1, 2);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		[Category(ExceptionCheckCategory)]
        public void DoesNotThrowArgumentException_WhenScaleIsZero()
		{
			Action action = () => new NumberValidator(1);
			action.Should().NotThrow<ArgumentException>();
		}

		[Test]
		[Category(ExceptionCheckCategory)]
        public void DoesNotThrowArgumentException_WhenNumberIsNull()
		{
			Action action = () => new NumberValidator(1).IsValidNumber(null);
			action.Should().NotThrow<ArgumentException>();
		}

		[Test]
		[Category(ExceptionCheckCategory)]
        public void DoesNotThrowArgumentException_WhenNumberIsEmpty()
		{
			Action action = () => new NumberValidator(1).IsValidNumber(string.Empty);
			action.Should().NotThrow<ArgumentException>();
		}

        [Test]
		[Category(ValidCategory)]
        public void Valid_WhenLessDigitsThenPrecision() => new NumberValidator(17, 2).IsValidNumber("0.00").Should().BeTrue();

		[Test]
		[Category(ValidCategory)]
        public void Valid_WhenLessDigitsThenScale() => new NumberValidator(3, 2).IsValidNumber("0.0").Should().BeTrue();

		[Test]
		[Category(ValidCategory)]
        public void Valid_WhenNoDigitsInScale() => new NumberValidator(3, 2).IsValidNumber("0").Should().BeTrue();

		[Test]
		[Category(ValidCategory)]
        public void Valid_WhenUseComma() => new NumberValidator(3, 2).IsValidNumber("0,00").Should().BeTrue();

		[Test]
		[Category(ValidCategory)]
        public void Valid_WhenUseMinus() => new NumberValidator(3, 2).IsValidNumber("-0").Should().BeTrue();

		[Test]
		[Category(ValidCategory)]
        public void Valid_WhenUsePlusInOnlyPositive() => new NumberValidator(3, 2, true).IsValidNumber("+0").Should().BeTrue();

		[Test]
		[Category(InvalidCategory)]
        public void Invalid_WhenMoreDigitsThenPrecision() => new NumberValidator(3, 2).IsValidNumber("00.00").Should().BeFalse();

		[Test]
		[Category(InvalidCategory)]
        public void Invalid_WhenMoreDigitsThenScale() => new NumberValidator(3, 1).IsValidNumber("0.00").Should().BeFalse();

		[Test]
		[Category(InvalidCategory)]
        public void Invalid_WhenMinusInOnlyPositive() => new NumberValidator(3, 2, true).IsValidNumber("-0").Should().BeFalse();

		[Test]
		[Category(InvalidCategory)]
        public void Invalid_WhenPlusCountsAsDigit() => new NumberValidator(1).IsValidNumber("+0").Should().BeFalse();

		[Test]
		[Category(InvalidCategory)]
        public void Invalid_WhenUseNotNumbers() => new NumberValidator(3, 2).IsValidNumber("a.aa").Should().BeFalse();
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