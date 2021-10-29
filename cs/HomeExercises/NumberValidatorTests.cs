using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{

		[Test(Description = "Presision is negative")]
		public void NumberValidator_WrongPrecision_ThrowsArgumentException()
        {
			Action action = () => new NumberValidator(-1, 2);
			action.Should().Throw<ArgumentException>();
        }

		[Test]
		public void NumberValidator_RightParameters_DoesNotThrows()
        {
			Action action = () => new NumberValidator(1);
			action.Should().NotThrow();
        }

		[TestCase(-1, TestName = "Scale is negative")]
		[TestCase(2, TestName ="Scale is more then precision")]
		public void NumberValidator_WrongScale_ThrowsArgumentException(int scale)
        {
			Action action = () => new NumberValidator(1, scale);
			action.Should().Throw<ArgumentException>();
		}

		[TestCase(17, 2, "0.0")]
		[TestCase(4, 2, "+1.23", TestName = "With plus in begining")]
		[TestCase(17, 2, "0", TestName = "Without fraction part")]
		[TestCase(9, 2, "1,23", TestName = "With comma")]
		[TestCase(5, 2, "-1.23", TestName = "With minus")]
		public void NumberValidator_Validate_True(int precision, int scale, string number)
        {
			var validator = new NumberValidator(precision, scale);
			validator.IsValidNumber(number).Should().BeTrue(number);
        }

		[TestCase(1, 0, false, "", TestName = "Empty word")]
		[TestCase(3, 0, false, "   ", TestName = "Just spaces")]
		[TestCase(3, 2, false, "00.00", TestName = "Length more then precision")]
		[TestCase(3, 2, false, "-0.00", TestName = "Full length more then precision")]
		[TestCase(17, 2, false, "0.000", TestName = "Fraction part more then scale")]
		[TestCase(3, 2, false, "a.sd", TestName = "Not a number")]
		[TestCase(4, 2, true, "-123", TestName = "Only positive numbers")]
		[TestCase(5, 2, false, ".12", TestName = "Without int part")]
		public void NumberValidator_Validate_False(int precision, int scale, bool onlyPositive,
			string number)
        {
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(number).Should().BeFalse(number);
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