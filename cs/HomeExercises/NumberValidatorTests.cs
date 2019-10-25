using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1, 0, true, "0", true)]
		[TestCase(2, 0, true, "1 1", false)]
		[TestCase(1, 2, true, "a.sd", false)]
		[TestCase(1, 0, true, "", false)]
		[TestCase(1, 0, true, " ", false)]
		[TestCase(2, 1, true, " 0.0", false)]
		[TestCase(1, 0, true, null, false)]
		[TestCase(3, 1, true, "01.0", true)]
		[TestCase(3, 0, true, "001", true)]
		[TestCase(2, 2, true, "111.0", true)]
		[TestCase(2, 0, true, "+11", false)]
		[TestCase(2, 0, true, "11.0", false)]
		[TestCase(2, 0, true, "-1", false)]
		[TestCase(1, 1, true, "+.1", false)]
		[TestCase(2, 0, false, "-1", true)]
		public void IsValidNumber_ReturnsRightResult(int leftCharsCount, int rightCharsCount, 
			bool onlyPositive, string value, bool expectedResult)
		{
			var precision = leftCharsCount + rightCharsCount;
			var validator = new NumberValidator(precision, rightCharsCount, onlyPositive);
			var actualResult = validator.IsValidNumber(value);
			actualResult.Should().Be(expectedResult);
		}

		[TestCase(-1, 0, "precision must be a positive number")]
		[TestCase(0, 0, "precision must be a positive number")]
		[TestCase(1, 1, "scale must be a non-negative number less or equal than precision")]
		[TestCase(1, -1, "scale must be a non-negative number less or equal than precision")]
		[TestCase(1, 2, "scale must be a non-negative number less or equal than precision")]
		public void NumberValidatorInitialise_ThrowsException(int precision, int scale, string message)
		{
			Action initialiseValidator = () => new NumberValidator(precision, scale);
			initialiseValidator.ShouldThrow<ArgumentException>().WithMessage(message);
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