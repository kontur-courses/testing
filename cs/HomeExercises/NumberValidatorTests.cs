using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void ThrowArgumentException_WhenScaleIsNegativeNumber()
		{
			Action acr = () => new NumberValidator(5, -1);
			acr.Should().Throw<ArgumentException>();
		}

		[Test]
		public void ThrowArgumentException_WhenScaleIsGreaterThenPrecision()
		{
			Action acr = () => new NumberValidator(5, 6);
			acr.Should().Throw<ArgumentException>();
        }

		[Test]
		public void ThrowArgumentException_WhenPrecisionIsNegativeNumber()
		{
			Action act = () => new NumberValidator(-1);
			act.Should().Throw<ArgumentException>();
		}

		[Test]
		public void ThrowArgumentException_WhenPrecisionIsZero()
		{
			Action act = () => new NumberValidator(0);
			act.Should().Throw<ArgumentException>();
		}

        [Test]
		public void DoesNotThrowException_WhenCreateWithValidArguments()
		{
			Action act = () => new NumberValidator(1, 0, true);
			act.Should().NotThrow();
		}

		[Test]
		public void ReturnTrue_WhenNumberWithPointIsCorrect()
		{
			var result = new NumberValidator(17, 2).IsValidNumber("0.0");
			result.Should().BeTrue();
		}

		[Test]
		public void ReturnTrue_WhenNumberWithoutFractionIsCorrect()
		{
			var result = new NumberValidator(17, 2).IsValidNumber("0");
			result.Should().BeTrue();
		}

		[Test]
		public void ReturnFalse_WhenNumberIsGreaterThanPrecision()
		{
			var result = new NumberValidator(3, 2).IsValidNumber("00.00");
			result.Should().BeFalse();
		}

		[Test]
		public void ReturnFalse_WhenNegativeNumbersAreForbidden_AndNumberIsNegative()
		{
			var result = new NumberValidator(5, 2, true).IsValidNumber("-0.00");
			result.Should().BeFalse();
		}

		[Test]
		public void ReturnFalse_WhenNumberWithPlusSignIsGreaterThenPrecision()
		{
			var result = new NumberValidator(3, 2, true).IsValidNumber("+0.00");
			result.Should().BeFalse();
		}

		[Test]
		public void ReturnFalse_WhenNumberWithMinusSignIsGreaterThenPrecision()
		{
			var result = new NumberValidator(3, 2).IsValidNumber("-1.23");
			result.Should().BeFalse();
		}

        [Test]
		public void ReturnTrue_WhenNumberWithPlusSignIsEqualsToPrecision()
		{
			var result = new NumberValidator(4, 2, true).IsValidNumber("+1.23");
			result.Should().BeTrue();
		}

		[Test]
		public void ReturnFalse_WhenFractionIsGreaterThenAllowed()
		{
			var result = new NumberValidator(17, 2, true).IsValidNumber("0.000");
			result.Should().BeFalse();
		}

		[Test]
		public void ReturnFalse_WhenValueIsNotANumber()
		{
			var result = new NumberValidator(3, 2, true).IsValidNumber("a.sd");
			result.Should().BeFalse();
		}

		[Test]
		public void ReturnTrue_WhenNumberWithCommaIsCorrect()
		{
			var result = new NumberValidator(17, 2, true).IsValidNumber("0,0");
			result.Should().BeTrue();
        }

		[Test]
		public void ReturnFalse_WhenValueStartsWithWhitespaces()
		{
			var result = new NumberValidator(17, 2, true).IsValidNumber("  0,0");
			result.Should().BeFalse();
        }

		[Test]
		public void ReturnTrue_WhenScaleIsZero_AndNumberIsInteger()
		{
			var result = new NumberValidator(17).IsValidNumber("0");
			result.Should().BeTrue();
        }

		[Test]
		public void ReturnFalse_WhenValueEndsWithWhitespaces()
		{
			var result = new NumberValidator(17, 2, true).IsValidNumber("0,0  ");
			result.Should().BeFalse();
		}

		[Test]
		public void ReturnFalse_WhenValueHaveWhitespaces()
		{
			var result = new NumberValidator(17, 2, true).IsValidNumber("  0 , 0 ");
			result.Should().BeFalse();
		}

		[Test]
		public void ReturnFalse_WhenValueIsNull()
		{
			var result = new NumberValidator(17, 2, true).IsValidNumber(null);
			result.Should().BeFalse();
        }

		[Test]
		public void ReturnFalse_WhenValueIsEmpty()
		{
			var result = new NumberValidator(17, 2, true).IsValidNumber("");
			result.Should().BeFalse();
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
				throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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