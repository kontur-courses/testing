using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
	{
        [TestCase(2, 1, TestName = "constructor should be created when scale is less than precision")]
        [TestCase(1, 0, TestName = "constructor should be created when precision is greater than zero")]
        [TestCase(4, 0, TestName = "constructor should be created when scale is greater than or equal zero")]
        public void CheckConstructorWithoutThrowException(int precision, int scale)
        {
            Action act = () => new NumberValidator(precision, scale);

            act.Should().NotThrow<ArgumentException>();
        }

        [TestCase(0, 2, TestName = "constructor throw exception when precision is zero")]
		[TestCase(-1, 2, TestName = "constructor throw exception when precision is negative")]
		[TestCase(1, 2, TestName = "constructor throw exception when precision is less than scale")]
		[TestCase(2, 2, TestName = "constructor throw exception when precision is equal scale")]
		[TestCase(2, -1, TestName = "constructor throw exception when scale is negative")]
		public void CheckConstructorWithExceptions(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision, scale);

			act.Should().Throw<ArgumentException>();
		}

		[TestCase(4, 3, true, "5", true, TestName = "IsValidNumber returns true when scale is't zero and number is integer")]
		[TestCase(4, 3, true, " ", false, TestName = "IsValidNumber returns false when input is space")]
		[TestCase(4, 3, true, "5.e", false, TestName = "IsValidNumber returns false when input contains more than numbers")]
		[TestCase(4, 3, true, "", false, TestName = "IsValidNumber returns false when input is empty")]
		[TestCase(4, 3, true, ".0", false, TestName = "IsValidNumber returns false when number doesn't contain integer part")]
		[TestCase(4, 3, true, "+", false, TestName = "IsValidNumber returns false when input is only sign")]
		[TestCase(4, 3, true, "5.", false, TestName = "IsValidNumber returns false when number doesn't contains fractional part after dot")]
		[TestCase(4, 3, true, "+-5", false, TestName = "IsValidNumber returns false when input contains pair of signs")]
		[TestCase(2, 0, false, "-5", true, TestName = "IsValidNumber returns true when input is negative and validator is not only positive")]
		[TestCase(2, 0, false, "+5", true, TestName = "IsValidNumber returns true when input is positive and validator is not only positive")]
		[TestCase(3, 0, false, "-15", true, TestName = "IsValidNumber returns true when input length is less than or equal precision")]
		[TestCase(2, 0, false, "-15", false, TestName = "IsValidNumber returns false when input length is greater than precision")]
		[TestCase(2, 0, true, "0", true, TestName = "IsValidNumber returns true when input length is less than precision")]
		[TestCase(2, 0, true, "00", true, TestName = "IsValidNumber returns true when input length is equal precision")]
		[TestCase(2, 0, true, "100", false, TestName = "IsValidNumber returns false when input length is greater than precision")]
		[TestCase(2, 0, true, "0.0", false, TestName = "IsValidNumber returns false when scale is zero and input contains fractional part")]
		[TestCase(3, 2, false, "0.0", true, TestName = "IsValidNumber returns true when sum integer part length and fractional part length is less than precision")]
		[TestCase(3, 2, false, "-0.00", false, TestName = "IsValidNumber returns false when sum integer part with sign length and fractional part length is greater than precision")]
		[TestCase(3, 2, false, "-0.0", true, TestName = "IsValidNumber returns true when sum integer part with sign length and fractional part length is less than or equal precision")]
		[TestCase(4, 2, true, "00.00", true, TestName = "IsValidNumber returns true when fractional part is equal scale")]
		[TestCase(4, 2, true, "000.00", false, TestName = "IsValidNumber returns false when sum length integer and fractional part is greater than precision")]
		[TestCase(4, 2, true, "0.000", false, TestName = "IsValidNumber returns false when fractional part length is greater than scale")]
		[TestCase(2, 1, true, "-1.0", false, TestName = "IsValidNumber returns false when validator is only positive and input is negative")]
		public void CheckIsValidNumber(int precision, int scale, bool onlyPositive, string input, bool expectedResult)
        {
			var validator = new NumberValidator(precision, scale, onlyPositive);

            var actualResult = validator.IsValidNumber(input);

            actualResult.Should().Be(expectedResult);
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
			if (string.IsNullOrEmpty(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}