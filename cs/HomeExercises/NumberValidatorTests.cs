using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void CreatesWithNoExceptions()
		{
			var creationOfValidator = new Action(() => { new NumberValidator(2, 1, true); });
			creationOfValidator.Should().NotThrow();
		}

		[TestCase(-1, 2, false, TestName = "Precision < 0")]
		[TestCase(3, -1, false, TestName = "Scale < 0")]
		[TestCase(3, 4, false, TestName = "Precision < Scale")]
		public void ShouldThrowException_WhenInitWithIncorrectData(int precision, int scale, bool onlyPositive)
		{
			var creationOfValidatorOnlyPositive = new Action(() => { new NumberValidator(precision, scale, onlyPositive); });
			creationOfValidatorOnlyPositive.Should().Throw<ArgumentException>();
		}

		[Test]
		public void ShouldNotValidate_Null()
		{
			string number = null!;
			var validator = new NumberValidator(5, 2);
			validator.IsValidNumber(number).Should().BeFalse("null should be false");
		}

		[Test]
		public void ShouldNotValidate_Empty()
		{
			const string number = "";
			var validator = new NumberValidator(5, 2);
			validator.IsValidNumber(number).Should().BeFalse("empty should be false");
		}

		[Test]
		public void ShouldNotValidate_NonNumeric()
		{
			const string notNumber = "a.bc";
			var validator = new NumberValidator(5, 2);
			validator.IsValidNumber(notNumber).Should().BeFalse($"{notNumber} is not a number");
		}

		[TestCase("12.34", true)]
		[TestCase("12", true)]
		[TestCase("1.000", false)]
		[TestCase("123.000", false)]
		public void ShouldValidate_NumbersWithoutSign(string inputValue, bool expected)
		{
			var validator = new NumberValidator(5, 2, true);
			validator.IsValidNumber(inputValue).Should().Be(expected, $"number {inputValue} is {(expected ? "correct" : "incorrect")}");
		}

		[TestCase("+1.23", true)]
		[TestCase("+1", true)]
		[TestCase("+0.5", true)]
		[TestCase("+0.567", false)]
		[TestCase("+123.45", false)]
		public void ShouldValidate_NumbersWithPositiveSign(string inputValue, bool expected)
		{
			var validator = new NumberValidator(5, 2, true);
			validator.IsValidNumber(inputValue).Should().Be(expected, $"number {inputValue} is {(expected ? "correct" : "incorrect")}");
		}

		[TestCase("-1.23", true)]
		[TestCase("-1", true)]
		[TestCase("-0.5", true)]
		[TestCase("-0.567", false)]
		[TestCase("-123.45", false)]
		public void ShouldValidate_NumbersWithNegativeSign(string inputValue, bool expected)
		{
			var positiveValidator = new NumberValidator(5, 2, true);
			positiveValidator.IsValidNumber(inputValue).Should().Be(false, "only positive validator should not validate negatives");

			var validator = new NumberValidator(5, 2);
			validator.IsValidNumber(inputValue).Should().Be(expected, $"number {inputValue} is {(expected ? "correct" : "incorrect")}");
		}

		[TestCase("1,23", true)]
		[TestCase("+1,23", true)]
		[TestCase("-1,23", true)]
		[TestCase("-123,45", false)]
		[TestCase("0,456", false)]
		public void ShouldValidate_DifferentDelimiter(string inputValue, bool expected)
		{
			var validator = new NumberValidator(5, 2);
			validator.IsValidNumber(inputValue).Should().Be(expected, $"{expected} should validate comma");
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