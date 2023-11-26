using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, TestName = "precision is non-positive")]
		[TestCase(null, 2, TestName = "precision is null")]
		[TestCase(1, -1, TestName = "scale is negative")]
		[TestCase(1, 2, TestName = "scale >= precision")]
		public void NumberValidator_CreateWithIncorrectValues_ThrowsException(int precision, int scale)
		{
			Action action = () => { new NumberValidator(precision, scale); };
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_CreateWithCorrectValues_Successes()
		{
			Action action = () => { new NumberValidator(1, 0, true); };
			action.Should().NotThrow();
		}

		[Test]
		public void IsValidNumber_CheckSameNumberTwoTimes_ResultsAreEqual()
		{
			var validator = new NumberValidator(17, 2, true);
            var numberToCheck = "0.0";

			var firstTimeCheck = validator.IsValidNumber(numberToCheck);
			var secondTimeCheck = validator.IsValidNumber(numberToCheck);

			firstTimeCheck.Should().Be(secondTimeCheck);
		}

		[TestCase(null, TestName = "value is null")]
		[TestCase("", TestName = "value is empty")]
		[TestCase("   ", TestName = "multiple whitespaces")]
		[TestCase("a.sd", TestName = "value has letters")]
		[TestCase("1.5.9", TestName = "more than one separator")]
		[TestCase("3^2", TestName = "value has special symbols")]
        public void IsValidNumber_CheckNonNumberValue_ReturnsFalse(string value)
		{
			var result = new NumberValidator(17, 2, true).IsValidNumber(value);

			result.Should().BeFalse();
        }

		[TestCase("1234", 3, 2, true, TestName = "integer and length > precision")]
		[TestCase("-1.3", 3, 2, true, TestName = "negative number and onlyPositive = true")]
		[TestCase("0.1234", 3, 2, false, TestName = "only decimal part and length > precision")]
		[TestCase("1.234", 3, 2, true, TestName = "total length > precision")]
		[TestCase("1.23", 3, 1, true, TestName = "decimal part length > scale")]
		[TestCase("+1.23", 3, 2, true, TestName = "has + and total length > precision")]
        public void IsValidNumber_CheckNotValidNumber_ReturnsFalse(string value, int precision, int scale, bool onlyPositive)
        {
	        var result = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);

            result.Should().BeFalse();
        }

		[TestCase("123", 3, 2, true, TestName = "integer and length = precision")]
		[TestCase("123", 4, 2,true, TestName = "integer and length < precision")]
		[TestCase("0.1", 3, 1, true, TestName = "only decimal part and length < precision")]
		[TestCase("0.12", 3, 2, true, TestName = "only decimal part and length = precision")]
		[TestCase("1.23", 4, 3, true, TestName = "decimal part length < scale")]
        [TestCase("1.23", 3, 2, true, TestName = "decimal part length = scale")]
		[TestCase("-1.23", 4, 2, false, TestName = "negative and onlyPositive = false")]
        public void IsValidNumber_CheckValidNumber_ReturnsTrue(string value, int precision, int scale, bool onlyPositive)
        {
	        var result = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);

	        result.Should().BeTrue();
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