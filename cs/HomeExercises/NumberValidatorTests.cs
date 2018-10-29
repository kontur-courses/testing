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
        private readonly NumberValidator numberValidator = GetNumberValidator();

        private static NumberValidator GetNumberValidator(int precision = 100,
            int scale = 99,
            bool onlyPositive = false)
            => new NumberValidator(precision, scale, onlyPositive);


	    [Test]
		[TestCase(-1, 3, false, TestName = "Validator gets negative precision")]
		[TestCase(0, 0, false, TestName = "Validator gets zero precision")]
		[TestCase(1, -1, false, TestName = "Validator gets negative scale")]
		[TestCase(1, 2, false, TestName = "Validator gets precision less than precision")]
		[TestCase(2, 2, false, TestName = "Validator gets precision equals to scale")]
	    [TestCase(9000, 10000, false, TestName = "Validator gets 9000 precision and 10000 scale")]

        public void CreationFails_WhenValidatorGetsIncorrectArguments(int precision, int scale, bool onlyPositive)
	    {
		    Assert.Throws<ArgumentException>(() => GetNumberValidator(precision, scale, onlyPositive));
	    }
		
        [Test]
		[TestCase(1, 0, false, TestName = "Precision is one and scale is zero")]
		[TestCase(2, 1, false, TestName = "Precision is two and scale is zero")]
		[TestCase(2, 1, true, TestName = "Validator can handle only positive numbers")]
		[TestCase(10000, 9000, false, TestName = "Validator gets 10000 precision and 9000 scale")]
        public void CreationIsSuccessful_WhenValidatorGetsCorrectArguments(int precision, int scale, bool onlyPositive)
        {
            Assert.DoesNotThrow(() => GetNumberValidator(precision, scale, onlyPositive));
        }
		
        [Test]
        [TestCase("abc", TestName = "Word instead of a number")]
        [TestCase("aaa.a", TestName = "Dot inside of word")]
        [TestCase(".", TestName = "One dot")]
		[TestCase("", TestName = "Empty string")]
		[TestCase(null, TestName = "Null instead of a string")]
        public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNotANumberWithoutDigits(String notANumberString)
        {
            numberValidator
                .IsValidNumber(notANumberString)
                .Should()
                .BeFalse();
        }

        [Test]
        [TestCase("string0.1", TestName = "Word before a number")]
        [TestCase("0.0string1", TestName = "Word inside of a number after a dot")]
        [TestCase("0.1string", TestName = "Word after a number")]
        [TestCase("112str1.1", TestName = "Word inside of a number before a dot")]
        public void IsValidNumber_ReturnsFalse_WhenValidatorGetsNotANumberWithNumber(String numberWithNotNumberString)
        {
            numberValidator
                .IsValidNumber(numberWithNotNumberString)
                .Should()
                .BeFalse();
        }

        [Test]
		[TestCase("-0.1", 100, 99, true, TestName = "Positive numbers validator gets a negative number")]
		[TestCase("11.001", 6, 1, false, TestName = "Scale of a number is bigger than possible")]
		[TestCase("110000.001", 5, 4, false, TestName = "Precision of a number is bigger than possible")]
		[TestCase("1000.0000000", 20, 3, false, TestName = "Gets zero partial with 6 digits, when maximum possible is 3")]
		[TestCase("00000000.1", 6, 4, false, TestName = "Gets zero integer part with 8 digits when maximum possible is 6")]
		[TestCase("1.001", 6, 2, false, TestName = "Integer part digits is less than scale, but fraction part digits is more than scale")]
		[TestCase("10.00001", 10, 4, false, TestName = "Integer contains less digits than a scale, but partial part contains more digits than the scale")]
        public void IsValidNumber_ReturnsFalse_WhenValidatorGetsWrongNumber(String numberString,
	        int precision,
	        int scale,
	        bool onlyPositive)
        {
            var validator = GetNumberValidator(precision, scale, true);

            validator
                .IsValidNumber(numberString)
                .Should()
                .BeFalse();
        }

	    [Test]
		[TestCase(".001", TestName = "No integer part before a dot")]
		[TestCase("100.", TestName = "No partial part after a dot")]
		[TestCase("100.0.0", TestName = "Too many dots in a number")]
	    public void IsValidNumber_ReturnsFalse_WhenValidatorGetsIncorrectNumberForms(String numberString)
	    {
		    numberValidator.IsValidNumber(numberString)
			    .Should()
			    .BeFalse();
	    }

	    [Test]
		[TestCase("-0.1", 100, 99, false, TestName = "All numbers validator gets a negative number")]
		[TestCase("+0.1", 100, 99, false, TestName = "All numbers validator gets a positive number")]
		[TestCase("+0.1", 100, 99, true, TestName = "Positive numbers validator gets a positive number")]
		[TestCase("0.0", 100, 99, true, TestName = "Positive numbers validator gets a zero number")]
		[TestCase("10.0", 100, 99, true, TestName = "Positive numbers validator gets a number without sign before")]
	    [TestCase("10000.0", 6, 2, false, TestName = "Validator gets a number with maximum precision")]
		[TestCase("11.00001", 10, 5, false, TestName = "Validator gets a number with maximum scale")]
		[TestCase("100", 100, 99, false, TestName = "Validator gets a number without partial part")]
		[TestCase("111111111111111111111111111111111111111.0", 100, 99, false, TestName = "Integer part is longer than long")]
		[TestCase("1.11111111111111111111111111111111111111", 100, 99, false, TestName = "Partial part is longer than 20 signs")]
	    public void IsValidNumber_ReturnsTrue_WhenValidatorGetsCorrectNumber(String numberString,
		    int precision,
		    int scale,
		    bool onlyPositive)
	    {
			var validator = new NumberValidator(precision, scale, onlyPositive);

		    validator.IsValidNumber(numberString)
			    .Should()
			    .BeTrue();
	    }
		
		[Test]
        public void IsValidNumber_ReturnsTrueOnOldValidator_WhenOldValidatorAcceptsNumberButNewDoesNot()
        {
            var oldValidator = new NumberValidator(100, 99);
            var newValidator = new NumberValidator(3, 1);
            var numberString = "1111.0";

            oldValidator.IsValidNumber(numberString)
                .Should()
                .BeTrue();
        }

        [Test]
        public void IsValidNumber_ReturnsTrueOnNewValidator_WhenNewValidatorAcceptsNumberButOldDoesNot()
        {
            var oldValidator = new NumberValidator(3, 1);
            var newValidator = new NumberValidator(100, 99);
            var numberString = "1111.0";

            newValidator.IsValidNumber(numberString)
                .Should()
                .BeTrue();
        }

        [Test, Timeout(1000)]
        public void IsValidNumber_WorksFast_WhenValidatorExecutesManyOperations()
        {
            var numberString = "123456.123";
            var iterationsCount = 100000;

            for (var iterationIndex = 0; iterationIndex < iterationsCount; iterationIndex++)
            {
                numberValidator.IsValidNumber(numberString);
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