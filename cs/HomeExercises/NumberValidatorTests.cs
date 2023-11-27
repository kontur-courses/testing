using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidator_WhenPassNegativePrecision_ShouldThrowsArgumentException()
		{
			TestDelegate testDelegate = () => new NumberValidator(-1, 2, true);

			Assert.Throws<ArgumentException>(testDelegate);
		}

		[Test]
		public void NumberValidator_WhenPassNegativeScale_ShouldThrowsArgumentException()
		{
			TestDelegate testDelegate = () => new NumberValidator(1, -2);

			Assert.Throws<ArgumentException>(testDelegate);
		}

		[Test]
		public void NumberValidator_WhenPassValidArguments_ShouldDoesNotThrows()
		{
			TestDelegate testDelegate = () => new NumberValidator(1, 0, true);

			Assert.DoesNotThrow(testDelegate);
		}

		[Test]
		public void NumberValidator_WhenPrecisionIsEqualToTheScale_ShouldReturnFalse()
		{
			TestDelegate testDelegate = () => new NumberValidator(2, 2, true);

			Assert.Throws<ArgumentException>(testDelegate);
		}

		[Test]
		public void IsValidNumber_WhenPassOnlyPositiveIsFalseButNumbersDoesNotHaveNegativeSign_ShouldReturnTrue()
		{
			var validator = new NumberValidator(17, 2);

			var numberIsValid = validator.IsValidNumber("1.0");

			Assert.True(numberIsValid);
		}

		[Test]
		public void IsValidNumber_WhenLettersInsteadOfNumber_ShouldReturnFalse()
		{
			var validator = new NumberValidator(3, 2, true);

			var numberIsValid = validator.IsValidNumber("a.sd");

			Assert.IsFalse(numberIsValid);
		}

		[Test]
		public void IsValidNumber_WhenSymbolsInsteadOfNumber_ShouldReturnFalse()
		{
			var validator = new NumberValidator(3, 2, true);

			var numberIsValid = validator.IsValidNumber("2.!");

			Assert.IsFalse(numberIsValid);
		}

		[Test]
		public void IsValidNumber_WhenFractionalPartIsMissing_ShouldReturnTrue()
		{
			var validator = new NumberValidator(17, 2, true);

			var numberIsValid = validator.IsValidNumber("0");

			Assert.IsTrue(numberIsValid);
		}

		[Test]
		public void IsValidNumber_WhenNumberIsNull_ShouldReturnFalse()
		{
			var validator = new NumberValidator(17, 2, true);

			var numberIsValid = validator.IsValidNumber(null!);

			Assert.IsFalse(numberIsValid);
		}

		[Test]
		public void IsValidNumber_WhenPassNumberIsEmpty_ShouldReturnFalse()
		{
			var validator = new NumberValidator(3, 2, true);

			var numberIsValid = validator.IsValidNumber("");

			Assert.IsFalse(numberIsValid);
		}

		[Test]
		public void IsValidNumber_WhenIntPartWithNegativeSignMoreThanPrecision_ShouldReturnFalse()
		{
			var validator = new NumberValidator(3, 2, true);

			var numberIsValid = validator.IsValidNumber("-0.00");

			Assert.IsFalse(numberIsValid);
		}

		[Test]
		public void IsValidNumber_WhenIntPartWithPositiveSignMoreThanPrecision_ShouldReturnFalse()
		{
			var validator = new NumberValidator(3, 2, true);

			var numberIsValid = validator.IsValidNumber("+1.23");

			Assert.IsFalse(numberIsValid);
		}

		[Test]
		public void IsValidNumber_WhenFractionalPartMoreThanScale_ShouldReturnFalse()
		{
			var validator = new NumberValidator(17, 2, true);

			var numberIsValid = validator.IsValidNumber("0.000");

			Assert.IsFalse(numberIsValid);
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