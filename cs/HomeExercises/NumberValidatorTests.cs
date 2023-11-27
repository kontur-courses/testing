using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void Throws_WhenPrecisionNotPositive()
        {
            Action action = () => new NumberValidator(-1, 2, true);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Throws_WhenScaleNegative()
        {
            Action action = () => new NumberValidator(1, -1, true);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Throws_WhenScaleBiggerPrecision()
        {
            Action action = () => new NumberValidator(1, 2, true);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Fails_WhenNumberEmpty()
        {
            var validator = new NumberValidator(17, 2, true);
            var result = validator.IsValidNumber(" ");
            result.Should().BeFalse();
        }

        [Test]
        public void Fails_WhenNumberNotMatch()
        {
            var validator = new NumberValidator(8, 2, true);
            var result = validator.IsValidNumber("1/43");
            result.Should().BeFalse();
        }

        [Test]
        public void Success_WhenIntAndFractPartLessWhenPrecision()
        {
            var validator = new NumberValidator(17, 2, true);
            var result = validator.IsValidNumber("0");
            result.Should().BeTrue();
        }

        [Test]
        public void Fails_WhenPrecisionLessCountNumbers()
        {
            var validator = new NumberValidator(3, 2, true);
            var result = validator.IsValidNumber("00.00");
            result.Should().BeFalse();
        }

        [Test]
        public void Fails_WhenPrecisionLessCountNumbersAndMinus()
        {
            var validator = new NumberValidator(3, 2, true);
            var result = validator.IsValidNumber("-0.00");
            result.Should().BeFalse();
        }

        [Test]
        public void Success_WhenPrecisionEqualsCountNumbersAndSign()
        {
            var validator = new NumberValidator(4, 2, true);
            var result = validator.IsValidNumber("+1.23");
            result.Should().BeTrue();
        }

        [Test]
        public void Fails_WhenScaleLessFractPartLength()
        {
            var validator = new NumberValidator(17, 2, true);
            var result = validator.IsValidNumber("0.000");
            result.Should().BeFalse();
        }

        [Test]
        public void Fails_WhenLetters()
        {
            var validator = new NumberValidator(3, 2, true);
            var result = validator.IsValidNumber("a.sd");
            result.Should().BeFalse();
        }

        [Test]
        public void Fails_WhenOnlyPositiveAndNegativeNumber()
        {
            var validator = new NumberValidator(3, 2, true);
            var result = validator.IsValidNumber("-1.2");
            result.Should().BeFalse();
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
                throw new ArgumentException("precision must be a non-negative number, scale must be less or equal than precision");
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