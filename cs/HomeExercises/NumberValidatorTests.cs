using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void NumberValidator_WithProperParameters_Success()
        {
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
        }

        [Test]
        public void NumberValidator_WithNegativePrecision_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
        }

        [Test]
        public void NumberValidator_WithNegativeScale_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(2, -2, true));
        }

        [Test]
        public void NumberValidator_WithScaleGreaterThanPrecision_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(2, 4, true));
        }

        [TestCase(17, 2, true, "0.0")]
        [TestCase(17, 2, true, "0")]
        [TestCase(17, 2, true, "+0.0")]
        [TestCase(3, 1, true, "+1.2")]
        [TestCase(3, 1, false, "-1.2")]
        public void IsValidNumber_WithProperValue_Success
            (int precision, int scale, bool positive, string value)
        {
            var validator = new NumberValidator(precision, scale, positive);

            Assert.True(validator.IsValidNumber(value));
        }

        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("+-1")]
        [TestCase("abc")]
        [TestCase("a.bc")]
        [TestCase("1.0.1")]
        public void IsValidNumber_WithIncorrectValue_ReturnFalse(string value)
        {
            Assert.False(new NumberValidator(10, 5).IsValidNumber(value));
        }


        [TestCase("-1.0")]
        [TestCase("+1.0")]
        [TestCase("10.0")]
        [TestCase("1.00")]
        public void IsValidNumber_WithValueGreaterThanPrecision_ReturnFalse(string value)
        {
            var validator = new NumberValidator(2, 1);

            Assert.False(validator.IsValidNumber(value));
        }

        [TestCase(true, "1.00")]
        [TestCase(false, "-1.00")]
        public void IsValidNumber_WithFracPartGreaterThanScale_ReturnFalse(bool positive, string value)
        {
            var validator = new NumberValidator(10, 1, positive);

            Assert.False(validator.IsValidNumber(value));
        }

        public void IsValidNumber_OnlyPositiveWithNegativeValue_ReturnFalse()
        {
            var validator = new NumberValidator(10, 5, true);

            Assert.False(validator.IsValidNumber("-1.0"));
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