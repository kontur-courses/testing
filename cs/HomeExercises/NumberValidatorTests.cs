using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidator_Tests
	{
		[TestCase(-2, 1, true, TestName = "Precision is negative")]
		[TestCase(2, -1, true, TestName = "Scale is negative")]
		[TestCase(1, 2, false, TestName = "Scale is larger precision")]
		public void NumberValidator_ThrowsException_When(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_DoesNotThrowsException_WhenCreatingWithCorrectArguments()
		{
			Action action = () => new NumberValidator(1, 0, true);
			action.Should().NotThrow<ArgumentException>();
		}

		[TestCase(5, 3, false, "12345", TestName = "Valid integer")]
		[TestCase(5, 4, false, "+1", TestName = "Valid integer with plus sign")]
		[TestCase(5, 4, false, "-1", TestName = "Valid integer with minus sign")]
		[TestCase(5, 3, false, "1.23", TestName = "Fractional number parts separated by point")]
		[TestCase(5, 3, false, "1,23", TestName = "Fractional number parts separated by comma")]
		[TestCase(5, 3, false, "1.23", TestName = "Fractional number parts separated by point with sign")]
		[TestCase(5, 3, false, "1,23", TestName = "Fractional number parts separated by comma with sign")]
		public void IsValidNumber_ReturnsTrue_If(int precision, int scale, bool onlyPositive, string number)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(number).Should().BeTrue();
		}

		[TestCase(3, 2, true, "123.4", TestName = "Length of number is longer precision")]
		[TestCase(3, 2, true, "+123.4", TestName = "Length of number with sign is longer precision")]
		[TestCase(3, 2, true, "1.234", TestName = "Length of number fraction part is longer scale")]
		[TestCase(3, 2, true, "+1.234", TestName = "Length of number fraction part with sign is longer scale")]
		[TestCase(5, 2, true, "as.df", TestName = "Argument is not a number")]
		[TestCase(3, 2, true, "", TestName = "Argument is empty string")]
		[TestCase(3, 2, true, null, TestName = "Null argument")]
		[TestCase(3, 2, true, "-1.2", TestName = "Argument is negative number, but only positive numbers expected")]
		[TestCase(3, 2, false, ".23", TestName = "No number in front of the point")]
		[TestCase(3, 2, false, "1.", TestName = "No number after the point")]
		[TestCase(3, 2, false, ".", TestName = "Only point without number")]
		[TestCase(3, 2, false, ",", TestName = "Only comma without number")]
		[TestCase(3, 2, false, "+", TestName = "Only plus sign without number")]
		[TestCase(3, 2, false, "-", TestName = "Only minus sign without number")]
		public void IsValidNumber_ReturnsFalse_If(int precision, int scale, bool onlyPositive, string number)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(number).Should().BeFalse();
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