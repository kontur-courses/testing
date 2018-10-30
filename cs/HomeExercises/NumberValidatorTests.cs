using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 5, TestName = "Precision is negative")]
		[TestCase(5, -1, TestName = "Scale is negative")]
		[TestCase(1, 5, TestName = "Scale greater then precision")]
		[TestCase(5, 5, TestName = "Scale equals precision")]
		public void ThrowsException_When(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
		}
		
		[Test]
		public void DoesNotThrowsException_WhenPrecisionAndScaleIsCorrect()
		{
			Assert.DoesNotThrow(() => new NumberValidator(5, 1, true));
		}
		
		[TestCase(17, 2, false, "0", TestName = "Number without fraction")]
		[TestCase(17, 2, false, "0.0", TestName = "Number with fraction divided by dot")]
		[TestCase(4, 2, true, "+1.23", TestName = "When number with plus sign is equals precision")]
		[TestCase(17, 2, true, "0,0", TestName = "Number with fraction divided by comma")]
		public void ShouldBeTrue_WhenNumberIsValid(int precision, int scale, bool onlyPositive, string number)
		{
			var result = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number);
			result.Should().BeTrue();
		}
		
		[TestCase(3, 2, false, "00.00", TestName = "Number is grater than precision")]
		[TestCase(5, 2, true, "-0.00", TestName = "Negative numbers are forbidden and number is negative")]
		[TestCase(3, 2, true, "+0.00", TestName = "Number with plus sign is greater then precision")]
		[TestCase(3, 2, false, "-1.23", TestName = "Number with minus sign is greater then precision")]
		[TestCase(17, 2, true, "0.000", TestName = "Fraction is greater then allowed")]
		[TestCase(3, 2, true, "a.sd", TestName = "Value is not a number")]
		[TestCase(17, 2, true, "  0.0", TestName = "Value starts with whitespaces")]
		[TestCase(17, 2, true, "0.0  ", TestName = "Value ends with whitespaces")]
		[TestCase(17, 2, true, "0 . 0", TestName = "Value has inside whitespaces")]
		[TestCase(17, 2, true, null, TestName = "Value is null")]
		[TestCase(17, 2, true, "", TestName = "Value is empty string")]
		public void ShouldBeFalse_WhenNumberIsInvalid(int precision, int scale, bool onlyPositive, string number)
		{
			var result = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number);
			result.Should().BeFalse();
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