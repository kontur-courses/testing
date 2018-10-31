using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-10, TestName = "On negative precision")]
		[TestCase(0, TestName = "On zero precision")]
		[TestCase(10, -1, TestName = "On negative scale and correct precision")]
		[TestCase(10, 20, TestName = "When scale more then precision")]
		[TestCase(10, 10, TestName = "When scale equals precision")]
		public void Constructor_ShouldRaiseArgumentException(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action constructorCall = () => new NumberValidator(precision, scale, onlyPositive);
			constructorCall.Should().Throw<ArgumentException>();
		}

		[TestCase(4, 0, TestName = "When precision is more then 0 and scale equals zero")]
		[TestCase(4, 2, TestName = "When precision is more then 0 and scale between zero and precision")]
		[TestCase(3, 2, false, TestName = "When precision equals scale+1 on validator allowing negative numbers")]
		public void Constructor_ShouldWorkProperly(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action constructorCall = () => new NumberValidator(precision, scale, onlyPositive);
			constructorCall.Should().NotThrow();
		}

		[TestCase(10, 5, true, "123,4", TestName = "When number divided by comma")]
		[TestCase(10, 5, true, "123.4", TestName = "When number divided by dot")]
        [TestCase(11, 5, false, "+0.0", TestName = "On zero with \"plus\"")]
		[TestCase(11, 5, false, "-0.0", TestName = "On zero with \"minus\"")]
        [TestCase(11, 5, false, "-234567890.1", TestName = "When number length including \"minus\" equals precision")]
        [TestCase(11, 5, false, "+234567890.1", TestName = "When number length including \"plus\" equals precision")]
        [TestCase(10, 5, true, "123", TestName = "When number without fractional part")]
		[TestCase(10, 5, true, "000.000", TestName = "When fractional and int parts consist only of zeros")]
		[TestCase(10, 5, true, "000.123", TestName = "When number starts with zeros")]
		[TestCase(10, 5, true, "1.000", TestName = "When number ends with zeros")]
        [TestCase(10, 5, true, "12345.12345", TestName = "When fractional part length equal scale")]
        [TestCase(10, 5, true, "123456.1234", TestName = "When fractional and int parts equal precision")]
        public void IsValidNumber_ShouldReturnTrue(int precision, int scale, bool onlyPositive, string input)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(input).Should().BeTrue();
        }

		[TestCase(10, 5, true, "123.", TestName = "When number ends with divider")]
		[TestCase(10, 5, true, ".123", TestName = "When number starts with divider")]
        [TestCase(10, 5, true, "", TestName = "On empty input")]
		[TestCase(10, 5, true, "abc.def", TestName = "On NaN")]
        [TestCase(10, 5, true, null, TestName = "On null input")]
		[TestCase(11, 5, false, "+234567890.12", TestName = "When number length including \"plus\" is more then precision")]
		[TestCase(11, 5, false, "-234567890.12", TestName = "When number length including \"minus\" is more then precision")]
        [TestCase(10, 5, true, "-1.1", TestName = "When negative number on non negative validator")]
        [TestCase(10, 5, true, "12.123456", TestName = "When fractional part length more then scale")]
        [TestCase(10, 5, true, "12345.123456", TestName = "When fractional and int parts are more then precision")]
        public void IsValidNumber_ShouldReturnFalse(int precision, int scale, bool onlyPositive, string input)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(input).Should().BeFalse();
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
				// Могу предположить, что в исходном варианте были опечатки:
	            // throw new ArgumentException("precision must be a non-negative number less or equal than precision");
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