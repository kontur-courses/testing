using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private static void Process
		(string value,
			int precision = 5,
			int scale = 4,
			bool onlyPositive = false,
			bool expected = true)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			var isValidNumber = validator.IsValidNumber(value);

			if (!expected)
				isValidNumber.Should().BeFalse();

			else
				isValidNumber.Should().BeTrue();
		}

		[Test]
		public void PrecisionShouldBeNotNegative()
		{
			Assert.Throws<ArgumentException>(()
				=> new NumberValidator(-1, 2, true));
		}

		[Test]
		public void ScaleShouldBeNotNegative()
		{
			Assert.Throws<ArgumentException>(()
				=> new NumberValidator(1, -1, true));
		}

		[Test]
		public void ScaleCanBeZero()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0));
		}

		[TestCase(1, TestName = "Check when scale equally precision")]
		[TestCase(2, TestName = "Check when scale greater than precision")]
		public void ScaleShouldBeLessThanPrecision(int scale)
		{
			Assert.Throws<ArgumentException>(()
				=> new NumberValidator(1, scale));
		}

		[TestCase("", TestName = "Check empty string")]
		[TestCase(null, TestName = "Check null")]
		public void ValidateShouldBeFalseWhenValueIsEmptyStringOrNull(string value)
		{
			Process(value, expected: false);
		}

		[TestCase("0.1", TestName = "Check dot")]
		[TestCase("0,1", TestName = "Check comma")]
		public void SeparatingSymbolShouldNotBeTakenInCountOfDigits(string value)
		{
			Process(value, 2, 1);
		}

		[TestCase(1, 0, "-1", TestName = "Check negative integer")]
		[TestCase(1, 0, "+1", TestName = "Check positive integer")]
		[TestCase(4, 3, "-1.002", TestName = "Check negative real number")]
		[TestCase(4, 3, "+1.002", TestName = "Check positive real number")]
		public void SignShouldBeTakenInCountDigits(int precision, int scale, string value)
		{
			Process(value, precision, scale, expected: false);
		}

		[TestCase("0.0", TestName = "Check real number without sign")]
		[TestCase("0", TestName = "Check integer without sign")]
		[TestCase("-1", TestName = "Check integer with minus")]
		[TestCase("-1.1", TestName = "Check real number with minus")]
		[TestCase("+1.1", TestName = "Check real number with plus")]
		[TestCase("+1", TestName = "Check integer with plus")]
		public void CorrectValidateWithCorrectNumber(string value)
		{
			Process(value);
		}

		[TestCase("12212", TestName = "Check integer")]
		[TestCase("0.2212", TestName = "Check real number")]
		public void CorrectValidateWhenDigitsInNumberGreaterThenPrecision(string value)
		{
			Process(value, 4, 3, expected: false);
		}

		[TestCase(3, "0.1234", TestName = "Check positive number")]
		[TestCase(2, "-12.1234", TestName = "Check negative number")]
		[TestCase(0, "0.1", TestName = "Check when scale is 0")]
		[TestCase(3, "1.0000", TestName = "Check when number has many zeros in fractional part")]
		public void CorrectValidateWhenDigitsInFractionalPartGreaterThenScale
			(int scale, string value)
		{
			Process(value, 10, scale, expected: false);
		}

		[TestCase("-1", TestName = "Check integer")]
		[TestCase("-0,1", TestName = "Check real number")]
		[TestCase("-0", TestName = "Check zero with minus")]
		public void CorrectValidateWhenNegativeNumberAndOnlyPositiveMod(string value)
		{
			Process(value, 10, 9, true, false);
		}

		[TestCase("1ad.1g", TestName = "Check when non digits in ceil and fractional parts")]
		[TestCase("ad.1", TestName = "Check when non digits in ceil part")]
		[TestCase("1.g", TestName = "Check when non digits in fractional part")]
		[TestCase("a1d", TestName = "Check when non digits in ceil part without fractional part")]
		public void CorrectValidateWhenStrokeHasNotDigits(string value)
		{
			Process(value, expected: false);
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
}