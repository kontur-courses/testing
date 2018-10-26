using System;
using System.Collections;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void TestNumberValidator_ThrowsArgumentException_AfterInvokeWithNegativePrecisionAndOnlyPositiveNumbers()
        {
            Action constructor = () => new NumberValidator(-1, 2, true);
            constructor.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void TestNumberValidator_ShouldNotThrowsArgumentException_AfterInvokeWithPositivePrecision()
        {
            Action constructor = () => new NumberValidator(1, 0, true);
            constructor.ShouldNotThrow();
        }

        [Test]
        public void TestNumberValidator_ThrowsArgumentException_AfterInvokeWithScaleMoreThanPrecision()
        {
            Action constructor = () => new NumberValidator(1, 2, true);
            constructor.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void TestNumberValidator_ThrowsArgumentException_AfterInvokeWithNegativeScale()
        {
            Action constructor = () => new NumberValidator(0, -1, true);
            constructor.ShouldThrow<ArgumentException>();
        }


        [Test, TestCaseSource(nameof(NumberValidatorTestCases))]
        public bool TestNumberValidator_NumberIsValid
            (int precision, int scale, bool onlyPositive, String numberToValid)
        {
            return new NumberValidator(precision, scale, onlyPositive)
                .IsValidNumber(numberToValid);
        }

        public static IEnumerable NumberValidatorTestCases
        {
            get
            {
                yield return new TestCaseData(17, 2, true, "0.0").Returns(true);
                yield return new TestCaseData(17, 2, true, "0").Returns(true);
                yield return new TestCaseData(3, 2, true, "00.00").Returns(false);
                yield return new TestCaseData(3, 2, true, "+0.00").Returns(false);
                yield return new TestCaseData(4, 2, true, "+1.23").Returns(true);
                yield return new TestCaseData(3, 2, true, "+1.23").Returns(false);
                yield return new TestCaseData(3, 2, true, "-1.23").Returns(false);

                yield return new TestCaseData(17, 2, true, "0.000").Returns(false)
                    .SetName("TestNumberValidator_ReturnFalse_WhenActualScaleMoreThanRequired");
                yield return new TestCaseData(3, 2, true, "a.sd")
                    .Returns(false).SetName("TestNumberValidator_ReturnFalse_AfterInvokeWithStringInsteadOfNumbers");
                yield return new TestCaseData(3, 2, true, "")
                    .Returns(false).SetName("TestNumberValidator_ReturnFalse_AfterInvokeWithEmptyString");
                yield return new TestCaseData(3, 2, true, null)
                    .Returns(false).SetName("TestNumberValidator_ReturnFalse_AfterInvokeWithNullString");
                yield return new TestCaseData(17, 2, true, "0,0")
                    .Returns(true).SetName("TestNumberValidator_ReturnTrue_AfterInvokeWithCommaSeparator");
                yield return new TestCaseData(3, 2, true, "-0.00")
                    .Returns(false).SetName("TestNumberValidator_ReturnFalse_AfterNegativeInputWhenOnlyPositiveRequired");
            }
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