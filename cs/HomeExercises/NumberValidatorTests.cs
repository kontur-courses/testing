using FluentAssertions;
using NUnit.Framework;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
	    [TestCase(-1, 2, true, TestName = "ThrowsException_WhenPrecisionIsNegative")]
	    [TestCase(1, 2, true, TestName = "ThrowsException_WhenScaleBiggerThenPrecision")]
	    [TestCase(1, -2, true, TestName = "ThrowsException_WhenScaleIsNegative")]
	    [TestCase(1, 0, false, TestName = "DoesNotThrowsException_WhenValidData")]
        public void ExceptionTests(int precision, int scale, bool shouldThrow)
	    {
		    Action validator = () => new NumberValidator(precision, scale);
			if (shouldThrow)
				validator.ShouldThrow<ArgumentException>();
			else
				validator.ShouldNotThrow<ArgumentException>();
        }

        [TestCase(17, 2, true, "0.0", TestName = "IsValid_WhenIntAndFrancPartLessThanPrecisionAndFrancLessThanScale", ExpectedResult = true)]
        [TestCase(4, 2, true, "+1.23", TestName = "IsValid_WhenIntAndFrancPartLessThanPrecisionAndFrancLessThanScaleWithSign", ExpectedResult = true)]
        [TestCase(3, 2, true, "00.00", TestName = "IsNotValid_WhenIntAndFrancPartBiggerThanPrecision", ExpectedResult = false)]
        [TestCase(3, 2, true, "-0.00", TestName = "IsNotValid_WhenIntAndFrancPartWithSignBiggerThanPrecision", ExpectedResult = false)]
        [TestCase(3, 2, true, "+0.00", TestName = "IsNotValid_WhenIntAndFrancPartWithSignBiggerThanPrecision", ExpectedResult = false)]
        [TestCase(17, 2, true, "0.000", TestName = "IsNotValid_WhenFrancBiggerThenScale", ExpectedResult = false)]
        [TestCase(3, 2, true, "a.sd", TestName = "IsNotValid_WhenNotANumber", ExpectedResult = false)]
        [TestCase(3, 2, true, null, TestName = "IsNotValid_WhenNull", ExpectedResult = false)]
        [TestCase(3, 2, true, "abrakadabra", TestName = "IsNotValid_WhenAbrakadabra", ExpectedResult = false)]
        [TestCase(17, 2, true, "-5.7", TestName = "IsNotValid_WhenNegativeIfOnlyPositive", ExpectedResult = false)]
        [TestCase(17, 2, true, "0,0", TestName = "IsValid_WithComma", ExpectedResult = true)]
        [TestCase(3, 2, true, "", TestName = "IsNotValid_WhenEmpty", ExpectedResult = false)]
        public bool Test(int precision, int scale, bool onlyPositive, string value)
        {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
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
                throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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