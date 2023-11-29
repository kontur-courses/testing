using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(17, 1, true, "0.0", true,
            TestName = "Is valid number when length of number less than precision and have fractional part")]
        [TestCase(17, 2, true, "0", true, 
			TestName = "Is valid number when length of number less than precision and don't have fractional part")]
        [TestCase(3, 2, true, "00.00", false,
			TestName = "Is not valid number when multiple zeros in int part and length of number greater than precision")]
        [TestCase(3, 2, true, "-0.0", false,
			TestName = "Is not valid number when number is negative with sign and onlyPositive is true")]
        [TestCase(3, 2, true, "+0.00", false, 
			TestName = "Is not valid number when number is positive with sign and length of number greater than precision")]
        [TestCase(4, 2, true, "+1.23", true, 
			TestName = "Is valid number when number is positive with sign and onlyPositive is true")]
        [TestCase(3, 2, false, "+1.23", false, 
			TestName = "Is not valid number when length of number greater than precision")]
        [TestCase(17, 2, true, "0.000", false, 
			TestName = "Is not valid number when fracPart greater than scale")]
        [TestCase(3, 2, true, "-1.23", false, 
			TestName = "Is not valid number when onlyPositive is true and number is negative")]
        [TestCase(3, 2, true, "a.sd", false, 
			TestName = "Is not valid number when number consists of letters")]
        [TestCase(3, 2, true, "", false, 
			TestName = "Is not valid number when number is empty")]
        [TestCase(3, 2, true, null, false, 
			TestName = "Is not valid number when number is null")]
        [TestCase(4, 2, false, "10*00", false,
			TestName = "Is not valid number when invalid separator")]
        [TestCase(4, 1, false, "1.0.0", false,
			TestName = "Is not valid number when more than one separator")]
		[TestCase(7, 3, false, "+1.00", true,
			TestName = "Is valid number when number is positive and onlyPositive is false")]
        public void ValidateNumberCorrect_When_DataCorrect(int precision, int scale, bool onlyPositive, string value, bool expectedResult)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            var result = validator.IsValidNumber(value);
            result.Should().Be(expectedResult);
        }

        [TestCase(-1, 2, true, 
			TestName = "Thorw argument exception when precision is not positive number")]
        [TestCase(1, -2, false, 
			TestName = "Throw argument exception when scale is negative number")]
        [TestCase(1, 2, true, 
			TestName = "Throw argument exception when precision is positive and scale is non-negative and scale greater than precision")]
        [TestCase(1, 1, false, 
			TestName = "Throw argument exception when precision is positive and scale is non-negative and equal to each other")]
        public void Should_ThrowArgumentException_When_IncorrectData(int precision, int scale, bool onlyPositive)
        {
            Action action = () => new NumberValidator(precision, scale, onlyPositive);
            action.Should().Throw<ArgumentException>();
        }

        [TestCase(1, 0, true,
			TestName = "Don't throw argument exception when precision is positive and scale is zero and precision greater than scale")]
        [TestCase(2, 1, true, 
			TestName = "Don't throw argument exception when precision is positive and scale is positive and precision greater than scale")]
        public void Should_DoesNotThrowArgumentException_When_CorrectData(int precision, int scale, bool onlyPositive)
        {
            Action action = () => new NumberValidator(precision, scale, onlyPositive);
            action.Should().NotThrow();

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