using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
	{
        [Test]
        public void NumberValidator_RightInput_ShouldNotThrowException()
        {
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
        }
        [Test]
        public void NumberValidator_NegativePrecision_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
        }
        [Test]
        public void NumberValidator_NegativeScale_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(1, -1, true));
        }
        [Test]
        public void NumberValidator_ScaleLesserThanPrecision_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
        }
        [Test]
        public void NumberValidator_CheckingEmptyInput_False()
        {
            Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(""));
        }
        [Test]
        public void NumberValidator_CheckingNullInput_False()
        {
            Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(null));
        }
        [Test]
        public void NumberValidator_CheckingRightInputOnlyIntPart_True()
        {
            Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
        }
        [Test]
        public void NumberValidator_CheckingWrongInputOnlyIntPartGreaterThanScale_False()
        {
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("111111"));
        }
        [Test]
        public void NumberValidator_CheckingRightInputBothPartsLesserThanLimits_True()
        {
            Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
        }
        [Test]
        public void NumberValidator_CheckingRightInputScaleEqualLimits_True()
        {
            Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("00.00"));
        }
        [Test]
        public void NumberValidator_CheckingRightInputPrecisionEqualLimits_True()
        {
            Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("00.00"));
        }
        [Test]
        public void NumberValidator_CheckingWrongInputScaleGreaterThanLimits_False()
        {
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
        }
        [Test]
        public void NumberValidator_CheckingWrongInputPrecisionGreaterThanLimits_False()
        {
            Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
        }
        [Test]
        public void NumberValidator_CheckingSignIsAlsoPartOfScale()
        {
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
            Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+0.00"));
        }
        [Test]
        public void NumberValidator_CheckingRightInputLesserNotPositive_True()
        {
            Assert.IsTrue(new NumberValidator(17, 2, false).IsValidNumber("-1"));
            Assert.IsTrue(new NumberValidator(17, 2, false).IsValidNumber("-1.0"));
        }
        [Test]
        public void NumberValidator_CheckingWrongInputWrongSignOnlyInt_False()
        {
            Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("-1"));
        }
        [Test]
        public void NumberValidator_CheckingWrongInputWrongSign_False()
        {
            Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("-1.0"));
        }
        [Test]
        public void NumberValidator_CheckingWrongInputNotNumbers_False()
        {
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("ф.ыв"));
            Assert.IsFalse(new NumberValidator(10, 5, true).IsValidNumber("Ä.äÖÜü"));
            Assert.IsFalse(new NumberValidator(10, 5, true).IsValidNumber("슈.퍼맨"));
            Assert.IsFalse(new NumberValidator(10, 5, true).IsValidNumber("==.=="));
            Assert.IsFalse(new NumberValidator(10, 5, true).IsValidNumber("\n.\n\n"));
        }
        [Test]
        public void NumberValidator_CheckingContainLetters_False()
        {
            Assert.IsFalse(new NumberValidator(17, 10, false).IsValidNumber("1.23asd"));
            Assert.IsFalse(new NumberValidator(17, 10, false).IsValidNumber("1.asd23"));
        }
        [Test]
        public void NumberValidator_CheckingMultipleSign_False()
        {
            Assert.IsFalse(new NumberValidator(17, 10, false).IsValidNumber("--1.23"));
            Assert.IsFalse(new NumberValidator(17, 10, false).IsValidNumber("-----1.23"));
            Assert.IsFalse(new NumberValidator(17, 10, false).IsValidNumber("-+1.23"));
            Assert.IsFalse(new NumberValidator(17, 10, false).IsValidNumber("+-1.23"));
        }
        public void NumberValidator_CheckingWrongInputSignInFracPart_False()
        {
            Assert.IsFalse(new NumberValidator(17, 10, false).IsValidNumber("1.-23"));
            Assert.IsFalse(new NumberValidator(17, 10, false).IsValidNumber("1.23-"));
        }
        [Test]
        public void NumberValidator_WrongDelimeter_False()
        {
            Assert.IsFalse(new NumberValidator(17, 10, false).IsValidNumber("1/23"));
        }

        [Test]
		public void NumberValidator_TestNotStaticOnlyPositive()
		{
            var val1 = new NumberValidator(17, 10, true);
            var val2 = new NumberValidator(17, 10, false);
            Assert.IsFalse(val1.IsValidNumber("-1"));
            Assert.IsTrue(val2.IsValidNumber("-1"));

        }
        [Test]
        public void NumberValidator_TestNotStaticScale()
        {
            var val1 = new NumberValidator(5, 3, true);
            var val2 = new NumberValidator(10, 3, false);
            Assert.IsFalse(val1.IsValidNumber("000000.0"));
            Assert.IsTrue(val2.IsValidNumber("000000.0"));

        }
        [Test]
        public void NumberValidator_TestNotStaticPrecision()
        {
            var val1 = new NumberValidator(10, 3, true);
            var val2 = new NumberValidator(10, 8, false);
            Assert.IsFalse(val1.IsValidNumber("1.000000"));
            Assert.IsTrue(val2.IsValidNumber("1.000000"));

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