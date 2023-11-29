using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    [TestFixture(TestOf = typeof(NumberValidator))]
    public class NumberValidatorTests
    {
        private static TestCaseData[] ArgumentExceptionTestCases =
        {
            new TestCaseData(-1, 2, true).SetName("WhenPassNegativePrecision"),
            new TestCaseData(1, -2, true).SetName("WhenPassNegativeScale"),
            new TestCaseData(2, 2, true).SetName("WhenPrecisionIsEqualToTheScale")
        };

        [TestCaseSource(nameof(ArgumentExceptionTestCases))]
        public void NumberValidatorCtor_WhenPassInvalidArguments_ShouldThrowArgumentException(int precision,
            int scale, bool onlyPositive) =>
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));

        [Test]
        public void NumberValidatorCtor_WhenPassValidArguments_ShouldNotThrows() =>
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

        private static TestCaseData[] ArgumentTestCases =
        {
            new TestCaseData(3,2,true,"a.sd", false).SetName("WhenLettersInsteadOfNumber"),
            new TestCaseData(3,2,true,"2.!", false).SetName("WhenSymbolsInsteadOfNumber"),
            new TestCaseData(3,2,true,"2,3", true).SetName("WhenCharactersAreSeparatedByComma"),
            new TestCaseData(3,2,true,null!, false).SetName("WhenPassNumberIsNull"),
            new TestCaseData(3,2,true,"2,.3", false).SetName("WhenTwoSeparatorsArePassed"),
            new TestCaseData(3,2,true,"2 3", false).SetName("WhenSeparatedBySpace"),
            new TestCaseData(3,2,true,"", false).SetName("WhenPassNumberIsEmpty"),
            new TestCaseData(3,2,true,"-0.00", false).SetName("WhenIntPartWithNegativeSignMoreThanPrecision"),
            new TestCaseData(3,2,true,"+1.23", false).SetName("WhenIntPartWithPositiveSignMoreThanPrecision"),
            new TestCaseData(3,2,true,"0.000", false).SetName("WhenFractionalPartMoreThanScale"),
            new TestCaseData(3,2,true,"0", true).SetName("WhenFractionalPartIsMissing"),
            new TestCaseData(3,2,true,"0.0", true).SetName("WhenNumberIsValid")
        };

        private NumberValidator GetCorrectNumberValidator(int precision, int scale, bool onlyPositive) =>
            new NumberValidator(precision, scale, onlyPositive);

        [TestOf(nameof(NumberValidator.IsValidNumber))]
        [TestCaseSource(nameof(ArgumentTestCases))]
        public void WhenPassInvalidArguments_ShouldReturnFalse(int precision, int scale,
            bool onlyPositive, string number, bool expectedResult)
        {
            var correctValidator = GetCorrectNumberValidator(precision, scale, onlyPositive);

            Assert.AreEqual(expectedResult, correctValidator.IsValidNumber(number));
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