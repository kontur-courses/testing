using System;
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

        [TestCase(-2, 2, TestName = "NumberValidator_WithNegativePrecision_ThrowsArgumentException")]
        [TestCase(2, -2, TestName = "NumberValidator_WithNegativeScale_ThrowsArgumentException")]
        [TestCase(2, 4, TestName = "NumberValidator_WithScaleGreaterThanPrecision_ThrowsArgumentException")]
        public void NumberValidator_InvalidParameters_ThrowsArgumentException(int precision, int scale)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, true));
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

            var actual = validator.IsValidNumber(value);

            Assert.True(actual);
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
            var validator = new NumberValidator(10, 5);

            var actual = validator.IsValidNumber(value);

            Assert.False(actual);
        }


        [TestCase("-1.0")]
        [TestCase("+1.0")]
        [TestCase("10.0")]
        [TestCase("1.00")]
        public void IsValidNumber_WithValueGreaterThanPrecision_ReturnFalse(string value)
        {
            var validator = new NumberValidator(2, 1);

            var actual = validator.IsValidNumber(value);

            Assert.False(actual);
        }

        [TestCase(true, "1.00")]
        [TestCase(false, "-1.00")]
        public void IsValidNumber_WithFracPartGreaterThanScale_ReturnFalse(bool positive, string value)
        {
            var validator = new NumberValidator(10, 1, positive);

            var actual = validator.IsValidNumber(value);

            Assert.False(actual);
        }

        [Test]
        public void IsValidNumber_OnlyPositiveWithNegativeValue_ReturnFalse()
        {
            var validator = new NumberValidator(10, 5, true);

            var actual = validator.IsValidNumber("-1.0");

            Assert.False(actual);
        }
    }
}