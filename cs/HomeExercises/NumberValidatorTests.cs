using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [TestCase(-1, 0, "precision must be a positive number", TestName = "precision is negative")]
        [TestCase(0, 0, "precision must be a positive number", TestName = "precision is zero")]
        [TestCase(1, -1, "precision must be a non-negative number less or equal than precision", TestName = "scale is negative")]
        [TestCase(1, 1, "precision must be a non-negative number less or equal than precision", TestName = "scale equals precision")]
        [TestCase(1, 2, "precision must be a non-negative number less or equal than precision", TestName = "scale is greater than precision")]
        public void Constructor_ThrowsArgumentExceptionWithMessage_IfPrecisionOrScaleIsIncorrect(int precision, int scale, string message)
        {
            FluentActions
                .Invoking(() => new NumberValidator(precision, scale))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage(message);
        }

        [TestCase(null, TestName = "string is null")]
        [TestCase("", TestName = "string is empty")]
        [TestCase(" ", TestName = "string is whitespace")]
        [TestCase(" 1", TestName = "whitespace before number")]
        [TestCase(" +1", TestName = "whitespace before signed number")]
        [TestCase("1+1", TestName = "sign between digits")]
        [TestCase("++1", TestName = "double sign")]
        [TestCase("+ 1", TestName = "whitespace between sign and digit")]
        [TestCase("+1+", TestName = "sign before and after digit")]
        [TestCase("+ ", TestName = "whitespace after sign without digit")]
        [TestCase("+1 ", TestName = "whitespace after signed number")]
        [TestCase("+a", TestName = "sign and letter")]
        [TestCase("a", TestName = "only letter")]
        [TestCase(".", TestName = "only dot")]
        [TestCase("+.", TestName = "plus and dot")]
        [TestCase("-.", TestName = "minus and dot")]
        [TestCase(",", TestName = "only comma")]
        [TestCase("+,", TestName = "plus and comma")]
        [TestCase("-,", TestName = "minus and comma")]
        [TestCase(".1", TestName = "dot and fractal part")]
        [TestCase("1.", TestName = "integer part and dot")]
        [TestCase("1..", TestName = "double dot after integer part")]
        [TestCase(",1", TestName = "comma and fractal part")]
        [TestCase("1,", TestName = "integer part and comma")]
        [TestCase("1,,", TestName = "double comma after integer part")]
        [TestCase("1+", TestName = "plus after integer part")]
        [TestCase("1-", TestName = "minus after integer part")]
        [TestCase("1 ", TestName = "whitespace after integer part")]
        [TestCase("1a", TestName = "letter after integer part")]
        [TestCase("1.+", TestName = "plus after integer part and dot")]
        [TestCase("1.-", TestName = "minus after integer part and dot")]
        [TestCase("1. ", TestName = "whitespace after integer part and dot")]
        [TestCase("1.a", TestName = "letter after integer part and dot")]
        [TestCase("1.1.", TestName = "dot after decimal with dot")]
        [TestCase("1.1,", TestName = "comma after decimal with dot")]
        [TestCase("1.1+", TestName = "plus after decimal with dot")]
        [TestCase("1.1-", TestName = "minus after decimal with dot")]
        [TestCase("1.1 ", TestName = "whitespace after decimal with dot")]
        [TestCase("1.1a", TestName = "letter after decimal with dot")]
        [TestCase("1,+", TestName = "plus after integer part and comma")]
        [TestCase("1,-", TestName = "minus after integer part and comma")]
        [TestCase("1, ", TestName = "whitespace after integer part and comma")]
        [TestCase("1,a", TestName = "letter after integer part and comma")]
        [TestCase("1,1,", TestName = "comma after decimal with comma")]
        [TestCase("1,1.", TestName = "dot after decimal with comma")]
        [TestCase("1,1+", TestName = "plus after decimal with comma")]
        [TestCase("1,1-", TestName = "minus after decimal with comma")]
        [TestCase("1,1 ", TestName = "whitespace after decimal with comma")]
        [TestCase("1,1a", TestName = "letter after decimal with comma")]
        public void IsValidNumber_ReturnsFalse_IfStringNumberFormatIsIncorrect(string inputString)
        {
            new NumberValidator(10).IsValidNumber(inputString).Should().BeFalse();
        }

        [TestCase("0", true, TestName = "unsigned integer")]
        [TestCase("0.0", true, TestName = "unsigned double with dot")]
        [TestCase("0,0", true, TestName = "unsigned double with comma")]
        [TestCase("00.00", true, TestName = "unsigned multi-digit double with dot")]
        [TestCase("00,00", true, TestName = "unsigned multi-digit double with comma")]
        [TestCase("+0", true, TestName = "signed integer")]
        [TestCase("+0.0", true, TestName = "signed double with dot")]
        [TestCase("+0,0", true, TestName = "signed double with comma")]
        [TestCase("+00.00", true, TestName = "signed multi-digit double with dot")]
        [TestCase("+00,00", true, TestName = "signed multi-digit double with comma")]
        [TestCase("-0", false, TestName = "negative integer")]
        [TestCase("-0.0", false, TestName = "negative double with dot")]
        [TestCase("-0,0", false, TestName = "negative double with comma")]
        [TestCase("-00.00", false, TestName = "negative multi-digit double with dot")]
        [TestCase("-00,00", false, TestName = "negative multi-digit double with comma")]
        public void IsValidNumber_ReturnsTrue_IfStringNumberFormatIsCorrect(string inputString, bool onlyPositive)
        {
            new NumberValidator(10, 5, onlyPositive).IsValidNumber(inputString).Should().BeTrue();
        }


        [TestCase(3, 2, "00.00", true, TestName = "unsigned number length is larger than precision")]
        [TestCase(3, 2, "+0.00", true, TestName = "positive number length is larger than precision")]
        [TestCase(3, 2, "-0.00", false, TestName = "negative number length is larger than precision")]
        [TestCase(10, 2, "0.000", true, TestName = "fractional part length is larger than scale")]
        public void IsValidNumber_ReturnsFalse_IfNumberLengthDoesNotMatchPrecisionOrScale(int precision, int scale, string inputString, bool onlyPositive)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(inputString).Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_IfInputStringRepresentsNegativeNumberAndOnlyPositiveIsTrue()
        {
            new NumberValidator(10, 5, true).IsValidNumber("-1").Should().BeFalse();
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