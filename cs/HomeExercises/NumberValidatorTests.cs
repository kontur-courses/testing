using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test, Category("Constructor")]
		public void ValidConstructorParameters_WithNoException()
		{
			Assert.DoesNotThrow(() => new NumberValidator(2,1, true));
		}
		
		[TestCase(-11, 2, false, TestName = "precision is negative", Category = "Constructor")]
		[TestCase(8, -13, false, TestName = "scale is negative", Category = "Constructor")]
		[TestCase(7, 8, false, TestName = "precision is less than scale", Category = "Constructor")]
		[TestCase(0, 0, false, TestName = "precision is zero", Category = "Constructor")]
		public void InvalidConstructorParameters_ThrowsArgumentException(int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
		}

		[TestCase(null!, TestName = "Null input", Category = "IsValidNumber")]
		[TestCase("", TestName = "Empty string", Category = "IsValidNumber")]
		[TestCase("         ", TestName = "Whitespace string", Category = "IsValidNumber")]
		[TestCase("%;?^", TestName = "Other symbols", Category = "IsValidNumber")]
		[TestCase("ddd", TestName = "Non-numeric string", Category = "IsValidNumber")]
		public void isValidNumber_ShouldBeFalse_WhenNumberIsNotCorrect(string number)
		{
			var validator = new NumberValidator(8, 6, true);
			validator.IsValidNumber(number).Should()
				.BeFalse($"For {number}, IsValidNumber should return false as it's invalid data.");
		}
		
		[TestCase("13.5", true, Category = "IsValidNumber")]
		[TestCase("88", true, Category = "IsValidNumber")]
		[TestCase("7.00", true, Category = "IsValidNumber")]
		[TestCase("198.2", true, Category = "IsValidNumber")]
		[TestCase("0", true, Category = "IsValidNumber")]
		[TestCase("+2.0", true, Category = "IsValidNumber")]
		public void isValidNumber_ShouldBeTrue_WhenNumberIsValid(string number, bool result)
		{
			var validator = new NumberValidator(5, 2, true);
			validator.IsValidNumber(number).Should().Be(result);
		}
		[TestCase("-8", false)]
		[TestCase("-0.8", false)]
		[TestCase("+4.0", true)]
		[TestCase("+0.00", true)]
		public void isValidNumber_ShouldBeTrue_WhenNumberWithSigns(string number, bool result)
		{
			var validator = new NumberValidator(7, 3, true);
			validator.IsValidNumber(number).Should().Be(result);
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