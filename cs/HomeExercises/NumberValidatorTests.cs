using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-2, 1, TestName = "Constructor_WhenPrecisionIsNegative_ThrowArgumentException")]
		[TestCase(2, -1, TestName = "Constructor_WhenScaleIsNegative_ThrowArgumentException")]
		[TestCase(1, 2, TestName = "Constructor_WhenScaleMoreThenPrecision_ThrowArgumentException")]
        public void Constructor_WhenIncorrectArguments_ThrowArgumentException(int precision, int scale)
		{
			// ReSharper disable once ObjectCreationAsStatement
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, true));
			// ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, false));
        }

        [TestCase(1, 0, TestName = "Constructor_WhenPrecisionIsPositiveAndScaleIsZero_DontThrowArgumentException")]
        [TestCase(2, 1, TestName = "Constructor_WhenPrecisionIsPositiveAndScaleIsPositive_DontThrowArgumentException")]
        public void Constructor_WhenCorrectArguments_DontThrowArgumentException(int precision, int scale)
		{
			// ReSharper disable once ObjectCreationAsStatement
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, true));
			// ReSharper disable once ObjectCreationAsStatement
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, false));
        }


        [TestCase("-0.00", TestName = "IsValidNumber_WhenNegativeNumberWithFractional_ReturnTrue")]
		[TestCase("+0.00", TestName = "IsValidNumber_WhenPositiveNumberWithFractional_ReturnTrue")]
		[TestCase("0,00", TestName = "IsValidNumber_WhenNumberWithComma_ReturnTrue")]
        [TestCase("0.00", TestName = "IsValidNumber_WhenNumberWithPoint_ReturnTrue")]
		[TestCase("00.00", TestName = "IsValidNumber_WhenNumberWithMultipleZeros_ReturnTrue")]
        public void IsValidNumber_WhenNumberWithFractionalAndInteger_ReturnTrue(string number)
		{
			Assert.IsTrue(new NumberValidator(17, 2).IsValidNumber(number));
		}

		[TestCase("0", TestName = "IsValidNumber_WhenInteger_ReturnTrue")]
		[TestCase("+0", TestName = "IsValidNumber_WhenPositiveInteger_ReturnTrue")]
		[TestCase("-0", TestName = "IsValidNumber_WhenNegativeInteger_ReturnTrue")]
        public void IsValidNumber_WhenInteger_ReturnTrue(string number)
        {
	        Assert.IsTrue(new NumberValidator(17, 0).IsValidNumber(number));
        }

        [TestCase("0.00", TestName = "IsValidNumber_WhenOverflowNumberInFractional_ReturnFalse")]
        [TestCase("-00.0", TestName = "IsValidNumber_WhenOverflowSymbolThroMinus_ReturnFalse")]
        [TestCase("+00.0", TestName = "IsValidNumber_WhenOverflowSymbolThroPlus_ReturnFalse")]
        [TestCase("0000", TestName = "IsValidNumber_WhenOverflowNumberInInteger_ReturnFalse")]
        public void IsValidNumber_WhenOverflowSymbol_ReturnFalse(string number)
		{
			Assert.IsFalse(new NumberValidator(3, 1, true).IsValidNumber(number));
		}

		[Test]
        public void IsValidNumber_WhenNegativeNumberWithPositiveValidator_ReturnFalse()
        {
	        Assert.IsFalse(new NumberValidator(3, 1, true).IsValidNumber("-0"));
	        Assert.IsFalse(new NumberValidator(3, 1, true).IsValidNumber("-0.0"));
        }

        [TestCase(null, TestName = "IsValidNumber_WhenNull_ReturnFalse")]
		[TestCase("", TestName = "IsValidNumber_WhenEmptyString_ReturnFalse")]
		[TestCase(" ", TestName = "IsValidNumber_WhenSpace_ReturnFalse")]
		[TestCase("\r\n", TestName = "IsValidNumber_WhenCRLF_ReturnFalse")]
		public void IsValidNumber_WhenIsNullOrEmpty_ReturnFalse(string number)
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(number));
		}

        [TestCase("a.sd", TestName = "IsValidNumber_WhenSomeTextWithPoint_ReturnFalse")]
        [TestCase("00.sd", TestName = "IsValidNumber_WhenSomeTextWithPointAndNumber_ReturnFalse")]
        [TestCase("asd", TestName = "IsValidNumber_WhenSomeText_ReturnFalse")]
        public void IsValidNumber_WhenNaN_ReturnFalse(string number)
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(number));
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