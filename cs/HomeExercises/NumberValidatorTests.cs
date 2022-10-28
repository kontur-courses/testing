using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1, 0, true, "1")]
		[TestCase(2, 0, true, "+1")]
		[TestCase(2, 0, false, "-1")]
		[TestCase(3, 1, true, "1.0")]
		[TestCase(10, 5, false, "-100.00001")]
		public void Validator_ShouldWork_WithCorrectData(int precision, int scale, bool onlyPositive, string number)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(number).Should().Be(true);
		}

		[Test]
		public void Validator_ShouldThrowArgumentException_WhenPrecisionNotPositive()
		{
			Action actionNegative = () => new NumberValidator(-1, 2, true);
			actionNegative.Should().Throw<ArgumentException>().WithMessage("precision must be a positive number");
			Action actionZero = () => new NumberValidator(0, 2, true);
			actionZero.Should().Throw<ArgumentException>().WithMessage("precision must be a positive number");
		}

		[Test]
		public void Validator_ShouldNotThrowArgumentException_WithCorrectData()
		{
			Action correctIni = () => new NumberValidator(1, 0, true);
			correctIni.Should().NotThrow<ArgumentException>();
		}

		[Test]
		public void Validator_ShouldThrowArgumentException_WhenScaleNegative()
		{
			Action action = () => new NumberValidator(17, -1, true);
			action.Should().Throw<ArgumentException>()
				.WithMessage("precision must be a non-negative number less or equal than precision");
		}

		[Test]
		public void Validator_ShouldThrowArgumentException_WhenPrecisionLessThanScale()
		{
			Action action = () => new NumberValidator(1, 2, true);
			action.Should().Throw<ArgumentException>()
				.WithMessage("precision must be a non-negative number less or equal than precision");
		}


		[TestCase(null, Description = "Should work correctly with null", TestName = "null")]
		[TestCase("", Description = "Should work correctly with empty string", TestName = "empty string")]
		[TestCase("+-0,0", Description = "Should work correctly with incorrect regex", TestName = "unmatched regex")]
		[TestCase("+a.bc", Description = "Should work correctly with incorrect number", TestName = "not a number")]
		public void Validator_ReturnFalse_WithIncorrectStringFormat(string number)
		{
			var validator = new NumberValidator(17, 2, true);
			validator.IsValidNumber(number).Should().Be(false);
		}

		[TestCase("0.001", Description = "Should match number's scale", TestName = "scale length")]
		[TestCase("5000.0", Description = "Should match number's precision", TestName = "precision length")]
		[TestCase("+5000", Description = "Character should be contained in number precision", TestName = "character")]
		public void Validator_ReturnFalse_WithNotValidNumberLenght(string number)
		{
			var validator = new NumberValidator(4, 2, true);
			validator.IsValidNumber(number).Should().Be(false);
		}

		[Test]
		public void Validator_ReturnFalse_WhenNumberDoesntMatchPositiveCondition()
		{
			var validator = new NumberValidator(3, 2, true);
			validator.IsValidNumber("-0.1").Should().Be(false);
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