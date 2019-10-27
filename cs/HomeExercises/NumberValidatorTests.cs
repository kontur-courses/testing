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
        public void ShouldThrowArgumentExeption_When_PresisionIsNegative()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2));
        }
        [Test]
        public void ShouldThrowArgumentExeption_When_ScaleIsNegative()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(1, -2));
        }
        [Test]
        public void ShouldThrowArgumentExeption_When_ScaleIsBiggerThanPresision()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(1, 3));
        }
        [Test]
        public void ShouldNotBeValid_When_ExpectedPresisionMoreThanActual()
        {
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("0.001"));
        }

        [Test]
        public void ShouldNotBeValid_When_ExpectePositiveNumberButActualIsNegative()
        {
            Assert.IsFalse(new NumberValidator(2, 0, true).IsValidNumber("-0"));
        }

        [Test]
        public void ShouldBeValid_When_ExpecteNegativeNumberButActualIsPositive()
        {
            Assert.IsTrue(new NumberValidator(2, 0, false).IsValidNumber("0"));
        }

        [Test]
        public void ShouldBeValid_When_ExpectNegativeNumberAndActualIsNegative()
        {
            Assert.IsTrue(new NumberValidator(2, 0, false).IsValidNumber("-0"));
        }

        [Test]
        public void ShouldBeValid_When_ExpectPositiveNumberAndActualIsPositive()
        {
            Assert.IsTrue(new NumberValidator(2, 0, true).IsValidNumber("0"));
        }

        [Test]
        public void ShouldNotBeValid_When_ExpectPresisionLessThanActual()
        {
            Assert.IsFalse(new NumberValidator(3, 0, true).IsValidNumber("-123"));
        }

        [Test]
        public void ShouldNotBeValid_When_ExpectScaleLessThanActual()
        {
            Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber("1.234"));
        }

        [Test]
        public void ShouldNotBeValid_When_ValueIsEmpty()
        {
            Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber(""));
        }

        [Test]
        public void ShouldNotBeValid_When_NoNumberInValue()
        {
            Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber("aaaa"));
        }

        [Test]
        public void ShouldBeValid_When_UseCommaInNumber()
        {
            Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("2,24"));
        }

        [Test]
        public void ShouldNotBeValid_When_NumbersAndNoNumbersInValue()
        {
            Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber("2,24$"));
        }

        [Test]
        public void ShouldBeValid_When_TwoZeroInIntPart()
        {
            Assert.IsTrue(new NumberValidator(2, 0, true).IsValidNumber("00"));
        }

        [Test]
        public void ShouldBeValid_When_BigNumber()
        {
            Assert.IsTrue(new NumberValidator(50, 0, true).IsValidNumber("11111111111111111111111111111111111111111111111111"));
        }

        [Test]
        public void ShouldBeValid_When_BigScale()
        {
            Assert.IsTrue(new NumberValidator(51, 50, true).IsValidNumber("0,11111111111111111111111111111111111111111111111111"));
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