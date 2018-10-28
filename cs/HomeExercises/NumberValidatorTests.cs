using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		// Refactored tests
		[Test]
		[Category("Exceptions")]
		public void ThrowsArgumentException_WhenPrecisionIsNegative()
		{
			Action action = () => new NumberValidator(-1, 2, true);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		[Category("Exceptions")]
		public void DoesNotThrowArgumentException_WhenPrecisionIsPositiveAndScaleIsZero()
		{
			Action action = () => new NumberValidator(1, 0, true);
			action.Should().NotThrow<ArgumentException>();
		}

		[Test]
		[Category("NumberValidations")]
		public void IsValid_WhenPrecisionIsMoreThanDigits()
		{
			new NumberValidator(17, 2).IsValidNumber("0.0").Should().BeTrue();
		}

		[Test]
		[Category("NumberValidations")]
		public void IsValid_WhenScaleHasNoDigits()
		{
			new NumberValidator(17, 2).IsValidNumber("0").Should().BeTrue();
		}

		[Test]
		[Category("NumberValidations")]
		public void IsInvalid_WhenPrecisionIsLessThanDigits()
		{
			new NumberValidator(3, 2).IsValidNumber("00.00").Should().BeFalse();
		}

		[Test]
		[Category("NumberValidations")]
		public void IsValid_WhenInputHasPlusSign()
		{
			new NumberValidator(4, 2).IsValidNumber("+1.23").Should().BeTrue();
		}

		[Test]
		[Category("NumberValidations")]
		public void IsInvalid_WhenScaleIsLessThanDigits()
		{
			new NumberValidator(17, 2).IsValidNumber("0.000").Should().BeFalse();
		}

		[Test]
		[Category("NumberValidations")]
		public void IsInvalid_WhenNotNumericInput()
		{
			new NumberValidator(17, 2).IsValidNumber("a.sd").Should().BeFalse();
		}

		// Additional tests
		[Test]
		[Category("Exceptions")]
		public void ThrowsArgumentException_WhenScaleIsNegative()
		{
			Action action = () => new NumberValidator(2, -2, true);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		[Category("Exceptions")]
		public void ThrowsArgumentException_WhenScaleIsMoreThanPrecision()
		{
			Action action2 = () => new NumberValidator(2, 3, true);
			action2.Should().Throw<ArgumentException>();
		}

		[Test]
		public void ThrowsArgumentException_WhenScaleIsEqualToPrecision()
		{
			Action action = () => new NumberValidator(2, 2, true);
			action.Should().Throw<ArgumentException>();
        }
		[Test]
		[Category("NumberValidations")]
		public void IsInvalid_WhenInputIsEmptyString()
		{
			new NumberValidator(17, 2).IsValidNumber("").Should().BeFalse();
		}

		[Test]
		[Category("NumberValidations")]
		public void IsInvalid_WhenInputIsNull()
		{
			new NumberValidator(17, 2).IsValidNumber(null).Should().BeFalse();
		}

		[Test]
		[Category("Exceptions")]
		public void DoesNotThrowArgumentException_WhenInputIsNull()
		{
			Action action = () => new NumberValidator(17, 2).IsValidNumber(null);
			action.Should().NotThrow<ArgumentException>();
		}

		[Test]
		[Category("Exceptions")]
		public void DoesNotThrowArgumentException_WhenInputIsEmpty()
		{
			Action action = () => new NumberValidator(17, 2).IsValidNumber("");
			action.Should().NotThrow<ArgumentException>();
        }
		[Test]
		[Category("NumberValidations")]
		public void IsInvalid_WhenMinusWithOnlyPositiveFlagIsSet()
		{
			new NumberValidator(17, 2, true).IsValidNumber("-5").Should().BeFalse();
		}

		[Test]
		[Category("NumberValidations")]
		public void IsValid_WhenCommaUsedAsDelimiter()
		{
			new NumberValidator(17, 2).IsValidNumber("0,00").Should().BeTrue();
		}

		[Test]
		[Category("NumberValidations")]
		public void IsInvalid_WhenStartsWithSeveralSymbols()
		{
			new NumberValidator(17, 2).IsValidNumber("+-0,00").Should().BeFalse();
		}

		[Test]
		[Category("NumberValidations")]
		public void IsValid_WhenHasOnlyFractalPart()
		{
			new NumberValidator(17, 2).IsValidNumber("0,12").Should().BeTrue();
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
}