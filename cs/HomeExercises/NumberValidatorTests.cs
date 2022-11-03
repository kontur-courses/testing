using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        #region BeforeRefactor
        /*[Test]
		public void Test()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
		}*/
        #endregion

        [Test]
        [TestCase(-1, 2, true, TestName = "Negative precision with onlyPositive = true")]
        [TestCase(-1, 2, false, TestName = "Negative precision with onlyPositive = false")]
        [TestCase(5, -2, TestName = "Scale is negative")]
        [TestCase(5, 5, TestName = "Scale greater or equal precision")]
        public void NumberValidator_ShouldThrowArgumentException(int precision, int scale, bool onlyPositive = false)
        {
	        new Action (() => new NumberValidator(precision, scale, onlyPositive)).Should().Throw<ArgumentException>();
        }

        [TestCase(1, 0, true, TestName = "Precision > 0, scale >= 0, precision > scale")]
        public void NumberValidator_ShouldNotThrow(int precision, int scale, bool onlyPositive)
        {
            new Action(() => new NumberValidator(precision, scale, onlyPositive)).Should().NotThrow();
        }

        [TestCase(17, 2, "0.0", TestName = "With value \"0.0\"")]
        [TestCase(17, 2, "0", TestName = "With value \"0\"")]
        [TestCase(4, 2, "+1.23", TestName = "With value \"1.23\"")]
        public void IsValidNumber_ShouldBeTrue(int precision, int scale, string value)
        {
            new NumberValidator(precision, scale, true).IsValidNumber(value).Should().BeTrue();
        }


        [TestCase(3, 2, null, TestName = "Value is null")]
        [TestCase(3, 2, "", TestName = "Value is empty")]
        [TestCase(3, 2, "00.00", TestName = "With value \"00.00\" and precision = 3")]
        [TestCase(3, 2, "-0.00", TestName = "With value \"-0.00\" and precision = 3")]
        [TestCase(3, 2, "+1.23", TestName = "With value \"+1.23\" and precision = 3")]
        [TestCase(3, 2, "a.sd", TestName = "With value \"a.sd\"")]
        [TestCase(17, 2, "0.000", TestName = "With value \"0.000\" and scale = 2")]
        [TestCase(17, 2, "秦.書", TestName = "With value \"秦.書\"")]
        [TestCase(17, 2, "3.\t", TestName = "With value \"3.\t\"")]
        public void IsValidNumber_ShouldBeFalse(int precision, int scale, string value)
        {
            new NumberValidator(precision, scale, true).IsValidNumber(value).Should().BeFalse();
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