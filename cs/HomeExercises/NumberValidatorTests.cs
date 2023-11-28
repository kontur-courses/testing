using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {
        private static IEnumerable<TestCaseData> IsValidNumberPrecisionTests = new []
        {
            new TestCaseData(3, 2, true, "+0.00").Returns(false)
            .SetName("False_WhenSymbolWithNumberLengthGreaterThanPrecision"),
            new TestCaseData(3, 2, true, "00.00").Returns(false)
            .SetName("False_WhenIntPartWithFracPartGreaterThanPrecision"),

            new TestCaseData(17, 2, true, "0").Returns(true)
            .SetName("True_WhenNumberLengthNotGreaterThanPrecision"),
            new TestCaseData(4, 2, true, "+1.23").Returns(true)
            .SetName("True_WhenPositiveSymbolWithNumberLengthNotGreaterThanPrecision"),
            new TestCaseData(4, 2, false, "-1.23").Returns(true)
            .SetName("True_WhenNegativeSymbolWithNumberLengthNotGreaterThanPrecision")
        };

        private static IEnumerable<TestCaseData> IsValidNumberScaleTests = new []
        {
            new TestCaseData(17, 2, true, "0.000").Returns(false)
            .SetName("False_WhenFracPartGreaterThanScale"),
            new TestCaseData(17, 2, true, "0.0").Returns(true)
            .SetName("True_WhenFracPartNotGreaterThanScale")
        };

        private static IEnumerable<TestCaseData> IsValidNumberPositivityTests = new []
        {
            new TestCaseData(3, 2, true, "-0.00").Returns(false)
            .SetName("False_WhenAcceptsOnlyPositiveButGivenNegativeNumber"),
            new TestCaseData(3, 2, false, "-0.0").Returns(true)
            .SetName("True_WhenAcceptsAnyAndGivenNegativeNumber")
        };

        private static IEnumerable<TestCaseData> IsValidNumberSymbolsTests = new []
        {
            new TestCaseData(3, 2, true, "a.sd").Returns(false).SetName("False_WhenGivenNotDigits"),
            new TestCaseData(17, 2, true, "").Returns(false).SetName("False_WhenEmptyStringGiven")
        };

        [TestCaseSource(nameof(IsValidNumberPositivityTests))]
        [TestCaseSource(nameof(IsValidNumberPrecisionTests))]
        [TestCaseSource(nameof(IsValidNumberScaleTests))]
        [TestCaseSource(nameof(IsValidNumberSymbolsTests))]
        public bool IsValidNumber_Returns(int precision, int scale, bool onlyPositive, string number) =>
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number);


        private static IEnumerable<TestCaseData> ConstructorArgumentExceptions = new[]
        {
            new TestCaseData(-1, 2, true).SetName("WhenPercisionNotPositive"),
            new TestCaseData(1, 2, true).SetName("WhenScaleGreaterThanPercision"),
            new TestCaseData(1, -1, true).SetName("WhenScaleNotPositive"),
            new TestCaseData(1, 1, true).SetName("WhenScaleEqualsPerci  sion")
        };

        [TestCaseSource(nameof(ConstructorArgumentExceptions))]
        public void Constructor_ThrowsArgumentException(int precision, int scale, bool onlyPositive) =>
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
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