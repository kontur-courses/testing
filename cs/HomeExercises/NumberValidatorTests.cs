using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using static FluentAssertions.FluentActions;
using NUnit.Framework;
using System.Collections;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {
        public static IEnumerable IsValidNumberPrecisionTests
        {
            get
            {
                yield return new TestCaseData(3, 2, true, "+0.00").Returns(false)
                    .SetName("IsValidNumber_ReturnsFalse_WhenSymbolWithNumberLengthGreaterThanPrecision");
                yield return new TestCaseData(3, 2, true, "00.00").Returns(false)
                    .SetName("IsValidNumber_ReturnsFalse_WhenIntPartWithFracPartGreaterThanPrecision");
                yield return new TestCaseData(17, 2, true, "0").Returns(true)
                    .SetName("IsValidNumber_ReturnsTrue_WhenNumberLengthNotGreaterThanPrecision");
                yield return new TestCaseData(4, 2, true, "+1.23").Returns(true)
                    .SetName("IsValidNumber_ReturnsTrue_WhenPositiveSymbolWithNumberLengthNotGreaterThanPrecision");
                yield return new TestCaseData(4, 2, false, "-1.23").Returns(true)
                    .SetName("IsValidNumber_ReturnsTrue_WhenNegativeSymbolWithNumberLengthNotGreaterThanPrecision");
            }
        }
        public static IEnumerable IsValidNumberScaleTests
        {
            get
            {
                yield return new TestCaseData(17, 2, true, "0.000").Returns(false)
                    .SetName("IsValidNumber_ReturnsFalse_WhenFracPartGreaterThanScale");
                yield return new TestCaseData(17, 2, true, "0.0").Returns(true)
                    .SetName("IsValidNumber_ReturnsTrue_WhenFracPartNotGreaterThanScale");
            }
        }
        public static IEnumerable IsValidNumberPositivityTests
        {
            get
            {
                yield return new TestCaseData(3, 2, true, "-0.00").Returns(false)
                    .SetName("IsValidNumber_ReturnsFalse_WhenAcceptsOnlyPositiveButGivenNegativeNumber");
                yield return new TestCaseData(3, 2, false, "-0.0").Returns(true)
                    .SetName("IsValidNumber_ReturnsTrue_WhenAcceptsAnyAndGivenNegativeNumber");
            }
        }
        public static IEnumerable IsValidNumberSymbolsTests
        {
            get
            {
                yield return new TestCaseData(3, 2, true, "a.sd").Returns(false)
                    .SetName("IsValidNumber_ReturnsFalse_WhenGivenNotDigits");
                yield return new TestCaseData(17, 2, true, "").Returns(false)
                    .SetName("IsValidNumber_ReturnsFalse_WhenEmptyStringGiven");
            }
        }


        public static IEnumerable ConstructorArgumentExceptions
        {
            get
            {
                yield return new TestCaseData(-1,2,true)
                    .SetName("Constructor_ThrowsArgumentExceptionWhenPercisionNotPositive");
                yield return new TestCaseData(1,2,true)
                    .SetName("Constructor_ThrowsArgumentExceptionWhenScaleGreaterThanPercision");
                yield return new TestCaseData(1,-1,true)
                    .SetName("Constructor_ThrowsArgumentExceptionWhenScaleNotPositive");
                yield return new TestCaseData(1,1,true)
                    .SetName("Constructor_ThrowsArgumentExceptionWhenScaleEqualsPercision");
            }
        }

        [Test, TestCaseSource(nameof(IsValidNumberPositivityTests)), 
            TestCaseSource(nameof(IsValidNumberPrecisionTests)),
            TestCaseSource(nameof(IsValidNumberScaleTests)),
            TestCaseSource(nameof(IsValidNumberSymbolsTests))]
        public bool IsValidNumber_Returns(int precision, int scale, bool onlyPositive, string number)
        {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number);
        }

        [Test, TestCaseSource(nameof(ConstructorArgumentExceptions))]
        public void Constructor_ThrowsArgumentException(int precision, int scale, bool onlyPositive)
        {
            Invoking(() => new NumberValidator(precision, scale, onlyPositive)).Should().Throw<ArgumentException>();
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