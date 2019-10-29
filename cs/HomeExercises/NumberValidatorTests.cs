using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
	{
		[TestCase(0, 0)]
		[TestCase(-17, 0)]
		[TestCase(1, -2)]
		[TestCase(3, 3)]
		[TestCase(2, 3)]
		public void NumberValidator_ThrowsArgumentException_CreationWithIncorrectArguments(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
        }

		[Test]
		public void NumberValidator_DoesNotThrowException_CorrectCreation()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0));
        }

		[TestCase("0")]
		[TestCase("10.01")]
		[TestCase("10,5")]
		[TestCase("+1.23")]
		[TestCase("-9.99")]
		public void IsValidNumber_ReturnsTrue_CheckingCorrectNumbers(string value)
		{
			Assert.IsTrue(new NumberValidator(4, 2).IsValidNumber(value));
        }

		[TestCase("1")]
		[TestCase("+2")]
		public void IsValidNumber_ReturnsTrue_CheckingCorrectPositiveNumbers(string value)
		{
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber(value));
		}

        [TestCase("0.000")]
		[TestCase("+1.23")]
		[TestCase("-1.23")]
		public void IsValidNumber_ReturnsFalse_CheckingIncorrectLargeNumbers(string value)
		{
			Assert.IsFalse(new NumberValidator(3, 2).IsValidNumber(value));
        }

		[Test]
		public void IsValidNumber_ReturnsFalse_CheckingIncorrectNegativeNumber()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.5"));
        }

		[TestCase("a.sd")]
		[TestCase("")]
		[TestCase(null)]
		public void IsValidNumber_ReturnsFalse_CheckingNotANumber(string value)
		{
			Assert.IsFalse(new NumberValidator(3, 2).IsValidNumber(value));
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