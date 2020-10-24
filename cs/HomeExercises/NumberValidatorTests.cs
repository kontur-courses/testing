using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true, TestName = "NegativePrecision")]
		[TestCase(0, 2, true, TestName = "ZeroPrecision")]
		[TestCase(1, -1, true, TestName = "NegativeScale")]
		[TestCase(1, 2, true, TestName = "ScaleMoreThenPrecision")]
		public void ThrowsArgumentException_OnWrongData(int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
		}

		[TestCase("a", TestName = "OneLetter")]
		[TestCase("-a", TestName = "NegativeOneLetter")]
		[TestCase("a.", TestName = "OneLetterBeforeDot")]
		[TestCase(".a", TestName = "OneLetterAfterDot")]
		[TestCase("a.a", TestName = "TwoLettersWithDot")]
		[TestCase("a,", TestName = "OneLetterBeforeComma")]
		[TestCase(",a", TestName = "OneLetterAfterComma")]
		[TestCase("a,a", TestName = "TwoLettersWithComma")]
		[TestCase(",", TestName = "Comma")]
		[TestCase(".", TestName = "Dot")]
		[TestCase("", TestName = "Empty")]
		[TestCase(null, TestName = "null")]
		public void ValidNumber_ReturnFalseNotOnNumbers(string checkedString)
		{
			var numberValidator = new NumberValidator(17, 2, true);
			numberValidator.IsValidNumber(checkedString).Should().BeFalse();
		}

		[TestCase("1.5", TestName = "OneDigitAfterDot")]
		[TestCase("1.05", TestName = "TwoDigitsAfterDot")]
		[TestCase("1", TestName = "Integer")]
		[TestCase("1,5", TestName = "OneDigitAfterComma")]
		[TestCase("1,05", TestName = "TwoDigitsAfterComma")]
		[TestCase("+1.5", TestName = "OneDigitAfterDotWithPlus")]
		[TestCase("+1.05", TestName = "TwoDigitsAfterDotWithPlus")]
		[TestCase("+1", TestName = "IntegerWithPlus")]
		[TestCase("+1,5", TestName = "OneDigitAfterCommaWithPlus")]
		[TestCase("+1,05", TestName = "TwoDigitsAfterCommaWithPlus")]
		public void CorrectWork_OnOnlyPositiveBigPrecisionAndScaleTwo(string checkedNumber)
		{
			var numberValidator = new NumberValidator(17, 2, true);
			numberValidator.IsValidNumber(checkedNumber).Should().BeTrue();
		}

		[Test]
		public void OnlyPositiveNumberValidator_ShouldBeFalse_OnNegativeNumber()
		{
			new NumberValidator(17, 2, true).IsValidNumber("-1");
		}

		
		[TestCase("1.05", TestName = "ThreeDigitsWithDot")]
		[TestCase("100", TestName = "ThreeDigits")]
		[TestCase("+100", TestName = "ThreeDigitsWithPlus")]
		[TestCase("+10,0", TestName = "ThreeDigitsWithPlusAndComma")]
		[TestCase("+10.0", TestName = "ThreeDigitsWithPlusAndDot")]
		[TestCase("1,05", TestName = "ThreeDigitsWithComma")]
		[TestCase("-1.5", TestName = "TwoDigitsWithDotAndMinus")]
		[TestCase("-10", TestName = "TwoDigitsWithMinus")]
		public void CorrectWork_OnNotOnlyPositive_PrecisionThree_ScaleTwo(string checkedNumber)
		{
			var numberValidator = new NumberValidator(3, 2, false);
			numberValidator.IsValidNumber(checkedNumber).Should().BeTrue();
		}

		[TestCase("-100", TestName = "NegativeInteger")]
		[TestCase("-1.00", TestName = "ThreeDigitsWithDotAndMinus")]
		[TestCase("-1,00", TestName = "ThreeDigitsWithCommaAndMinus")]
		[TestCase("1000", TestName = "PositiveInteger")]
		[TestCase("10.00", TestName = "FourDigitsAntTwoDigitsAfterDot")]
		[TestCase("10,00", TestName = "FourDigitsAntTwoDigitsAfterComma")]
		[TestCase("1.000", TestName = "FourDigitsAntThreeDigitsAfterDot")]
		[TestCase("1,000", TestName = "FourDigitsAntThreeDigitsAfterComma")]
		public void PrecisionThree_ScaleTwo_NotOnlyPositive_ShouldBeFalseOnTooLongNumbers(string checkedNumber)
		{
			var numberValidator = new NumberValidator(3, 2, false);
			numberValidator.IsValidNumber(checkedNumber).Should().BeFalse();
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