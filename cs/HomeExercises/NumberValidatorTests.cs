using System;
using System.Collections;
using System.Collections.Generic;
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

        [TestCaseSource(typeof(NumberValidatorData), nameof(NumberValidatorData.TestCaseData))]
        public void IsValidNumber_Tests
            (int precision, int scale, bool positive, string value, bool expected)
        {
            var validator = new NumberValidator(precision, scale, positive);

            var actual = validator.IsValidNumber(value);

            Assert.AreEqual(expected, actual);
        }
    }

    public class NumberValidatorData
    {
        public static IEnumerable TestCaseData
        {
            get
            {
                //IsValidNumber_WithProperValue_Success
                yield return new TestCaseData(17, 2, true, "0.0", true);
                yield return new TestCaseData(17, 2, true, "0", true);
                yield return new TestCaseData(17, 2, true, "+0.0", true);
                yield return new TestCaseData(3, 1, true, "+1.2", true);
                yield return new TestCaseData(3, 1, false, "-1.2", true);

                //IsValidNumber_WithIncorrectValue_ReturnFalse
                yield return new TestCaseData(10, 5, false, " ", false);
                yield return new TestCaseData(10, 5, false, "", false);
                yield return new TestCaseData(10, 5, false, null, false);
                yield return new TestCaseData(10, 5, false, "+-1", false);
                yield return new TestCaseData(10, 5, false, "abc", false);
                yield return new TestCaseData(10, 5, false, "a.bc", false);
                yield return new TestCaseData(10, 5, false, "1.0.1", false);

                //IsValidNumber_WithValueGreaterThanPrecision_ReturnFalse
                yield return new TestCaseData(2, 1, false, "-1.0", false);
                yield return new TestCaseData(2, 1, false, "+1.0", false);
                yield return new TestCaseData(2, 1, false, "10.0", false);
                yield return new TestCaseData(2, 1, false, "1.00", false);

                //IsValidNumber_WithFracPartGreaterThanScale_ReturnFalse
                yield return new TestCaseData(10, 1, true, "1.00", false);
                yield return new TestCaseData(10, 1, false, "-1.00", false);

                //IsValidNumber_OnlyPositiveWithNegativeValue_ReturnFalse
                yield return new TestCaseData(10, 5, true, "-1.0", false);
            }
        }
    }
}