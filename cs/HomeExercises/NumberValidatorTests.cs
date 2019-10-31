using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void Ctor_ShouldThrow_WhenPrecisionNotPositive()
        {
            Action action = () => new NumberValidator(-1, 2);

            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Ctor_ShouldThrow_WhenScaleIsNegative()
        {
            Action action = () => new NumberValidator(1, -2);

            action.Should().Throw<ArgumentException>();
        }

        [TestCase(1, 2, TestName = "Scale more than precision")]
        [TestCase(1, 1, TestName = "Scale is equal to precision")]
        public void Ctor_ShouldThrow_WhenScaleMoreOrEqualToPrecision(int precision, int scale)
        {
            Action action = () => new NumberValidator(precision, scale);

            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Ctor_ShouldNotThrowException_WhenPrecisionAndScaleCorrect()
        {
            Action action = () => new NumberValidator(4, 2);

            action.Should().NotThrow();
        }

        [TestCase(null, TestName = "Null")]
        [TestCase("", TestName = "Empty string")]
        [TestCase("   ", TestName = "Whitespaces")]
        [TestCase("\t\t\t", TestName = "Tabs")]
        public void IsValidNumber_ShouldBeFalse_WhenNullOrEmpty(string input)
        {
            new NumberValidator(17, 2)
                .IsValidNumber(input)
                .Should()
                .BeFalse();
        }

        [Test]
        public void IsValidNumber_ShouldBeFalse_WhenNotANumber()
        {
            new NumberValidator(17, 2)
                .IsValidNumber("a.sd")
                .Should()
                .BeFalse();
        }

        [TestCase(3, 2, "00.00", TestName = "Fraction")]
        [TestCase(3, 2, "+1.23", TestName = "Number with sign")]
        [TestCase(3, 2, "1234", TestName = "Integer")]
        public void IsValidNumber_ShouldBeFalse_WhenNumberMoreThanPrecision(int precision, int scale, string input)
        {
            new NumberValidator(precision, scale)
                .IsValidNumber(input)
                .Should()
                .BeFalse();
        }

        [TestCase(17, 2, "0.000", TestName = "Long fraction part")]
        [TestCase(17, 0, "0.0", TestName = "Fraction part when scale is 0")]
        public void IsValidNumber_ShouldBeFalse_WhenFractionalPartMoreThanScale(int precision, int scale, string input)
        {
            new NumberValidator(precision, scale)
                .IsValidNumber(input);
        }

        [Test]
        public void IsValidNumber_ShouldBeFalse_WhenNegativeNumberWithOnlyPositiveFlag()
        {
            new NumberValidator(17, 2, true)
                .IsValidNumber("-1.23")
                .Should()
                .BeFalse();
        }

        [TestCase(17, 2, "0.0", TestName = "Fraction")]
        [TestCase(17, 2, "0", TestName = "Integer")]
        public void IsValidNumber_ShouldBeTrue_OnFractionThatFitsPrecisionAndScale(int precision, int scale, string input)
        {
            new NumberValidator(precision, scale)
                .IsValidNumber(input)
                .Should()
                .BeTrue();
        }

        [TestCase(17, 2, "+1.23", TestName = "Positive number")]
        [TestCase(17, 2, "-1.23", TestName = "Negative number")]
        [TestCase(17, 2, "-0", TestName = "Zero with negative sign")]
        [TestCase(17, 2, "+0", TestName = "Zero with positive sign")]
        public void IsValidNumber_ShouldBeTrue_OnNumberWithSign(int precision, int scale, string input)
        {
            new NumberValidator(precision, scale)
                .IsValidNumber(input)
                .Should()
                .BeTrue();
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