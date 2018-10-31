using System;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(17, 2, false, "0",
			TestName = "valid integer input without sign")]
		[TestCase(17, 2, true, "+0",
			TestName = "valid positive integer input")]
		[TestCase(17, 2, false, "-0",
			TestName = "valid negative integer input")]
		[TestCase(17, 3, true, "1.23",
			TestName = "valid input without sign")]
		[TestCase(17, 3, true, "+1.23",
			TestName = "valid positive input")]
		[TestCase(17, 3, false, "-1.23",
			TestName = "valid negative input")]
		[TestCase(17, 2, true, "0,0",
			TestName = "valid input with comma")]
		[TestCase(17, 3, false, "+1.23",
			TestName = "valid positive input when not only positive")]
		[TestCase(17, 2, true, "0.00",
			TestName = "valid input with fraction part length equal to scale")]
		[TestCase(4, 3, false, "00.00",
			TestName = "valid input with length equal to precision")]
		[TestCase(17, 2, false, "᠕",
			TestName = "valid input with mongolian digit")]
		public void IsValidNumber_Validates_When(int precision, int scale, bool onlyPositive, string input)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(input)
				.Should()
				.BeTrue();
		}

		[TestCase(17, 2, true, "-0",
			TestName = "input with negative zero when only positive")]
		[TestCase(3, 2, false, "-0.00",
			TestName = "negative input with length more then precision")]
		[TestCase(3, 2, true, "+0.00",
			TestName = "positive input with length more then precision")]
		[TestCase(17, 2, true, "0.000",
			TestName = "input with fraction part length more than scale")]
		[TestCase(17, 2, true, "12ab2.00",
			TestName = "input with letters in int part")]
		[TestCase(17, 2, true, "12.1a",
			TestName = "input with letters in fraction part")]
		[TestCase(17, 2, false, "-+0.0",
			TestName = "input with more than one signs")]
		[TestCase(3, 2, false, "",
			TestName = "empty input")]
		[TestCase(3, 2, false, null,
			TestName = "null input")]
		[TestCase(3, 2, false, "0.",
			TestName = "input with empty fraction part")]
		[TestCase(3, 2, false, ".0",
			TestName = "input with empty int part")]
		[TestCase(3, 2, false, "0..0",
			TestName = "input with two dots")]
		[TestCase(4, 2, false, " 0.0",
			TestName = "input with white space before number")]
		[TestCase(4, 2, false, "0.0 ",
			TestName = "input with white space after number")]
		[TestCase(17, 10, false, "1.1e1",
			TestName = "input with scientific view")]
		public void IsValidNumber_DoesNotValidate_When(int precision, int scale, bool onlyPositive, string input)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(input)
				.Should()
				.BeFalse();
		}


		[TestCase(-1, 0, false, "precision must be a positive number*",
			TestName = "precision less than zero")]
		[TestCase(0, 0, false, "precision must be a positive number*",
			TestName = "precision is equal to zero")]
		[TestCase(2, -1, false, "precision must be a non-negative number*",
			TestName = "scale less than zero")]
		[TestCase(2, 2, false, "precision must be a non-negative number*",
			TestName = "precision is equal to scale")]
		[TestCase(2, 3, false, "precision must be a non-negative number*",
			TestName = "precision less than scale")]
		public void Constructor_ThrowsArgumentException_When
			(int precision, int scale, bool onlyPositive, string expectedExceptionMessage)
		{
			Action ctorInvocation = () => new NumberValidator(precision, scale, onlyPositive);
			ctorInvocation.Should().Throw<ArgumentException>().WithMessage(expectedExceptionMessage);
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
				throw new ArgumentException(
					"precision must be a non-negative number less or equal than precision");
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