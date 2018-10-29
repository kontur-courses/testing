using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [TestCase(-1, 2, TestName = "Negative precision in constructor NumberValidator")]
        [TestCase(3, -2, TestName = "Negative scale in constructor NumberValidator")]
        [TestCase(3, 4, TestName = "Scale more then precision in constructor NumberValidator")]
        [TestCase(3, 3, TestName = "Scale equal precision in constructor NumberValidator")]
        public void Constructor_Should_ThrowArgumentException(int precision, int scale)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, true));
        }

        [TestCase(3, 2, TestName = "Correct precision and scale in constructor NumberValidator")]
        public void Constructor_Should_DoesNotThrowArgumentException(int precision, int scale)
        {
            Assert.DoesNotThrow(() => new NumberValidator(precision, scale, true));
        }

        [TestCase(4, 3, true, "3.141", TestName = "Corect only positive namber in IsValidNumber")]
        [TestCase(11, 3, false, "-2.44", TestName = "Negative namber in IsValidNumber")]
        [TestCase(11, 3, false, "+2.44", TestName = "Positive namber in IsValidNumber")]
        [TestCase(4, 0, true, "3", TestName = "Integer number in IsValidNumber")]
        [TestCase(4, 2, false, "+2.44", TestName = "Consider the sign in front of the number in IsValidNumber")]
        [TestCase(3, 2, false, "2,44", TestName = "Consider a comma in IsValidNumber")]
        public void IsValidNumber_Should_BeTrue(int precision, int scale, bool onlyPositive, string input)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(input).Should().Be(true);
        }

        [TestCase(10, 2, true, null, TestName = "Null string in IsValidNumber")]
        [TestCase(10, 2, true, "", TestName = "Empty string in IsValidNumber")]
        [TestCase(10, 2, true, "0zrt0", TestName = "Invalid string format in IsValidNumber")]
        [TestCase(4, 3, true, "123.33", TestName = "Simbols in number more than precision in IsValidNumber")]
        [TestCase(11, 3, true, "123.33240", TestName = "Simbols in fraction part more than scale in IsValidNumber")]
        [TestCase(11, 3, true, "-2.44", TestName = "Incorrect only positive namber in IsValidNumber")]
        [TestCase(4, 3, true, "3.2.2", TestName = "More one fraction part in IsValidNumber")]
        [TestCase(3, 2, false, ".44", TestName = "No integer part in IsValidNumber")]
        public void IsValidNumber_Should_BeFalse(int precision, int scale, bool onlyPositive, string input)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(input).Should().Be(false);
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