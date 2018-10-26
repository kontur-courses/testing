using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void Test()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
            Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

            Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
            Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
            Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
            Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
            Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
            Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
        }
    }

    [Category("NumberValidator creation")]
    [TestFixture]
    public class NumberValidatorCreation_Should
    {
        [TestCase(5, -3, "scale must be a non-negative number less than precision", TestName = "scale is negative")]
        [TestCase(0, -1, "precision must be a positive number", TestName = "precision is non positive")]
        [TestCase(5, 7, "scale must be a non-negative number less than precision", TestName = "scale is bigger than precision")]
        public void ThrowArgumentException_When(int precision, int scale, string message)
        {
            Action creation = () => new NumberValidator(precision, scale);
            creation.Should()
                    .Throw<ArgumentException>()
                    .WithMessage(message);
        }
    }

    [Category("NumberValidator::IsValidNumber")]
    [TestFixture]
    public class NumberValidatorIsValidNumber_Should
    {
        [TestCase(null, TestName = "is null")]
        [TestCase("", TestName = "is empty")]
        [TestCase("   \t  ", TestName = "contains only whitespace characters")]
        [TestCase("a7,3e", TestName = "contains any letter")]
        [TestCase("1..1", TestName = "contains several \".\"")]
        [TestCase("1,,1", TestName = "contains several \",\"")]
        [TestCase("++1", TestName = "contains several \"+\"")]
        [TestCase("--1", TestName = "contains several \",\"")]
        [TestCase(",1", TestName = "starts with \",\"")]
        [TestCase(".1", TestName = "starts with \".\"")]
        public void BeFalse_WhenValue(string value)
        {
            TestOneCase(value: value);
        }
        [TestCase("1", TestName = "has only integral part")]
        [TestCase("0.13", TestName = "has only fractal part")]
        [TestCase("1,4", TestName = "has \",\" as delimiter")]
        [TestCase("1.4", TestName = "has \".\" as delimiter")]
        [TestCase("-1", TestName = "has \"-\" sign")]
        [TestCase("+1", TestName = "has \"+\" sign")]
        [TestCase("00.00", TestName = "has several zeroes")]
        public void BeTrue_WhenValue(string value)
        {
            TestOneCase(value: value, expected: true);
        }


        private static void TestOneCase(int precision = 5,
                                 int scale = 2,
                                 bool onlyPositive = false,
                                 string value = "1,2", bool expected = false)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value)
                                                               .Should()
                                                               .Be(expected);
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
                throw new ArgumentException("scale must be a non-negative number less than precision");
            numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
        }

        public bool IsValidNumber(string value)
        {
            // Проверяем соответствие входного значения формату N(m,k),
            // в соответствии с правилом, 
            // описанным в Формате описи документов, направляемых в налоговый орган
            // в электронном виде по телекоммуникационным каналам связи:
            // Формат числового значения указывается в виде N(m.к),
            // где m – максимальное количество знаков в числе, включая знак
            // (для отрицательного числа), 
            // целую и дробную часть числа без разделяющей десятичной точки,
            // k – максимальное число знаков дробной части числа. 
            // Если число знаков дробной части числа равно 0 (т.е. число целое),
            // то формат числового значения имеет вид N(m).

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