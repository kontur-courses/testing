using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        private NumberValidator numberValidator;

        [SetUp]
        public void SetUp()
        {
            numberValidator = new NumberValidator(6, 2);
        }

        [Test]
        public void NumberValidator_ThrowsException_IfPrecisionIsZero()
        {
            Action act = () => new NumberValidator(0, 2);

            act.Should().Throw<ArgumentException>();
        }
        
        [Test]
        public void NumberValidator_ThrowsException_IfPrecisionIsNegative()
        {
            Action act = () => new NumberValidator(-1, 2);

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void NumberValidator_ThrowsException_IfScaleIsNegative()
        {
            Action act = () => new NumberValidator(2, -1);

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void NumberValidator_ThrowsException_IfScaleEqualsPrecision()
        {
            Action act = () => new NumberValidator(3, 3);

            act.Should().Throw<ArgumentException>();
        }
        
        [Test]
        public void NumberValidator_ThrowsException_IfScaleMoreThanPrecision()
        {
            Action act = () => new NumberValidator(3, 4);

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void NumberValidator_NotThrowsException_IfArgumentsIsCorrect()
        {
            Action act = () => new NumberValidator(3, 2, true);

            act.Should().NotThrow();
        }

        [Test]
        public void IsValidNumber_IsFalse_IfNumberIsNegativeWhenOnlyPositive()
        {
            var onlyPositiveNumberValidator = GetOnlyPositiveNumberValidator();

            onlyPositiveNumberValidator.IsValidNumber("-0.1").Should().BeFalse();
        }

        [Test]
        [TestCase(".05")]
        [TestCase("-+5")]
        [TestCase("5:2")]
        [TestCase("1.1e1")]
        [TestCase("A,5")]
        [TestCase("  ")]
        [TestCase("55.")]
        [TestCase("55,")]
        public void IsValidNumber_IsFalse_IfFormatIsIncorrect(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_IsFalse_IfStringIsNull()
        {
            numberValidator.IsValidNumber(null).Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_IsFalse_IfStringIsEmpty()
        {
            numberValidator.IsValidNumber(string.Empty).Should().BeFalse();
        }

        [Test]
        [TestCase("1234567")]
        [TestCase("12345.67")]
        [TestCase("-123456")]
        [TestCase("-1234.56")]
        [TestCase("+123456")]
        [TestCase("+1234.56")]
        public void IsValidNumber_IsFalse_IfNumberLengthMoreThanPrecision(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_IsFalse_IfFractalPartLengthMoreThanScale()
        {
            numberValidator.IsValidNumber("13.123").Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_IsTrue_IfNumberIsNegativeWhenNotOnlyPositive()
        {
            numberValidator.IsValidNumber("-123.5").Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_IsTrue_IfNumberIsPositiveWhenNotOnlyPositive()
        {
            numberValidator.IsValidNumber("123.5").Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_IsTrue_IfNumberIsPositiveWhenOnlyPositive()
        {
            var onlyPositiveNumberValidator = GetOnlyPositiveNumberValidator();

            onlyPositiveNumberValidator.IsValidNumber("123.5").Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_IsTrue_IfNumberHasPlus()
        {
            numberValidator.IsValidNumber("+123.5").Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_IsTrue_IfFractalPartLengthEqualsScale()
        {
            numberValidator.IsValidNumber("123.12").Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_IsTrue_IfFractalPartIsEmpty()
        {
            numberValidator.IsValidNumber("123").Should().BeTrue();
        }

        [Test]
        [TestCase("0000")]
        [TestCase("0000.0")]
        [TestCase("+0000")]
        [TestCase("-0000")]
        public void IsValidNumber_IsTrue_IfNumberContainsOnlyZeros(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeTrue();
        }

        [Test]
        [TestCase("123456")]
        [TestCase("12345.6")]
        [TestCase("-12345")]
        [TestCase("-1234.5")]
        [TestCase("+12345")]
        [TestCase("+1234.5")]
        public void IsValidNumber_IsTrue_IfNumberLengthEqualsPrecision(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_IsTrue_IfScaleIsZero()
        {
            var numberValidatorWithZeroScale = new NumberValidator(5);

            numberValidatorWithZeroScale.IsValidNumber("123");
        }

        private NumberValidator GetOnlyPositiveNumberValidator()
        {
            return new NumberValidator(6, 2, true);
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