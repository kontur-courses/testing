using System;
using System.Net;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidator_Should
    {
        [TestCase(0, 1, true, TestName = "Precision_LessOrEqualZero")]
        [TestCase(2, -1, true, TestName = "Scale_LessThanZero")]
        [TestCase(0, 1, true, TestName = "Scale_GreatThanPrecision")]
        public void ThrowArgumentException(int precision, int scale, bool isOnlyPositive)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, isOnlyPositive));
        }
    }

    [TestFixture]
    public class IsValidNumber_Should
    {
        private readonly NumberValidator numberValidator = new NumberValidator(precision: 5, scale: 2, onlyPositive: false);
        private readonly NumberValidator positiveNumberValidator = new NumberValidator(precision: 4, scale: 2, onlyPositive: true);

        [TestCase("", TestName = "Empty string")]
        [TestCase(null, TestName = "Null string")]
        [TestCase("^2.15", TestName = "Invalid sign")]
        [TestCase("s.15", TestName = "Invalid integer part")]
        [TestCase("0.s", TestName = "Invalid fraction part")]
        [TestCase(".15", TestName = "Decimal number without integer part")]
        [TestCase("0.", TestName = "Integer number with separator")]
        [TestCase("1.1234", TestName = "Exceedingly long fraction part")]
        [TestCase("123.123", TestName = "Exceedingly long number")]
        [TestCase("123456", TestName = "Exceedingly long integer number")]
        public void NumberValidator_ReturnFalse(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeFalse();
        }

        [TestCase("5", TestName = "Integer number")]
        [TestCase("+123.12", TestName = "Decimal number length is equal precision but is plus")]
        [TestCase("0.15", TestName = "Point separator")]
        [TestCase("0,15", TestName = "Comma separator")]
        [TestCase("+0.15", TestName = "Plus sign")]
        [TestCase("-0.15", TestName = "Minus sign")]
        [TestCase("00.00", TestName = "Integer part from zeros")]
        [TestCase("00", TestName = "Integer number from zeros")]
        public void NumberValidator_ReturnTrue(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeTrue();
        }

		[TestCase("-1.12", TestName = "Negative decimal number")]
		[TestCase("-1", TestName = "Negative integer number")]
        public void PositiveNumberValidator_ReturnFalse(string number)
        {
            positiveNumberValidator.IsValidNumber(number).Should().BeFalse();
        }

	    [TestCase("1.12", TestName = "Positive Decimal number")]
	    [TestCase("1", TestName = "Positive integer number")]
		[TestCase("+1.12", TestName = "Decimal number with plus sign")]
	    [TestCase("+1", TestName = "Integer number with plus sign")]
        public void PositiveNumberValidator_ReturnTrue(string number)
	    {
		    positiveNumberValidator.IsValidNumber(number).Should().BeTrue();
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
            // Здесь не учитывалось, что "+" - не нужно считать.
            var intPart = match.Groups[1].Value == "+" ?
                match.Groups[2].Value.Length :
                match.Groups[1].Value.Length + match.Groups[2].Value.Length;

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
