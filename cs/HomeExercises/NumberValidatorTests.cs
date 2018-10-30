using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace HomeExercises
{
	[TestFixture]
    public class NumberValidatorTests
    {
	    [TestFixture]
        private class Constructor
        {
		    [TestCase(-1, 2, TestName = "Precision is negative")]
		    [TestCase(3, -2, TestName = "Scale is negative")]
		    [TestCase(3, 4, TestName = "Scale more then precision")]
		    [TestCase(3, 3, TestName = "Scale equal precision")]
		    public void Should_ThrowArgumentException_When(int precision, int scale)
		    {
			    Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, true));
		    }

		    [TestCase(3, 2, TestName = "Correct precision and scale")]
		    public void Should_DoesNotThrowArgumentException_When(int precision, int scale)
		    {
			    Assert.DoesNotThrow(() => new NumberValidator(precision, scale, true));
		    }
	    }

	    [TestFixture]
        private class IsValidNumber
	    {

		    [TestCase(4, 3, true, "3.141", TestName = "Number is only positive")]
		    [TestCase(11, 3, false, "-2.44", TestName = "Number is negative")]
		    [TestCase(11, 3, false, "+2.44", TestName = "Number is positive")]
		    [TestCase(4, 0, true, "3", TestName = "Number is integer")]
		    [TestCase(4, 2, false, "+2.44", TestName = "String includes the sign in front of the number")]
		    [TestCase(3, 2, false, "2,44", TestName = "String includes a comma")]

		    public void Should_BeTrue_When(int precision, int scale, bool onlyPositive, string input)
		    {
			    new NumberValidator(precision, scale, onlyPositive).IsValidNumber(input).Should().Be(true);
		    }

		    [TestCase(10, 2, true, null, TestName = "String is null")]
		    [TestCase(10, 2, true, "", TestName = "String is empty")]
		    [TestCase(10, 2, true, "0zrt0", TestName = "Invalid format string")]
		    [TestCase(4, 3, true, "123.33", TestName = "Symbols in number more than precision")]
		    [TestCase(11, 3, true, "123.33240", TestName = "Count of symbols in the fraction part more than the scale")]
		    [TestCase(11, 3, true, "-2.44", TestName = "Number is not only positive")]
		    [TestCase(4, 3, true, "3.2.2", TestName = "More one fraction part")]
		    [TestCase(3, 2, false, ".44", TestName = "No integer part")]
		    public void Should_BeFalse_When(int precision, int scale, bool onlyPositive, string input)
		    {
			    new NumberValidator(precision, scale, onlyPositive).IsValidNumber(input).Should().Be(false);
		    }
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