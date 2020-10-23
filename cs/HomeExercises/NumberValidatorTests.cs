﻿using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void NumberValidator_ThrowArgumentException_WhenPrecisionNotPositive()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
        }

        [Test]
        public void NumberValidator_ThrowArgumentException_WhenScaleMoreThenPrecision()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
        }

        [Test]
        public void NumberValidator_ThrowArgumentException_WhenScaleNegative()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(1, -1, true));
        }

        [Test]
        public void NumberValidator_IsValidate_FloatNumber()
        {
            new NumberValidator(17, 2, true).IsValidNumber("0.0").Should().BeTrue();
        }

        [Test]
        public void NumberValidator_IsValidate_FloatNumberWithComma()
        {
            new NumberValidator(17, 2, true).IsValidNumber("0,0").Should().BeTrue();
        }

        [Test]
        public void NumberValidator_IsValidate_PositiveNumberWhithPlus()
        {
            new NumberValidator(17, 2, true).IsValidNumber("+0.0").Should().BeTrue();
        }

        [Test]
        public void NumberValidator_IsValidate_PositiveIntNumber()
        {
            new NumberValidator(17, 2, true).IsValidNumber("0").Should().BeTrue();
        }

        [Test]
        public void NumberValidator_IsValidate_NegativeIntNumber()
        {
            new NumberValidator(17, 2, true).IsValidNumber("-0").Should().BeTrue();
        }

        [Test]
        public void NumberValidator_IsValidate_NegativeFloatNumber()
        {
            new NumberValidator(17, 2, false).IsValidNumber("-0.0").Should().BeTrue();
        }

        [Test]
        public void NumberValidator_IsNotValidate_NotNumber()
        {
            var line = "abc";
            new NumberValidator(17, 2, true).IsValidNumber("abc").Should().BeFalse(line);
        }

        [Test]
        public void NumberValidator_IsNotValidate_Negative_WhenOnlyPositive()
        {
            new NumberValidator(17, 2, true).IsValidNumber("-1.0").Should().BeFalse();
        }

        [Test]
        public void NumberValidator_IsNotValidate_WhenMultipleDots()
        {
            new NumberValidator(17, 10, true).IsValidNumber("1.0.0").Should().BeFalse();
        }

        [Test]
        public void NumberValidator_IsNotValidate_WhenLengthMoreThenPrecision()
        {
            new NumberValidator(3, 2, true).IsValidNumber("+0.00").Should().BeFalse();
        }

        [Test]
        public void NumberValidator_IsNotValidate_WhenFracPartMoreThenScale()
        {
            new NumberValidator(17, 2, true).IsValidNumber("0.000").Should().BeFalse();
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