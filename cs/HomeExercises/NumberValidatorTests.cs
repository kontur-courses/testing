using System;
using System.Collections;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void ShouldNotThrowsArgumentException_AfterInvokeWithValidArguments()
        {
            Action constructor = () => new NumberValidator(1, 0, true);
            constructor.ShouldNotThrow();
        }
		
		[TestCase(-1, 2, true, TestName = "ThrowsArgumentException_AfterInvokeWithNegativePrecisionAndOnlyPositiveNumbers")]
		[TestCase(1, 2, true, TestName = "ThrowsArgumentException_AfterInvokeWithScaleMoreThanPrecision")]
		[TestCase(0, -1, true, TestName = "ThrowsArgumentException_AfterInvokeWithNegativeScale")]
	    [TestCase(10, 10, true, TestName = "ThrowsArgumentException_AfterInvokeWithScaleEqualsToPrecision")]
        public void TestNumberValidator_ThrowsArgumentException(int precision, int scale, bool onlyPositive)
        {
		    Action constructor = () => new NumberValidator(precision, scale, onlyPositive);
		    constructor.ShouldThrow<ArgumentException>();
	    }

        [TestCase(4, 2, true, "+1.23", ExpectedResult = true, TestName = "IsValid_AfterSimpleInput")]
        [TestCase(3, 2, true, "+1.23", ExpectedResult = false, TestName = "IsNotValid_WhenActualPrecisionMoreThanRequired")]
        [TestCase(4, 2, true, "-1.00", ExpectedResult = false, TestName = "IsNotValid_AfterNegativeInputWhenOnlyPositiveRequired")]
        [TestCase(15, 2, true, "3000000000", ExpectedResult = true, TestName = "IsValid_AfterInvokeWithValueMoreThanInt32MaxValue")]

        [TestCase(17, 2, true, "0", ExpectedResult = true, TestName = "IsValid_OnInputWithoutSeparator")]
        [TestCase(17, 2, true, "0.0", ExpectedResult = true, TestName = "IsValid_WithoutSignInTheBeginning")]
        [TestCase(4, 2, true, "+0.0", ExpectedResult = true, TestName = "IsValid_WithPlusInTheBeginning")]
        [TestCase(4, 2, true, "+", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeOnlyWithSign")]

        [TestCase(17, 2, true, "0,0", ExpectedResult = true, TestName = "IsValid_AfterInvokeWithCommaSeparator")]
        [TestCase(17, 2, true, "0:0", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithColonSeparator")]
        [TestCase(17, 2, true, "0;0", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithSemiсolonSeparator")]

        [TestCase(17, 2, false, "-1.1000", ExpectedResult = false, TestName = "IsNotValid_WithNegativeNumberAndScaleMoreThanRequiredWhenOnlyPositiveFalse")]
        [TestCase(4, 2, false, "-0.0", ExpectedResult = true, TestName = "IsValid_AfterInvokeWithZeroAndMinusInTheBeginningWhenOnlyPositiveFalse")]
        [TestCase(4, 2, false, "-1.0", ExpectedResult = true, TestName = "IsValid_AfterInvokeWithNegativeNumberWhenOnlyPositiveTrueWhenOnlyPositiveFalse")]
        [TestCase(4, 2, false, "", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithEmptyStringWhenOnlyPositiveFalse")]

        [TestCase(17, 2, true, "0.000", ExpectedResult = false, TestName = "IsNotValid_WhenActualScaleMoreThanRequired")]
        [TestCase(3, 2, true, "1.111", ExpectedResult = false, TestName = "IsNotValid_WhenActualPrecisionMoreThanRequired")]

        [TestCase(3, 2, true, "", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithEmptyString")]
        [TestCase(3, 2, true, null, ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithNullString")]

        [TestCase(17, 2, true, "a.sd", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithStringInsteadOfNumbers")]
        [TestCase(17, 2, true, "A.SD", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithStringInUpperCase")]
        [TestCase(17, 2, true, "ф.ыв", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithStringWithRussianLetters")]
        [TestCase(17, 2, true, "a.00", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithStringMixWithNumbers")]

        [TestCase(17, 2, true, "0..0", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithDoubleSeparator")]
        [TestCase(17, 4, true, "0.0.0", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithDoubleFraction")]
        [TestCase(17, 4, true, "0. 0", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithSpace")]
        [TestCase(17, 4, true, "0.\n0", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithLineBreak")]
        [TestCase(17, 4, true, "0.\"0\"", ExpectedResult = false, TestName = "IsNotValid_AfterInvokeWithQuotedFraction")]
        public bool TestNumberValidator_NumberIsValid
            (int precision, int scale, bool onlyPositive, String numberToValid)
        {
            return new NumberValidator(precision, scale, onlyPositive)
                .IsValidNumber(numberToValid);
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