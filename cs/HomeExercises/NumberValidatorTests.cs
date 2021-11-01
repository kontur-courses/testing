using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
	{
		[Test]
		public void Test()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			//Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			//Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			//Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			//Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
		}

		[Test]
		public void NumberValidator_PrecisionIsNegativeNumber_ArgumentException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2));
		}

		[Test]
		public void NumberValidator_PrecisionIsZero_ArgumentException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(0, 2, true));
		}

		[Test]
		public void NumberValidator_PrecisionShouldBeMoreZero_Passing()
		{
			Assert.DoesNotThrow(() => new NumberValidator(2, 1));
		}

		[Test]
		public void NumberValidator_ScaleIsNegativeNumber_ArgumentException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(2, -1, true));
		}

		[Test]
		public void NumberValidator_ScaleShouldBeMoreOrEqualZero_Passing()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0));
			Assert.DoesNotThrow(() => new NumberValidator(2, 1));
		}

		[Test]
		public void NumberValidator_ScaleShouldBeSmallestThenPrecision_ArgumentException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(2, 2, true));
		}

		[Test]
		public void IsValidNumber_InputIsSpace_False()
		{
			CheckInputIsCorrect(" ",false);
		}

		[Test]
		public void IsValidNumber_InputIsLetters_False()
		{
			CheckInputIsCorrect("e.e", false);
		}

		[Test]
		public void IsValidNumber_InputIsEmpty_False()
		{
			CheckInputIsCorrect("", false);
		}

		[Test]
		public void IsValidNumber_InputIsFractionalPartOnly_False()
		{
			CheckInputIsCorrect(".0", false);
		}

		[Test]
		public void IsValidNumber_InputIsPlus_False()
		{
			CheckInputIsCorrect("+", false);
		}

		[Test]
		public void IsValidNumber_InputIsWholePartWithDotAndWithoutFractionalPart_False()
		{
			CheckInputIsCorrect("5.", false);
		}

		[Test]
		public void IsValidNumber_InputIs_False()
		{
			CheckInputIsCorrect("+-5", false);
		}

		NumberValidator numberValidator  = new NumberValidator(5, 4);
		private void CheckInputIsCorrect(string input,bool expectedResult)
        {
			Assert.AreEqual(expectedResult, numberValidator.IsValidNumber(input));
        }

		[Test]
		public void IsValidNumber_SignIsAWholePart_Passing()
		{
			CheckWithoutFractionalPart("-5",true);
			CheckWithoutFractionalPart("+15", false);
			CheckWithoutFractionalPart("+5", true);
		}

		[Test]
		public void IsValidNumber_WholePartMustBeTwoCharactersOrLess_Passing()
		{
			CheckWithoutFractionalPart("0", true);
			CheckWithoutFractionalPart("00", true);
			CheckWithoutFractionalPart("100", false);
		}

		[Test]
		public void IsValidNumber_NumberWithoutFractionalPartCannotContainFractionalPart_False()
		{
			CheckWithoutFractionalPart("0.0", false);
		}

		NumberValidator validatorWithoutFractionalPart = new NumberValidator(2, 0);
		private void CheckWithoutFractionalPart(string input, bool expectedResult)
		{
			Assert.AreEqual(expectedResult, validatorWithoutFractionalPart.IsValidNumber(input));
		}

		[Test]
		public void IsValidNumber_SumLengtFractionalAndIntegerPartsShouldBeLessOrEqual4_Passing()
		{
			CheckWithFractionalPart("-0.0", true);
			CheckWithFractionalPart("00.00", true);
			CheckWithFractionalPart("000.00", false);
		}

		[Test]
		public void IsValidNumber_NumberMayNotContainFractionalPart_True()
		{
			CheckWithFractionalPart("0000", true);
		}

		[Test]
		public void IsValidNumber_LengtFractionalPartsShouldBeLessOrEqual2_False()
		{
			CheckWithFractionalPart("0.000", false);
		}

		NumberValidator validatorWithFractionalPart = new NumberValidator(4, 2);
		private void CheckWithFractionalPart(string input, bool expectedResult)
		{
			Assert.AreEqual(expectedResult, validatorWithFractionalPart.IsValidNumber(input));
		}

		[Test]
		public void IsValidNumber_NumbersShouldBePositiveIfValidatorOnlyPositive_False()
		{
			Assert.IsFalse(new NumberValidator(4,2,true).IsValidNumber("-1.0"));
		}
	}

	public class NumberValidator
	{
		private readonly Regex numberRegex;
		private readonly bool onlyPositive;
		//строго больше 0 - максимальное число знаков в числе включая знак
		private readonly int precision;
		//больше или равен 0 и строго меньше precision - максимальное число знаков дробной части
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