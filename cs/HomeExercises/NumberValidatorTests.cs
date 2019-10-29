using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {

	    [TestCase( -1,2, TestName = "When_PrecisionIsNegative")]
	    [TestCase(1, -2, TestName = "When_ScaleIsNegative")]
	    [TestCase(1, 3, TestName = "When_ScaleIsBiggerThanPrecision")]
	    [TestCase(1, 1, TestName = "When_ScaleIsEqualToPrecision")]

	    public void ShouldThrowArgumentException(int precision, int scale)
	    {
		    Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
	    }

	    [Test]
        public void ShouldNotBeValid_When_ExpectedPrecisionMoreThanActual()
        {
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("0.001"));
        }

        [Test]
        public void ShouldNotBeValid_When_ExpectPositiveNumberButActualIsNonPositive()
        {
            Assert.IsFalse(new NumberValidator(2, 0, true).IsValidNumber("-1"));
        }

		[TestCase("0",ExpectedResult = true,TestName = "WhenActualIsZero")]
		[TestCase("-1",ExpectedResult = true,TestName = "WhenActualIsNegative")]
		[TestCase("1",ExpectedResult = true,TestName = "WhenActualIsPositive")]
        public bool ShouldBeValid_When_ExpectNonPositiveNumberAndActualIsPositiveOrNegativeOrZero(string arg)
        {
	        return new NumberValidator(2, 0, false).IsValidNumber(arg);
        }

        [Test]
        public void ShouldNotBeValid_When_ExpectPrecisionLessThanActual()
        {
            Assert.IsFalse(new NumberValidator(3, 0, true).IsValidNumber("-123"));
        }

        [Test]
        public void ShouldNotBeValid_When_ExpectScaleLessThanActual()
        {
            Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber("1.234"));
        }

        [TestCase(null, ExpectedResult = false, TestName = "ValueIsNull")]
        [TestCase("", ExpectedResult = false, TestName = "ValueIsEmptyString")]
        public bool ShouldNotBeValid_WhenValueIsEmptyOrNull(string arg)
        {
	        return new NumberValidator(4, 2, true).IsValidNumber(arg);
        }

        [TestCase("2,24",TestName = "When_CommaIsUsedInValue")]
        [TestCase("2.24", TestName = "When_PointIsUsedInValue")]
        public void ShouldBeValid_When_UseDiffrentDecimalSeparators(string arg)
        {
            Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber(arg));
        }

        [TestCase("aaaa",TestName = "When_OnlyNoNumbersInValue")]
        [TestCase("2, 24$", TestName = "When_NumbersAndNoNumbersInValue")]
        public void ShouldNotBeValid_When_NoNumbersInValue(string arg)
        {
            Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber("2,24$"));
        }

        [Test]
        public void ShouldBeValid_When_TwoZeroInIntPart()
        {
            Assert.IsTrue(new NumberValidator(2, 0, true).IsValidNumber("00"));
        }

        [Test]
        public void ShouldBeValid_When_BigIntNumber()
        {
            Assert.IsTrue(new NumberValidator(50, 0, true).IsValidNumber(new String('1', 50)));
        }

        [Test]
        public void ShouldBeValid_When_BigScale()
        {
            Assert.IsTrue(new NumberValidator(325, 320, true).IsValidNumber(String.Format("0,{0}", new String('1', 320))));
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