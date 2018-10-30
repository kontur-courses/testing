﻿using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
		[TestCase(-100,1, "precision must be a positive number", TestName = "PrecisionIsNegative")]
		[TestCase(0, 1, "precision must be a positive number", TestName = "PrecisionIs0")]
		[TestCase(1, -2, "scale must be a non-negative number less or equal than precision", TestName = "ScaleIsNegative")]
		[TestCase(3, 5, "scale must be a non-negative number less or equal than precision", TestName = "ScaleLessPrecision")]
        public void NumberValidator_ShouldThrowArgumentException(int precision, int scale, string errorMessage)
	    {
		    Action create = () => new NumberValidator(precision, scale);
		    create.ShouldThrow<ArgumentException>()
			    .WithMessage(errorMessage);
        }
	   	   
		[TestCase(20, 1, true, "1.1", TestName = "NumberLengthLessPrecision")]
		[TestCase(20, 19, true, "1.1", TestName = "FracPartLengthLessScale")]
		[TestCase(2, 1, true, "1", TestName = "WhenFracPartIsEmpty")]
		[TestCase(3, 1, false, "+1.1", TestName = "NumberWithPlus")]
		

        public void IsValidNumber_ShouldReturnTrue(int precision, int scale, bool onlyPositive, string testString)
	    {
		    new NumberValidator(precision, scale, onlyPositive).IsValidNumber(testString).Should().BeTrue(
			    "because\nNumberValidator:\n" +
				$"	precision {precision},\n" +
			    $"	scale {scale},\n" +
			    $"	onlyPositive {onlyPositive}.\n" +
			    $"Test string is \"{testString}\"\n");
	    }

	    [TestCase(3,2,true, null, TestName = "NullString")]
	    [TestCase(3, 2, true, "", TestName = "EmptyString")]
        [TestCase(3, 2, true, "abracadabra", TestName = "StringWithoutMatches")]
	    [TestCase(3, 2, true, "11.23", TestName = "PrecisionLessNumberLength")]
	    [TestCase(3, 2, true, "1.234", TestName = "ScaleLessFracPartLength")]
	    [TestCase(3, 2, true, "-1.2", TestName = "StringWithMinusAndOnlyPositiveTrue")]
	    [TestCase(3, 2, true, "-11.2", TestName = "NumberLengthWithSighMorePrecision")]
	    [TestCase(4, 1, false, "+-1.1", TestName = "NumberWithBothSighs")]
	    [TestCase(4, 1, false, "1.,1", TestName = "NumberWithBothSeparatorsSighs")]
	    [TestCase(4, 1, false, "1..1", TestName = "NumberWithDuplicateSigh")]
	    [TestCase(4, 1, false, " ", TestName = "StringWithWhiteSpace")]
        public void IsValidNumber_ShouldReturnFalse(int precision, int scale, bool onlyPositive, string testString)
	    {
		    new NumberValidator(precision, scale, onlyPositive).IsValidNumber(testString).Should().BeFalse(
			    "because\nNumberValidator:\n" +
				$"	precision {precision},\n" +
			    $"	scale {scale},\n" +
			    $"	onlyPositive {onlyPositive}.\n" +
			    $"Test string is \"{testString}\"\n");
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