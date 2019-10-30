using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidator_Tests
	{
		[TestCase(-2, 5, TestName = "Negative precision")]
		[TestCase(0, 5, TestName = "Zero precision")]
		public void Test_Throws_OnIncorrectPrecision(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale, true);
			action
				.ShouldThrow<ArgumentException>()
				.WithMessage("precision must be a positive number");
        }

		[TestCase(1, 2, true, TestName = "Precision less then scale")]
		[TestCase(1, 1, true, TestName = "Precision is equal to scale")]
		[TestCase(2, -1, true, TestName = "Negative scale")]
		public void Test_Throws_OnIncorrectScale(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action
				.ShouldThrow<ArgumentException>()
				.WithMessage("precision must be a non-negative number less or equal than precision");
		}


		[TestCase(5, 2, true, TestName = "With OnlyPositive flag")]
		[TestCase(5, 2, false, TestName = "Without onlyPositive flag")]
		[TestCase(5, 0, false, TestName = "Scale is zero")]
        public void Test_NotThrow_OnCorrectInput(int precision, int scale, bool onlyPositive)
        {
	        Action action = () => new NumberValidator(5, 2, true);
	        action.ShouldNotThrow();
        }


        [TestCase("0", TestName = "On Int zero")]
        [TestCase("+0", TestName = "On Signed zero with plus")]
        [TestCase("0.00", TestName = "On zero with fractional part")]
        [TestCase("00000", TestName = "On Number precision is equal than Validator precision")]
		[TestCase("1.23", TestName = "On unsigned Number with point")]
		[TestCase("1,23", TestName = "On unsigned Number with comma")]
		[TestCase("+1.23", TestName = "On Signed number with plus")]
        public void Test_IsValid_WithOnlyPositiveFlag(string number)
        {
			var numberValidator = new NumberValidator(5, 2, true);

			numberValidator.IsValidNumber(number).Should().BeTrue();
        }

		[TestCase(5, 2, "-1.23", TestName = "On Negative number")]
		[TestCase(3, 2, "0000", TestName = "On Precision is less than the number precision")]
		[TestCase(5, 2, "-0", TestName = "On Negative zero")]
		[TestCase(5, 2, "0,000", TestName = "On Scale is less than the number scale")]
		[TestCase(5, 2, "b.ab", TestName = "On String from letters")]
		[TestCase(5, 2, "2..3", TestName = "On Doubled point")]
		[TestCase(5, 2, "+-5", TestName = "On Doubled sign")]
		[TestCase(5, 2, "2a.5b", TestName = "On Number contains letters")]
		[TestCase(5, 2, null, TestName = "On Null string")]
		[TestCase(5, 2, "", TestName = "On Empty string")]
		[TestCase(5, 2, ".25", TestName = "On Number without int part")]
		[TestCase(5, 2, "25.", TestName = "On Number with point without fractional part")]
		public void Test_IsNotValid_WithOnlyPositiveFlag(int precision, int scale, string number)
        {
			var numberValidator = new NumberValidator(precision, scale, true);

	        numberValidator.IsValidNumber(number).Should().BeFalse();
        }

		[TestCase("+1.24", TestName = "On positive number")]
		[TestCase("-0", TestName = "On negative zero")]
		[TestCase("-1.23", TestName = "On negative number with point")]
		[TestCase("-1,23", TestName = "On negative number with comma")]
        public void Test_IsValid_WithoutOnlyPositiveFlag(string number)
		{
			var numberValidator = new NumberValidator(5, 2);

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