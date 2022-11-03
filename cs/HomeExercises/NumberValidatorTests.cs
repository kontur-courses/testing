using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [TestCase(-1, 2, true, TestName = "Negative precision with onlyPositive = true")]
        [TestCase(-1, 2, false, TestName = "Negative precision with onlyPositive = false")]
        [TestCase(5, -2, TestName = "Scale is negative")]
        [TestCase(5, 5, TestName = "Scale greater or equal precision")]
        public void NumberValidator_ShouldThrowArgumentException(int precision, int scale, bool onlyPositive = false)
        {
            new Action(() => new NumberValidator(precision, scale, onlyPositive)).Should().Throw<ArgumentException>();
        }

        [TestCase(17, 2, "0.0", TestName = "With format value \"x.x\"")]
        [TestCase(17, 2, "0", TestName = "With format value \"x\"")]
        [TestCase(4, 2, "+1.23", TestName = "With format value \"x.xx\"")]
        public void IsValidNumber_ShouldBeTrue(int precision, int scale, string value)
        {
            new NumberValidator(precision, scale, true).IsValidNumber(value).Should().BeTrue();
        }

        [TestCase(3, 2, null, TestName = "Value is null")]
        [TestCase(3, 2, "", TestName = "Value is empty")]
        [TestCase(3, 2, "00.00", TestName = "With format value \"0x.xx\"")]
        [TestCase(3, 2, "-0.00", TestName = "With format value \"-x.xx\" and precision = 3")]
        [TestCase(3, 2, "+1.23", TestName = "With format value \"+x.xx\" and precision = 3")]
        [TestCase(3, 2, "a.sd", TestName = "With uncountable value")]
        [TestCase(17, 2, "0.000", TestName = "With format value \"x.xxx\" and scale = 2")]
        public void IsValidNumber_ShouldBeFalse(int precision, int scale, string value)
        {
            new NumberValidator(precision, scale, true).IsValidNumber(value).Should().BeFalse();
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