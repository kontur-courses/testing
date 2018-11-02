using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorConstructorTests
    {
        [Test]
        public void Constructor_ShouldFallOn_NegativePrecision()
        {
            Action act = () => new NumberValidator(-1);

            act.ShouldThrow<ArgumentException>()
                .WithMessage("precision must be a positive number");
        }

        [Test]
        public void Constructor_ShouldFallOn_ZeroPrecision()
        {
            Action act = () => new NumberValidator(0);

            act.ShouldThrow<ArgumentException>()
                .WithMessage("precision must be a positive number");
        }

        [Test]
        public void Constructor_ShouldNotFallOn_PositivePrecision()
        {
            Action act = () => new NumberValidator(1);

            act.ShouldNotThrow();
        }

        [Test]
        public void Constructor_ShouldFallOn_NegativeScaleValue()
        {
            Action act = () => new NumberValidator(1, -1);

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Constructor_ShouldFallOn_PrecisionLessThanScale()
        {
            Action act = () => new NumberValidator(1, 2);

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Constructor_ShouldNotFallOn_PrecisionEqualToScale()
        {
            Action act = () => new NumberValidator(2, 2);

            act.ShouldNotThrow();
        }
    }

    [TestFixture]
    public class NumberValidator_IsValidNumberTests
    {
        private NumberValidator numberValidator;

        [SetUp]
        public void InitNumberValidator()
        {
            numberValidator = new NumberValidator(5, 2);
        }

        [Test]
        public void IsValidNumber_ShouldBeFalseOn_NullOrEmptyValue()
        {
            numberValidator.IsValidNumber("")
                .Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_ShouldBeFalseOn_NotANumberValue()
        {
            numberValidator.IsValidNumber("a.sd")
                .Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_ShouldBeTrueOn_PositiveDoubleValue()
        {
            numberValidator.IsValidNumber("0.0")
                .Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_ShouldBeTrueOn_NegativeDoubleValue()
        {
            numberValidator.IsValidNumber("-1.23")
                .Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_ShouldBeTrueOn_PositiveValueWithZeroScale()
        {
            new NumberValidator(2).IsValidNumber("12")
                .Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_ShouldBeFalseOn_OnlyPositiveValidator_WithNegativeValue()
        {
            new NumberValidator(10, 1, true).IsValidNumber("-12")
                .Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_ShouldBeFalseOn_ExcessPrecision()
        {
            new NumberValidator(5).IsValidNumber("123456")
                .Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_ShouldBeFalseOn_ExcessScale()
        {
            numberValidator.IsValidNumber(".123")
                .Should().BeFalse();
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
            if (scale < 0 || scale > precision)
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