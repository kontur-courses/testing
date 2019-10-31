using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
	{
        [TestCase(-1, 2, true, TestName = "NegativePrecision_ShouldThrowArgumentException")]
        [TestCase(1, -1, true, TestName = "NegativeScale_ShouldThrowArgumentException")]
        [TestCase(1, 2, true, TestName = "ScaleLessThanPrecision_ShouldThrowArgumentException")]
        public void NumberValidatorCreation_Throws(int precision, int scale, bool onlyPositive)
        {
            Action ValidatorCreation = () => new NumberValidator(precision, scale, onlyPositive);
            ValidatorCreation.ShouldThrow<ArgumentException>();
        }

        [TestCase(2, 1, true, TestName = "PositiveScaleAndPrecision_ShouldNotThrowException")]
        [TestCase(100, 30, true, TestName = "PositiveScaleAndPrecision_ShouldNotThrowException")]
        [TestCase(1, 0, true, TestName = "ZeroScale_ShouldNotThrowException")]
        public void NumberValidatorCreation_ShouldNotThrows(int precision, int scale, bool onlyPositive)
        {
            Action ValidatorCreation = () => new NumberValidator(precision, scale, onlyPositive);
            ValidatorCreation.ShouldNotThrow<ArgumentException>();
        }

        [TestCase(17, 2, true, "0.0", ExpectedResult = true, TestName = "BothPartsLesserThanLimits_ShouldBeValid")]
        [TestCase(17, 2, true, "00.00", ExpectedResult = true, TestName = "ScaleEqualLimits_ShouldBeValid")]
        [TestCase(4, 2, true, "00.00", ExpectedResult = true, TestName = "PrecisionEqualLimits_ShouldBeValid")]
        [TestCase(17, 2, true, "0", ExpectedResult = true, TestName = "OnlyIntPart_ShouldBeValid")]
        [TestCase(10, 5, false, "-1.0", ExpectedResult = true, TestName = "InputNegative_ShouldBeValidIfValidatorAcceptNegative")]
        public bool Test_IsNumberValid_ValidCases(int precision, int scale, bool onlyPositive, string number)
        {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number);
        }

        [TestCase(17, 2, true, "", ExpectedResult = false, TestName = "EmptyInput_ShouldNotBeValid")]
        [TestCase(10, 5, true, null, ExpectedResult = false, TestName = "NullInput_ShouldNotBeValid")]
        [TestCase(3, 2, true, "111111", ExpectedResult = false, TestName = "OnlyIntPartGreaterThanScale_ShouldNotBeValid")]
        [TestCase(3, 2, true, "00.00", ExpectedResult = false, TestName = "ScaleGreaterThanLimits_ShouldNotBeValid")]
        [TestCase(17, 2, true, "0.000", ExpectedResult = false, TestName = "PrecisionGreaterThanLimits_ShouldNotBeValid")]
        [TestCase(17, 2, true, "-1.0", ExpectedResult = false, TestName = "InputNegative_ShouldNotBeValidIfValidatorNotAcceptNegative")]
        [TestCase(10, 5, true, "a.sd", ExpectedResult = false, TestName = "NotNumbers_ShouldNotBeValid")]
        [TestCase(10, 5, true, "ф.ыв", ExpectedResult = false, TestName = "NotNumbers_ShouldNotBeValid")]
        [TestCase(10, 5, true, "\n.\n\n", ExpectedResult = false, TestName = "NotNumbers_ShouldNotBeValid")]
        [TestCase(10, 5, true, "1.23asd", ExpectedResult = false, TestName = "IfContainLetters_ShouldNotBeValid")]
        [TestCase(10, 5, false, "-+1.23", ExpectedResult = false, TestName = "MultipleSign_ShouldNotBeValid")]
        [TestCase(10, 5, false, "1.-23", ExpectedResult = false, TestName = "SignInFracPart_ShouldNotBeValid")]
        public bool Test_IsNumberValid_NotValidCases(int precision, int scale, bool onlyPositive, string number)
        {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number);
        }

        [TestCase(3, 2, true, "+0.00", ExpectedResult = false, TestName = "NumberWithSign_SignShouldBePartOfScale")]
        [TestCase(4, 2, true, "+0.00", ExpectedResult = true, TestName = "NumberWithSign_SignShouldBePartOfScale")]
        public bool Test_NumberWithSign_SignShouldBePartOfScale(int precision, int scale, bool onlyPositive, string number)
        {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number);
        }

        [Test]
		public void CheckingThatClassNotStaticOnlyPositive_firstCreatedValidator()
		{
            var val1 = new NumberValidator(17, 10, true);
            var val2 = new NumberValidator(17, 10, false);
            val1.IsValidNumber("-1").Should().BeFalse();
        }

        [Test]
        public void NumberValidator_CheckingThatScaleNotStatic()
        {
            var val1 = new NumberValidator(5, 3, true);
            var val2 = new NumberValidator(10, 3, false);
            val1.IsValidNumber("000000.0").Should().BeFalse();
        }

        [Test]
        public void NumberValidator_CheckingThatPrecisionNotStatic()
        {
            var val1 = new NumberValidator(10, 3, true);
            var val2 = new NumberValidator(10, 8, false);
            val1.IsValidNumber("1.000000").Should().BeFalse();
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