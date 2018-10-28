using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private readonly NumberValidator numberValidator = GetNumberValidator();

		private static NumberValidator GetNumberValidator(int precision = 100, 
			int scale = 99,
			bool onlyPositive = false)
			=> new NumberValidator(precision, scale, onlyPositive);

		[Test]
		public void CreationFails_WhenValidatorHasNegativePrecision()
		{
			Assert.Throws<ArgumentException>(() => GetNumberValidator(-1, 3));
		}

		[Test]
		public void CreationFails_WhenValidatorHasZeroPrecision()
		{
			Assert.Throws<ArgumentException>(() => GetNumberValidator(0, 0));
		}

		[Test]
		public void CreationFails_WhenValidatorHasNegativeScale()
		{
			Assert.Throws<ArgumentException>(() => GetNumberValidator(1, -1));
		}

		[Test]
		public void CreationFails_WhenValidatorHasScaleMoreThanPrecision()
		{
			Assert.Throws<ArgumentException>(() => GetNumberValidator(1, 2));
		}

		[Test]
		public void CreationFails_WhenValidatorHasScaleEqualsPrecision()
		{
			Assert.Throws<ArgumentException>(() => GetNumberValidator(1, 1));
		}

		[Test]
		public void CreationIsSuccessful_WhenValidatorHasZeroScale()
		{
			Assert.DoesNotThrow(() => GetNumberValidator(1, 0));
		}

		[Test]
		public void CreationIsSuccessful_WhenValidatorHasTwoPrecisionAndOneScale([Values]bool onlyPositive)
		{
			Assert.DoesNotThrow(() => GetNumberValidator(2, 1, onlyPositive));
		}
		

		[Test]
		[TestCase("abc")]
		[TestCase("aaa.a")]
		[TestCase(".")]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNotANumberWithoutDigits(String notANumberString)
		{
			numberValidator
				.IsValidNumber(notANumberString)
				.Should()
				.BeFalse();
		}

		[Test]
		[TestCase("string0.1")]
		[TestCase("0.0string1")]
		[TestCase("0.1string")]
		[TestCase("112str1.1")]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNotANumberWithNumber(String numberWithNotNumberString)
		{
			numberValidator
				.IsValidNumber(numberWithNotNumberString)
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenPositiveNumbersValidatorGetsNegativeNumber()
		{
			var negativeNumberString = "-0.1";
			var validator = GetNumberValidator(100, 99, true);

			validator
				.IsValidNumber(negativeNumberString)
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenAllNumbersValidatorGetsCorrectNegativeNumber()
		{
			var negativeNumberString = "-0.1";
			var validator = GetNumberValidator(99, 98, false);

			validator
				.IsValidNumber(negativeNumberString)
				.Should()
				.BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenValidatorGetsCorrectPositiveNumber([Values]bool onlyPositive)
		{
			var positiveNumberString = "+0.1";
			var validator = GetNumberValidator(100, 99, onlyPositive);

			validator
				.IsValidNumber(positiveNumberString)
				.Should()
				.BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenValidatorGetsZero([Values]bool onlyPositive)
		{
			var zeroString = "0.0";
			var validator = GetNumberValidator(100, 99, onlyPositive);

			validator
				.IsValidNumber(zeroString)
				.Should()
				.BeTrue();
		}

        [Test]
		public void IsValidNumber_ReturnsTrue_WhenValidatorGetsCorrectNumberWithoutSignificantBefore([Values] bool onlyPositive)
		{
			var positiveNumberString = "0.1";
			var validator = GetNumberValidator(100, 99, onlyPositive);

			validator
				.IsValidNumber(positiveNumberString)
				.Should()
				.BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenValidatorGetsCorrectNumberWithMaximumPrecision()
		{
			var positiveNumberString = "11111.0";
			var validator = GetNumberValidator(6, 2);

			validator.IsValidNumber(positiveNumberString)
				.Should()
				.BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenValidatorGetsCorrectNumberWithMaximumScale()
		{
			var positiveNumberString = "11.00001";
			var validator = GetNumberValidator(100, 5);

			validator.IsValidNumber(positiveNumberString)
				.Should()
				.BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNumberWithTooBigScale()
		{
			var positiveNumberString = "11.001";
			var validator = GetNumberValidator(6, 1);

			validator.IsValidNumber(positiveNumberString)
				.Should()
				.BeFalse();
        }

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNumberWithTooBigPrecision()
		{
			var positiveNumberString = "110000.001";
			var validator = GetNumberValidator(7, 5);

			validator.IsValidNumber(positiveNumberString)
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenValidatorGetsCorrectNumberWithoutPartial()
		{
			var integerString = "100";

			numberValidator.IsValidNumber(integerString)
				.Should()
				.BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNumberWithoutIntegerPart()
		{
			var numberWithDotAtStart = ".0001";

			numberValidator.IsValidNumber(numberWithDotAtStart)
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNumberWithNoPartialAfterDot()
		{
			var numberWithDotAtTheEndString = "100.";

			numberValidator.IsValidNumber(numberWithDotAtTheEndString)
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNumberWithTwoDotsInside()
		{
			var numberWithTwoDots = "100.0.0";

			numberValidator.IsValidNumber(numberWithTwoDots)
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenValidatorGetsCorrectNumberWithIntegerPartMoreThanLong()
		{
			var number = String.Concat(Enumerable.Repeat("1", 30)) + ".0";

			numberValidator.IsValidNumber(number)
				.Should()
				.BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnsTrue_WhenValidatorGetsCorrectNumberWithPartialPartMoreThanThirtySigns()
		{
			var number = "1." + String.Concat(Enumerable.Repeat("1", 31));

			numberValidator.IsValidNumber(number)
				.Should()
				.BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenGetsNumberWithZeroScaleWithTooBigPrecision()
		{
			var numberString = "1.0000000";
			var validator = GetNumberValidator(100, 3);

			validator.IsValidNumber(numberString)
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenGetsIntegerNumberEqualsZeroWithTooBigPrecision()
		{
			var zeroString = "00000000.0";
			var validator = GetNumberValidator(5, 2);

			validator.IsValidNumber(zeroString)
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNull()
		{
			numberValidator.IsValidNumber(null)
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsEmptyString()
		{
			numberValidator.IsValidNumber("")
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsTrueOnOldValidator_WhenOldValidatorAcceptsNumberButNewDoesNot()
		{
			var oldValidator = new NumberValidator(100, 99);
			var newValidator = new NumberValidator(3, 1);
			var numberString = "1111.0";

			oldValidator.IsValidNumber(numberString)
				.Should()
				.BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnsTrueOnNewValidator_WhenNewValidatorAcceptsNumberButOldDoesNot()
		{
			var oldValidator = new NumberValidator(3, 1);
			var newValidator = new NumberValidator(100, 99);
			var numberString = "1111.0";

			newValidator.IsValidNumber(numberString)
				.Should()
				.BeTrue();
		}

        [Test, Timeout(1000)]
		public void IsValidNumber_WorksFast_WhenValidatorExecutesManyOperations()
		{
			var numberString = "123456.123";
			var iterationsCount = 100000;

			for (var iterationIndex = 0; iterationIndex < iterationsCount; iterationIndex++)
			{
				numberValidator.IsValidNumber(numberString);
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