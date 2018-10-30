using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Filters;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        private readonly NumberValidator numberValidator = new NumberValidator(100, 99);
		
		[Test]
		[TestCase(-1, 3, false, TestName = "Negative precision")]
		[TestCase(0, 0, false, TestName = "Zero precision")]
		[TestCase(1, -1, false, TestName = "Negative scale")]
		[TestCase(1, 2, false, TestName = "Precision less than precision")]
		[TestCase(2, 2, false, TestName = "Precision equals to scale")]
	    [TestCase(9000, 10000, false, TestName = "Big precision less than scale")]
        public void CreationFails_WhenValidatorGetsIncorrectPrecisionAndScale(int precision, int scale, bool onlyPositive)
	    {
		    Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
	    }
		
        [Test]
		[TestCase(1, 0, false, TestName = "Precision is bigger than zero scale")]
		[TestCase(2, 1, false, TestName = "Precision is bigger than scale")]
		[TestCase(2, 1, true, TestName = "Validator can handle only positive numbers")]
		[TestCase(10000, 9000, false, TestName = "Precision is bigger than big scale")]
        public void CreationIsSuccessful_WhenValidatorGetsCorrectArguments(int precision, int scale, bool onlyPositive)
        {
            Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
        }
		
        [Test]
		[TestCase("abc", TestName = "Word instead of a number")]
		[TestCase("aaa.a", TestName = "Dot inside of word")]
		[TestCase(".", TestName = "One dot")]
		[TestCase("", TestName = "Empty string")]
		[TestCase(null, TestName = "Null instead of a string")]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNotANumberWithoutDigits(String notANumber)
        {
			numberValidator
				.IsValidNumber(notANumber)
				.Should()
				.BeFalse();
		}

		[Test]
		[TestCase("string0.1", TestName = "Word before a number")]
		[TestCase("0.0string1", TestName = "Word inside of a number after a dot")]
		[TestCase("0.1string", TestName = "Word after a number")]
		[TestCase("112str1.1", TestName = "Word inside of a number before a dot")]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNotANumberWithDigits(String numberWithDigits)
		{
		    numberValidator
		        .IsValidNumber(numberWithDigits)
		        .Should()
		        .BeFalse();
		}

		[Test]
		[TestCase("-0.1", 100, 99, true, TestName = "Positive numbers validator gets a negative number")]
		[TestCase("11.001", 6, 1, false, TestName = "Scale of a number is bigger than possible")]
		[TestCase("110000.001", 5, 4, false, TestName = "Precision of a number is bigger than possible")]
		[TestCase("1000.0000000", 20, 3, false, TestName = "Big zero partial part with scale more than possible")]
		[TestCase("00000000.1", 6, 4, false, TestName = "Big zero integer part with precision more than possible")]
		[TestCase("10.00001", 10, 4, false, TestName = "Integer part contains less digits than scale, but partial part contains more digits than scale")]
		public void IsValidNumber_ReturnsFalse_WhenValidatorGetsWrongNumber(String number,
	        int precision,
	        int scale,
	        bool onlyPositive)
        {
            var validator = new NumberValidator(precision, scale, true);

            validator
                .IsValidNumber(number)
                .Should()
                .BeFalse();
        }

		[Test]
		[TestCase(".001", TestName = "No integer part before a dot")]
		[TestCase("100.", TestName = "No partial part after a dot")]
		[TestCase("100.0.0", TestName = "Too many dots in a number")]
	    public void IsValidNumber_ReturnsFalse_WhenValidatorGetsIncorrectNumberForms(String number)
	    {
		    numberValidator.IsValidNumber(number)
			    .Should()
			    .BeFalse();
	    }

	    [Test]
		[TestCase("-0.1", 100, 99, false, TestName = "All numbers validator gets a negative number")]
		[TestCase("+0.1", 100, 99, false, TestName = "All numbers validator gets a positive number")]
		[TestCase("+0.1", 100, 99, true, TestName = "Positive numbers validator gets a positive number")]
		[TestCase("0.0", 100, 99, true, TestName = "Positive numbers validator gets a zero number")]
		[TestCase("10.0", 100, 99, true, TestName = "Positive numbers validator gets a number without sign before")]
	    [TestCase("10000.0", 6, 2, false, TestName = "Number with maximum allowed precision")]
		[TestCase("11.00001", 10, 5, false, TestName = "Number with maximum allowed scale")]
		[TestCase("100", 100, 99, false, TestName = "Number without partial part")]
		[TestCase("111111111111111111111111111111111111111.0", 100, 99, false, TestName = "Integer part is bigger than any long number")]
		[TestCase("1.11111111111111111111111111111111111111", 100, 99, false, TestName = "Partial part contains more digits than long type can contain")]
	    public void IsValidNumber_ReturnsTrue_WhenValidatorGetsCorrectNumber(String number,
		    int precision,
		    int scale,
		    bool onlyPositive)
	    {
			var validator = new NumberValidator(precision, scale, onlyPositive);

		    validator.IsValidNumber(number)
			    .Should()
			    .BeTrue();
	    }
		
		[Test]
        public void IsValidNumber_ReturnsTrueOnOldValidator_WhenOldValidatorAcceptsNumberButNewDoesNot()
        {
            var oldValidator = new NumberValidator(100, 99);
            var newValidator = new NumberValidator(3, 1);
            var number = "1111.0";

            oldValidator.IsValidNumber(number)
                .Should()
                .BeTrue();
        }

        [Test]
        public void IsValidNumber_ReturnsTrueOnNewValidator_WhenNewValidatorAcceptsNumberButOldDoesNot()
        {
            var oldValidator = new NumberValidator(3, 1);
            var newValidator = new NumberValidator(100, 99);
            var number = "1111.0";

            newValidator.IsValidNumber(number)
                .Should()
                .BeTrue();
        }

		[Test, Timeout(1000)]
        public void IsValidNumber_WorksFast_WhenValidatorExecutesManyOperations()
        {
            var number = "123456.123";
            var iterationsCount = 100000;

            for (var iterationIndex = 0; iterationIndex < iterationsCount; iterationIndex++)
            {
                numberValidator.IsValidNumber(number);
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