using System;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{

        [SetUp]
        public void InitEvery()
        { defaultNumberValidator = new NumberValidator(int.MaxValue, int.MaxValue - 1, false);
            
        }

        [OneTimeSetUp]
        public void Init()
        {
            longWrongString = RepeatChar('a', 20000);
            longCorrectString = new String('1', 20000);

        }

        private NumberValidator defaultNumberValidator;
        private static string longWrongString;
        private string longCorrectString;
        private Action ConstructNumberValidator(int precision, int? scale = null)
        {
                return  () => new NumberValidator(precision, scale ?? 0);
        }

        private void DoInCycle(int count, Delegate function)
        {
            Object result;
            for (var i = 0; i < count; i++)
                result = function.DynamicInvoke();

        }
        private string RepeatChar(char chr, int count)
        {
            return new String(chr, count);

        }

        [TestCase(2, 1, TestName = "Positive scale is less then positive precision")]
        [TestCase(1, 0, TestName = "Zero scale")]
        [TestCase(1, TestName = "Positive precision, no scale")]
        [TestCase(int.MaxValue, int.MaxValue - 1, TestName = "Scale is max value of int, precision is positive and less then scale ")]
        public void Constructor_OnCorrectArguments_DoesntThrowAnyExceptions(int precision, int? scale = null)
        {
            Action act = ConstructNumberValidator(precision, scale);
            act.ShouldNotThrow();
        }

        [TestCase(-1, TestName = "Negative precision")]
        [TestCase(0, TestName = "Zero precision")]
        [TestCase(1, -2, TestName = "Negative scale")]
        [TestCase(1, 2, TestName = "Scale is greater then precision")]
        [TestCase(1, 1, TestName = "Scale equals precision")]
        public void Constructor_OnWrongArguments_ThrowsArgumentExceptions(int precision, int? scale = null)
        {
            Action act = ConstructNumberValidator(precision, scale);
            act.ShouldThrow<ArgumentException>();
        }
        [TestCase(3, 0, true, "111", TestName = "Integer number, zero scale")]
        [TestCase(3, 1, true, "111", TestName = "Integer number, nonzero scale doesn't oblige you to use it")]
        [TestCase(3, 0, true, "+11", TestName = "Integer number with plus")]
        [TestCase(3, 0, false, "-11", TestName = "Integer number with minus")]
        [TestCase(7, 3, true, "1111.222", TestName = "Precision is equal to amount of numerals, scale to after-dot part")]
        [TestCase(10, 5, true, "+1111.222", TestName = "Precision and scale are greater then numerals and plus")]
        [TestCase(10, 5, false, "-1111.222", TestName = "Precision and scale are greater then numerals and minus")]       
        [TestCase(3, 2, false, "0.11", TestName = "Integer part is zero")]
        [TestCase(3, 2, false, "0,11", TestName = "Number contains comma")]
        public void IsValid_OnCorrectNumber_ReturnsTrue(int precision, int scale, bool onlyPositive, string value)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            validator.IsValidNumber(value).Should().BeTrue();
        }

        
        
        [TestCase(5, 2, true, "0.000", TestName = "Count of frac numerals is greater then scale")]
        [TestCase(2, 0, true, "000", TestName = "Count of int numerals is greater then precision")]
        [TestCase(3, 2, true, "12.23", TestName = "Count of int and frac numerals is greater then precision")]
        [TestCase(4, 2, false, "-12.32", TestName = "Count of numerals and minus is greater then precision")]
        [TestCase(4, 3, false, "+12.32", TestName = "Count of numerals and plus is greater then precision")]
        [TestCase(3, 1, true, "-1.2", TestName = "Number should be positive but it is negative")]
        public void IsValid_OnWrongNumber_ReturnsFalse(int precision, int scale, bool onlyPositive, string value)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            validator.IsValidNumber(value).Should().BeFalse();
        }

        
        [TestCase(null, TestName = "String is null")]
        [TestCase("", TestName = "Empty string")]
        [TestCase("abc.5", TestName = "String is non numerical before dot")]
        [TestCase("abc", TestName = "String is non numerical")]
        [TestCase("12.abc", TestName = "String is non numerical after dot")]
        [TestCase("12.5abc", TestName = "String contains non numerical after dot")]
        [TestCase("a12.5abc", TestName = "String contains non numerical before dot")]
        [TestCase("12.12.12", TestName = "String contains 2 dots")]
        [TestCase(".12", TestName = "Empty string before dot")]
        [TestCase("12.", TestName = "Empty string after dot")]
        public void IsValide_OnWrongString_ReturnsFalse(string value)
        {
            defaultNumberValidator.IsValidNumber(value).Should().BeFalse();
        }

        [Test, Timeout(10000)]
        public void IsValid_On10000Iterations_WorksFast()
        {
            Func<bool> funct = () => defaultNumberValidator.IsValidNumber("-1.2");
            DoInCycle(10000, funct);
        }

        [Test, Timeout(10000)]
        public void Constructor_On10000Iterations_WorksFast()
        {
            Func<NumberValidator> funct = () => new NumberValidator(10,2);
            DoInCycle(10000, funct);
        }
        [Test, Timeout(10000)]
        public void IsValid_OnCoorectArgumentsLongerThen10000_WorksFast()
        {
            defaultNumberValidator.IsValidNumber(longCorrectString);
        }
        [Test, Timeout(10000)]
        public void IsValid_OnWrongArgumentsLongerThen10000_WorksFast()
        {
            defaultNumberValidator.IsValidNumber(longWrongString);
        }
        [Test]
        public void IsValid_OnMoreThanOneNumbers_ReturnsTrue()
        {
            var validator = new NumberValidator(10, 5);
            validator.IsValidNumber("0.2").Should().BeTrue();
            validator.IsValidNumber("-0.2").Should().BeTrue();
        }

        [Test]
        public void IsValid_WorksCorrect_WithMoreThanOneValidatorCreated()
        {
            var validator1 = new NumberValidator(2, 1, true);
            var validator2 = new NumberValidator(10, 5);
            validator2.IsValidNumber("-0.22").Should().BeTrue();
        }

        [TestCase(10001, 1, TestName = "Count of int numerals is more then 10000")]
        [TestCase(1, 10001, TestName = "Count of frac numerals is more then 10000")]
        [TestCase(10001, 10001, TestName = "Count of int and frac numerals is more then 10000")]
        public void IsValid_OnNumbersLonger10000_ReturnTrue(int intPartCount, int fracPartCount)
        {
            var value = new String('1', intPartCount) + "." + new String('1',fracPartCount);
            defaultNumberValidator.IsValidNumber(value).Should().BeTrue();
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
