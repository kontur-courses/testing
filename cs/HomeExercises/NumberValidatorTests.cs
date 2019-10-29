using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorConstructorTests
    {
        [TestCase(-1, TestName = "precision is negative")]
        [TestCase(0, TestName = "precision is zero")]
        [TestCase(1, -5, TestName = "scale is negative")]
        [TestCase(5, 8, TestName = "scale is bigger than precision")]
        [TestCase(7, 7, TestName = "scale and precision are equal")]
        public void Should_ThrowArgumentException_When(int precision, int scale = 0)
        {
            Action action = () => new NumberValidator(precision, scale);

            action.Should().Throw<ArgumentException>();
        }

        [TestCase(100, TestName = "precision is positive")]
        [TestCase(100, 0, TestName = "scale is zero")]
        [TestCase(100, 19, TestName = "scale is positive")]
        [TestCase(9, 3, TestName = "scale is less than precision")]
        [TestCase(100, 2, true, TestName = "non default scale and onlyPositive parameters are used (correct ones)")]
        public void Should_Not_ThrowAnyExceptions_When(int precision, int scale = 0, bool onlyPositive = false)
        {
            Action action = () => new NumberValidator(precision, scale, onlyPositive);

            action.Should().NotThrow<Exception>();
        }
    }

    [TestFixture]
    public class NumberValidatorIsValidNumberTests
    {
        [TestCase(null, 10, 2, TestName = "input is null")]
        [TestCase("", 10, 2, TestName = "input is empty")]
        [TestCase("1j.l3", 10, 2, TestName = "input contains non digit elements")]
        [TestCase("0..23", 10, 2, TestName = "input has wrong int and frac part dividers")]
        [TestCase(".0.23", 10, 2, TestName = "input has wrong starting symbols")]
        [TestCase("0.23.", 10, 2, TestName = "input has wrong ending symbols")]
        [TestCase("abc 0.2 abc", 10, 2, TestName = "only substring of input string matches a numberRegex")]
        [TestCase("00.0", 2, 1, TestName = "input is longer than precision (even if there are useless zeroes)")]
        [TestCase("005", 2, TestName = "input is longer than precision (even if there are useless zeroes at start)")]
        [TestCase("10.2", 2, 1, TestName = "input is longer than precision (int part is too long)")]
        [TestCase("-1.2", 2, 1, TestName = "input is longer than precision (because of sign -)")]
        [TestCase("+1.2", 2, 1, TestName = "input is longer than precision (because of sign +)")]
        [TestCase("1.20", 2, 1, TestName = "input frac part is longer than scale")]
        [TestCase("1.0", 2, 0, TestName = "scale is zero, but input has frac part")]
        [TestCase("1.0", 2, 0, TestName = "scale is zero, but input has frac part")]
        [TestCase("-34.24", 10, 2, true, TestName = "onlyPositive is true, but input is negative")]
        [TestCase("0C90ABFF", 10, TestName = "hexadecimal numbers are not supported")]
        [TestCase("IV", 3, TestName = "roman numerals are not supported")]
        [TestCase("2٫1", 2, 1, TestName = "arabic decimal separator (٫) is not supported")]
        [TestCase("௨", 1, TestName = "tamil numerals are not supported")]
        public void Should_ReturnFalse_When(string input, int precision, int scale = 0, bool onlyPositive = false)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);

            numberValidator.IsValidNumber(input).Should().BeFalse();
        }

        [TestCase("-5", 10, 2, TestName = "input is negative integer")]
        [TestCase("1", 10, 2, TestName = "input is integer but scale is not 0")]
        [TestCase("20", 10, TestName = "input is integer and scale is 0")]
        [TestCase("2.2", 5, 3, TestName = "input frac part is shorter than scale and input is shorter than precision")]
        [TestCase("-2.5", 7, 1, TestName = "input frac part length == scale and input is shorter than precision")]
        [TestCase("-2.5", 3, 1, TestName = "input frac part length == scale and input length == precision (signed, -)")]
        [TestCase("+2.5", 3, 1, TestName = "input frac part length == scale and input length == precision (signed, +)")]
        [TestCase("2.5", 2, 1, TestName = "input frac part length == scale and input length == precision (unsigned)")]
        [TestCase("2.5", 2, 1, true, TestName = "input is positive and onlyPositive is true")]
        [TestCase("02.2", 3, 1, TestName = "there are useless zeroes at start")]
        [TestCase("002", 3, 1, TestName = "there are useless zeroes at start of integer")]
        [TestCase("2.200", 4, 3, TestName = "there are useless zeroes at end")]
        public void Should_ReturnTrue_When(string input, int precision, int scale = 0, bool onlyPositive = false)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);

            numberValidator.IsValidNumber(input).Should().BeTrue();
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
                throw new ArgumentException(
                    "precision must be a non-negative number less or equal than precision"); // Exception message typo?
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