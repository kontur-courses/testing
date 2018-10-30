using System;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, TestName = "Negative precision")]
		[TestCase(0, 0, TestName = "Zero precision")]
		[TestCase(1, -2, TestName = "Negative scale")]
		[TestCase(2, 3, TestName = "Scale is greater then precision")]
		[TestCase(1, 2, TestName = "Scale equals precision")]
		public void Constructor_ThrowsException_OnWrongArguments(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision, scale, true);
			act.ShouldThrow<ArgumentException>();
		}

		[TestCase(2, 0, TestName = "Zero scale")]
		[TestCase(2, 1, TestName = "Positive scale is less then positive precision")]
		[TestCase(20000000, 1000000, TestName = "Scale and precision is more than 10000")]
		public void Constructor_DoesNotThrowException_OnCorrectArguments(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision, scale, true);
			act.ShouldNotThrow();
		}

		[TestCase("", TestName = "String is empty")]
		[TestCase(null, TestName = "It is null")]
		[TestCase("abc.5", TestName = "String contains non digit before dot")]
		[TestCase("abc", TestName = "String is non digit")]
		[TestCase("12.abc", TestName = "String contains non digit after dot")]
		[TestCase("12.12.12", TestName = "String contains 2 dots")]
		[TestCase(".12", TestName = "Empty string before dot")]
		[TestCase("12.", TestName = "Empty string after dot")]
		public void IsValid_ReturnFalse_WithWrongString(string value)
		{
			var validator = new NumberValidator(20, 10, true);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(20, 10, true, "-1.2", TestName = "Number should be positive but it is negative")]
		[TestCase(3, 2, true, "12.23", TestName = "Sum of int and frac parts is greater then precision")]
		[TestCase(5, 2, true, "0.000", TestName = "Frac part is greater then scale")]
		[TestCase(3, 2, true, "0000", TestName = "Integer part is greater then precision")]
		[TestCase(4, 3, false, "-12.32", TestName = "Number of digits and minus is greater then precision")]
		[TestCase(4, 3, false, "+12.32", TestName = "Number of digits and plus is greater then precision")]
		public void IsValid_ReturnFalse_OnWrongNumber(int precision, int scale, bool onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(6, 3, true, "111.222", TestName = "Precision and scale are equal to parts")]
		[TestCase(10, 5, true, "+1111.222", TestName = "Precision and scale are greater then parts and plus")]
		[TestCase(10, 5, false, "-1111.222", TestName = "Precision and scale are greater then parts and minus")]
		[TestCase(3, 0, true, "111", TestName = "Integer number")]
		[TestCase(3, 0, true, "+11", TestName = "Integer number with minus")]
		[TestCase(3, 0, false, "-11", TestName = "Integer number with plus")]
		[TestCase(3, 2, false, "0.11", TestName = "Integer part is zero")]
		[TestCase(3, 2, false, "0,11", TestName = "Number contains comma")]
		public void IsValid_ReturnTrue_OnCorrectNumber(int precision, int scale, bool onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase(20000, 2, TestName = "Length of int part is more then 10000")]
		[TestCase(2, 20000, TestName = "Length of frac part is more then 10000")]
		[TestCase(20000, 20000, TestName = "Length of int adn frac part is more then 10000")]
		public void IsValid_ReturnTrue_OnLongNumbers(int intPartLength, int fracPartLength)
		{
			var validator = new NumberValidator(200000, 100000, true);
			var valueBuilder = new StringBuilder();
			valueBuilder.Append('1', intPartLength);
			valueBuilder.Append('.');
			valueBuilder.Append('2', fracPartLength);
			var value = valueBuilder.ToString();
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[Test]
		public void IsValid_ReturnTrue_OnMoreThanOneNumbers()
		{
			var validator = new NumberValidator(10, 5);
			validator.IsValidNumber("+1.235").Should().BeTrue();
			validator.IsValidNumber("-1234.235").Should().BeTrue();
        }

		[Test]
		public void IsValid_ReturnTrue_WithMoreThanOneValidator()
		{
			var validator1 = new NumberValidator(2, 1, true);
			var validator2 = new NumberValidator(10, 5);
			validator2.IsValidNumber("-12.356").Should().BeTrue();
		}

        [Test, Timeout(1000)]
        public void IsValid_WorksFast_On1000Iterations()
		{
			var validator = new NumberValidator(10, 5);
			for (var i = 0; i < 10000; i++)
				validator.IsValidNumber("-1234.567");
		}

		[Test, Timeout(1000)]
		public void Constructor_WorksFast_On10000Iterations()
		{
			NumberValidator validator;
			for (var i = 0; i < 10000; i++)
				validator = new NumberValidator(10, 5);
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