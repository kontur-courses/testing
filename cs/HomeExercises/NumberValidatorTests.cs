using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
	    [TestCase(-1, 2, TestName = "When Precision Is Negative")]
	    [TestCase(1, 2, TestName = "When Scale Bigger Then Precision")]
	    [TestCase(1, -2, TestName = "When Scale Is Negative")]
        public void Should_ThrowException(int precision, int scale)
	    {
		    Action validator = () => new NumberValidator(precision, scale);
			validator.ShouldThrow<ArgumentException>();
        }

	    [TestCase(1, 0, TestName = "When Valid Data")]
        public void ShouldNot_ThrowException(int precision, int scale)
	    {
		    Action validator = () => new NumberValidator(precision, scale);
		    validator.ShouldNotThrow<ArgumentException>();
        }

        [TestCase(17, 2, true, "0.0", TestName = "When Int And Fraction Part Less Than Precision And Franc Less Than Scale")]
        [TestCase(4, 2, true, "+1.23", TestName = "When Int And Fraction Part Less Than Precision And Franc Less Than Scale With Sign")]
        [TestCase(17, 2, true, "0,0", TestName = "With Comma")]
		[TestCase(17, 2, false, "-12.2", TestName = "When Negative Number")]
        [TestCase(17, 2, false, "000012.20", TestName = "When First Are Zeros")]
        [TestCase(17, 0, false, "1232", TestName = "When Only Int Part")]
        public void Should_BeValid(int precision, int scale, bool onlyPositive, string value)
        {
           new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
        }

		[TestCase(17, 2, false, "-121.", TestName = "When Nothing After Dot")]
	    [TestCase(3, 2, true, "00.00", TestName = "When Int And Fraction Part Bigger Than Precision")]
	    [TestCase(3, 2, true, "-0.00", TestName = "When Int And Fraction Part With Sign Bigger Than Precision")]
	    [TestCase(17, 2, true, "0.000", TestName = "When Fraction Bigger Than Scale")]
	    [TestCase(3, 2, true, "a.sd", TestName = "When Not A Number")]
	    [TestCase(3, 2, true, null, TestName = "When Null")]
        [TestCase(3, 2, true, "abrakadabra", TestName = "When Abrakadabra")]
	    [TestCase(17, 2, true, "-5.7", TestName = "When Negative If Only Positive")]
        [TestCase(3, 2, true, "", TestName = "When Empty")]
        public void ShouldNot_BeValid(int precision, int scale, bool onlyPositive, string value)
	    {
		    new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
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