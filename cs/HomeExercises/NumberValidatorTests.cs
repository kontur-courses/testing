using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
    public class NumberValidatorTests
    {
        //[Test]
        //public void Test()
        //{
        //    Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
        //    Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
        //    Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
        //    Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
        //    Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
        //    Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
        //}

        [TestCase(-1, 0, TestName = "Negative precision")]
        [TestCase(0, 1, TestName = "Zero precision")]
        [TestCase(1, -1, TestName = "Negative scale")]
        [TestCase(1, 1, TestName = "Scale equals precision")]
        [TestCase(1, 2, TestName = "Scale more than precision")]
        public void Should_ThrowException(int precision, int scale)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
        }

        [TestCase("", TestName = "Empty value")]
        [TestCase(null, TestName = "Null value")]
        [TestCase("a.sd", TestName = "Value does not represent a number")]
        [TestCase("5.", TestName = "Fractional part is absent while delimiter is present")]
        [TestCase(".9", TestName = "Integer part is absent")]
        [TestCase("123.4", TestName = "Integer+fractional is more than precision")]
        [TestCase("1.234", TestName = "Fractional part is more than scale")]
        [TestCase("00.00", TestName = "Zeroes are taken into account")]
        [TestCase("+00.0", TestName = "Sign is taken into account in precision")]
        [TestCase("-1", 3, 2, true, TestName = "Only positive permitted, negative value")]
        public void IsNotValidNumber(string value, int precision = 3, int scale = 2, bool onlyPositive = false)
        {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse(
		        "value is {0}, precision is {1}, scale is {2}, onlyPositive is {3}", value, precision, scale, onlyPositive);
        }

	    [TestCase("12.3", TestName = "Integer+fractional equals precision")]
	    [TestCase("1.23", TestName = "Fractional equals scale")]
        [TestCase("+0.1", TestName = "Value starts \"+\" sign")]
	    [TestCase("1.2", 3, 2, true, TestName = "Only positive permitted, positive value")]
        [TestCase("123", TestName = "Integer part only")]
	    [TestCase("1,00", TestName = "Comma-separated")]
        public void IsValidNumber(string value, int precision = 3, int scale = 2, bool onlyPositive = false)
	    {
		    new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue(
			    "value is {0}, precision is {1}, scale is {2}, onlyPositive is {3}", value, precision, scale, onlyPositive);
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
                throw new ArgumentException("scale must be a non-negative number less than precision");
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