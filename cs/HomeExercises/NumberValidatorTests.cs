﻿using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
        [Test]
        [TestCase(17, 2, true, "0.0", true)]
        [TestCase(17, 2, true, "0", true)]
        [TestCase(3, 2, true, "00.00", false)]
        [TestCase(3, 2, true, "-0.0", false)]
        [TestCase(3, 2, true, "+0.00", false)]
        [TestCase(4, 2, true, "+1.23", true)]
        [TestCase(3, 2, true, "+1.23", false)]
        [TestCase(17, 2, true, "0.000", false)]
        [TestCase(3, 2, true, "-1.23", false)]
        [TestCase(3, 2, true, "a.sd", false)]
        [TestCase(3, 2, true, "", false)]
        [TestCase(3, 2, true, null, false)]
        public void ValidateNumberCorrect(int precision, int scale, bool onlyPositive, string value, bool expectedResult)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            var result = validator.IsValidNumber(value);
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase(-1, 2, true)]
        [TestCase(1, -2, false)]
        [TestCase(1, 2, true)]
        [TestCase(1, 1, false)]
        public void ThrowArgumentException(int precision, int scale, bool onlyPositive)
        {
            Action action = () => new NumberValidator(precision, scale, onlyPositive);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        [TestCase(1, 0, true)]
        [TestCase(2, 1, true)]
        public void DoesNotThrowArgumentException(int precision, int scale, bool onlyPositive)
        {
            Action action = () => new NumberValidator(precision, scale, onlyPositive);
            action.Should().NotThrow();

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