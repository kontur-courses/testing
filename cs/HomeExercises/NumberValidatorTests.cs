﻿using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
	    [Test]
	    public void NumberValidator_ThrowArgumentException_WhenPrecisionIsNegative()
	    {
			Action create = () => new NumberValidator(-100, 1, true);
		    create.ShouldThrow<ArgumentException>().WithMessage("precision must be a positive number");
	    }

	    [Test]
	    public void NumberValidator_ThrowArgumentException_WhenPrecisionIs0()
	    {
		    Action create = () => new NumberValidator(0, 1, true);
		    create.ShouldThrow<ArgumentException>().WithMessage("precision must be a positive number");
	    }

        [Test]
	    public void NumberValidator_ThrowArgumentException_WhenScaleIsNegative()
	    {
		    Action create = () => new NumberValidator(1, -2, true);
		    create.ShouldThrow<ArgumentException>()
			    .WithMessage("scale must be a non-negative number less or equal than precision");
	    }

	    [Test]
	    public void NumberValidator_ThrowArgumentException_WhenScaleIsLessPrecision()
	    {
		    Action create = () => new NumberValidator(3, 5, true);
		    create.ShouldThrow<ArgumentException>()
			    .WithMessage("scale must be a non-negative number less or equal than precision");
	    }
	   	   
		[TestCase(20, 1, true, "1.1", Description = "When number length less what precision")]
		[TestCase(20, 19, true, "1.1", Description = "When fracPart length less what scale")]
		[TestCase(2, 1, true, "1", Description = "When fracPart is empty")]
        public void IsValidNumber_ShouldReturnTrue(int precision, int scale, bool onlyPositive, string testString)
	    {
		    new NumberValidator(precision, scale, onlyPositive).IsValidNumber(testString).Should().BeTrue(
			    "because\n	NumberValidator:\n		precision {0},\n" +
			    "		scale {1},\n" +
			    "		onlyPositive {2}.\n" +
			    "	Test string is \"{3}\"\n",
			    precision, scale, onlyPositive, testString);
	    }

	    [TestCase(3,2,true, null, Description = "When null string")]
	    [TestCase(3, 2, true, "", Description = "When empty string")]
        [TestCase(3, 2, true, "abracadabra", Description = "When string without matches")]
	    [TestCase(3, 2, true, "11.23", Description = "When precision less what intPart anf fracPart length")]
	    [TestCase(3, 2, true, "1.234", Description = "When scale less what fracPart length")]
	    [TestCase(3, 2, true, "-1.2", Description = "When string with \"-\" and onlyPositive is true")]
	    [TestCase(3, 2, true, "-11.2", Description = "When number length with sigh more what precision")]
        public void IsValidNumber_ShouldReturnFalse(int precision, int scale, bool onlyPositive, string testString)
	    {
		    new NumberValidator(precision, scale, onlyPositive).IsValidNumber(testString).Should().BeFalse(
			    "because\n	NumberValidator:\n		precision {0},\n" +
			    "		scale {1},\n" +
			    "		onlyPositive {2}.\n" +
			    "	Test string is \"{3}\"\n",
			    precision, scale, onlyPositive, testString);
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