using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidator_Should
	{
		[TestCase(17, 2, true, "0.0",
			TestName = "input length less than precision")]
		[TestCase(17, 2, true, "0",
			TestName = "integer input")]
		[TestCase(17, 2, false, "-0",
			TestName = "negative integer input")]
		[TestCase(4, 2, true, "+1.23",
			TestName = "input length with plus sign is equal to precision")]
		[TestCase(4, 2, false, "-1.23",
			TestName = "input length with minus sign is equal to precision")]
		[TestCase(17, 2, true, "0,0",
			TestName = "input with comma")]
		[TestCase(17, 2, false, "-0.00",
			TestName = "negative input")]
		[TestCase(17, 2, false, "᠕",
			TestName = "input with mongolian digit")]
		public void Validate_CorrectInput(int precision, int scale, bool onlyPositive, string input)
		{
			Assert.IsTrue(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(input));
		}

		[TestCase(17, 2, true, "-0",
			TestName = "negative zero with only positive")]
		[TestCase(3, 2, true, "00.00",
			TestName = "input length more then precision")]
		[TestCase(3, 2, true, "-0.00",
			TestName = "input length with minus sign more then precision")]
		[TestCase(3, 2, true, "+0.00",
			TestName = "input length with plus sign more then precision")]
		[TestCase(17, 2, true, "0.000",
			TestName = "fraction part length more than scale")]
		[TestCase(3, 2, true, "precision.sd",
			TestName = "letters in input")]
		[TestCase(17, 2, true, "12ab2.00",
			TestName = "letters with digits in input")]
		[TestCase(17, 2, true, "12.1a",
			TestName = "letters with digits in fraction part")]
		[TestCase(17, 2, false, "-+0.0",
			TestName = "more than one signs")]
		[TestCase(3, 2, false, "-0.00",
			TestName = "negative input length more than precision")]
		[TestCase(3, 2, false, "-0.001",
			TestName = "negative input fraction part length more than scale")]
		[TestCase(3, 2, true, "",
			TestName = "empty string with only positive")]
		[TestCase(3, 2, false, "",
			TestName = "empty string")]
		[TestCase(3, 2, false, null,
			TestName = "null")]
		[TestCase(3, 2, false, "0.",
			TestName = "fraction part is empty")]
		[TestCase(3, 2, false, ".0",
			TestName = "int part is empty")]
		[TestCase(3, 2, false, "0..0",
			TestName = "input with two dots")]
		[TestCase(4, 2, false, " 0.0",
			TestName = "input with white space")]
		public void NotValidate_IncorrectInput(int precision, int scale, bool onlyPositive, string input)
		{
			Assert.IsFalse(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(input));
		}

		[TestCase(-1, 2, true,
			TestName = "precision less than zero with only positive")]
		[TestCase(-1, 2, false,
			TestName = "precision less than zero")]
		[TestCase(2, -1, false,
			TestName = "scale less than zero")]
		[TestCase(2, 2, false,
			TestName = "precision is equal to scale")]
		[TestCase(2, 3, false,
			TestName = "precision less than scale")]
		[TestCase(-1, -2, false,
			TestName = "precision and scale less than zero")]
		[TestCase(0, 1, false,
			TestName = "precision is equal to zero")]
		public void ThrowArgumentException_When_IncorrectValidatorParams(
			int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
		}

		[TestCase(1, 0, true,
			TestName = "correct validator params with only positive")]
		[TestCase(1, 0, false,
			TestName = "correct validator params with not only positive")]
		public void NotThrowException_When_CorrectValidatorParams(int precision, int scale, bool onlyPositive)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
		}
	}

	public class NumberValidatorTests
	{
		[Test]
		public void Test()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("precision.sd"));
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
				throw new ArgumentException("precision must be precision positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException(
					"precision must be precision non-negative number less or equal than precision");
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