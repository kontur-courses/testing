using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {


        [TestCase(-1, 2, true, TestName = "Negative Precision")]
        [TestCase(2, -1, true, TestName = "Negative Scale")]
        [TestCase(2, 3, true, TestName = "Precision < Scale")]
        [TestCase(3, 3, true, TestName = "Precision = Scale")]
        [TestCase(2, 3, true, TestName = "Precision = Zero")]
        [TestCase(null, 3, true, TestName = "Precision is null")]
        [TestCase(null, 3, true, TestName = "Scale is null")]

        public void NumberValidator_CreateInvalidNumber_ShouldBeTrowException(int precision, int scale, bool onlyPositive)
        {
            Action createNumberValidator = () => new NumberValidator(precision, scale, onlyPositive); 
           createNumberValidator.Should().Throw<ArgumentException>();
        }



        [TestCase(2, 0, "   ", TestName = "number is someWhiteSpaces")]
        [TestCase(2, 0, "string.string", TestName = "number is word")]
        [TestCase(2, 0, "", TestName = "Number is Empty")]
        [TestCase(2, 0, null, TestName = "Number Is Null")]
        [TestCase(4, 0, "-+20", TestName = "more than one sign")]
        [TestCase(4, 2, "20?00", TestName = "invalid separator. It isn't '.' or ','")]
        [TestCase(4, 2, "20......00", TestName = "more than one separators in a row")]
        [TestCase(2, 0, "20.", TestName = "Number with separator, but Empty fractional part")]
        [TestCase(1, 0, "+", TestName = "number with sign but without fractional and integer parts")]
        [TestCase(4, 1, "2.0.0.0", TestName = "more than one separator")]
        [TestCase(4, 1, "D75", TestName = "number in is not the correct number system")]
        [TestCase(4, 1, "    200    ", TestName = "number surrounded by spaces")]

        public void IsValidNumber_IfInvalidTypeValue_ShouldBeFalse(int precision, int scale, string number, bool onlyPositive = true)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);
            numberValidator.IsValidNumber(number).Should().BeFalse();
        }



        [TestCase(5, 2, true, "-20.00", TestName = "incorrect sign")]
        [TestCase(3, 1, true, "20.00", TestName = "precision < length number")]
        [TestCase(4, 0, true, "20.00", TestName = "scale < length fractional part")]

        public void IsValidNumber_IfIncorrectArgs_ShouldBeFalse(int precision, int scale, bool onlyPositive,  string number)
        {
	        var numberValidator = new NumberValidator(precision, scale, onlyPositive);
	        numberValidator.IsValidNumber(number).Should().BeFalse();
        }


        [TestCase(21, 1, true, "20.0", TestName = "precision > number Length")]
        [TestCase(3, 2, true, "20.0", TestName = "scale > fractional part")]
        [TestCase(3, 1, true, "20,0", TestName = "used ','")]
        [TestCase(4, 1, true, "+20.0", TestName = "use '+' before number")]
        [TestCase(4, 1, false, "+20.0", TestName = "use '+' without onlyPositive flag")]
        [TestCase(2, 0, false, "20", TestName = "number without fractional part")]
        [Test]
        public void IsValidNumber_IfCorrectValue_ShouldBeTrue(int precision, int scale, bool onlyPositive, string number)
        {
	        var numberValidator = new NumberValidator(precision, scale, onlyPositive);
	        numberValidator.IsValidNumber(number).Should().BeTrue();
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