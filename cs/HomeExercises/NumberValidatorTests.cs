using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidator_ThrowsException_OnNegativePrecision_OnOnlyPositive()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
		}

		[Test]
		public void NumberValidator_ThrowsException_OnNegativePrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2));
		}

		[Test]
		public void NumberValidator_ThrowsException_OnNegativeScale_OnOnlyPositive()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(3, -2, true));
		}

		[Test]
		public void NumberValidator_ThrowsException_OnNegativeScale()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(3, -2, false));
		}

		[Test]
		public void NumberValidator_ThrowsException_OnScaleGreaterThanPrecision_OnOnlyPositive()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
		}

		[Test]
		public void NumberValidator_ThrowsException_OnScaleGreaterThanPrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, false));
		}

		[Test]
		public void NumberValidator_ThrowsException_OnPrecisionEqualsZero_OnOnlyPositive()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(0, 0, true));
		}

		[Test]
		public void NumberValidator_ThrowsException_OnPrecisionEqualsZero()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(0, 0, false));
		}

		[Test]
		public void NumberValidator_DoesNotThrowException_OnScaleEqualsZero_OnOnlyPositive()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		}

		[Test]
		public void NumberValidator_DoesNotThrowException_OnScaleEqualsZero()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, false));
		}

		[Test]
		public void NumberValidator_DoesNotThrowException_OnPrecisionGreaterThanScale_OnOnlyPositive()
		{
			Assert.DoesNotThrow(() => new NumberValidator(4, 2, true));
		}

		[Test]
		public void NumberValidator_DoesNotThrowException_OnPrecisionGreaterThanScale()
		{
			Assert.DoesNotThrow(() => new NumberValidator(4, 2, false));
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenPrecisionGreaterThanNumberLength()
		{
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_OnNegativeNumber_WhenOnlyPositive()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("-0.0"));
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_OnNegativeNumber_WhenNotOnlyPositive()
		{
			Assert.IsTrue(new NumberValidator(17, 2).IsValidNumber("-0.0"));
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_OnInteger_WhenScaleEqualsZero()
		{
			Assert.IsTrue(new NumberValidator(17, 0, true).IsValidNumber("0"));
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_OnInteger_WhenScaleGreaterThanZero()
		{
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_OnNumberEndingWithPoint()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0."));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_OnNumberStartingWithPoint()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(".0"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_OnNumberEndingWithWhiteSpaces()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0   "));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_OnNumberStartingWithWhiteSpaces()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("   0"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_OnNull()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(null));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_OnEmptyString()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(""));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_StringContainsWhiteSpaces()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("  "));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenPrecisionLessThanNumberLength()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("12.12"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenPrecisionLessThanNumberLength_IncludingMinus()
		{
			Assert.IsFalse(new NumberValidator(3, 2, false).IsValidNumber("-0.00"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenPrecisionLessThanNumberLength_IncludingPlus()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenPrecisionEqualsNumberLength_IncludingMinus()
		{
			Assert.IsTrue(new NumberValidator(4, 2, false).IsValidNumber("-0.00"));
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenPrecisionEqualsNumberLength_IncludingPlus()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenNumberContainsWoreThanOneDifferentSigns()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+-0.00"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenNumberContainsWoreThanOnePlus()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("++0.00"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenNumberContainsWoreThanOneMinus()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("--0.00"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenNumberContainsWoreThanOnePoint()
		{
			Assert.IsFalse(new NumberValidator(10, 5, true).IsValidNumber("0.0.0"));
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenNumberContainsCommaInsteadOfPoint()
		{
			Assert.IsTrue(new NumberValidator(3, 2, true).IsValidNumber("0,00"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse__WhenPrecisionLessThanNumberLength_IncludingInsignificantZeros()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("0012"));
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenScaleLessThanFracPartLength()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
		}

		[Test]
		public void IsValidNumber_ReturnsSameResult_OnSecondCall()
		{
			var numberValidator = new NumberValidator(17, 2, true);
			Assert.IsTrue(numberValidator.IsValidNumber("0.00"));
			Assert.IsTrue(numberValidator.IsValidNumber("0.00"));
		}

		[Test]
		public void IsValidNumber_SupportsAllDigits()
		{
			var allCasesAreValid = true;
			var numberValidator = new NumberValidator(17, 5, true);
			for (var i = 0; i < 10; i++)
				if (!numberValidator.IsValidNumber($"{i}{i}.{9 - i}{9 - i}"))
				{
					allCasesAreValid = false;
					break;
				}
			Assert.IsTrue(allCasesAreValid);
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_OnNonNumericInput()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
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