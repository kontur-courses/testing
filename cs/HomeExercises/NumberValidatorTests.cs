using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void PrecisionShouldBeNotNegative()
		{
			new Func<NumberValidator>(() => new NumberValidator(-1, 2, true))
				.Should().Throw<ArgumentException>();
		}

		[Test]
		public void ScaleShouldBeNotNegative()
		{
			new Func<NumberValidator>(() => new NumberValidator(1, -1, true))
				.Should().Throw<ArgumentException>();
		}

		[Test]
		public void ScaleShouldBeLessThanPrecision()
		{
			new Func<NumberValidator>(() => new NumberValidator(1, 1, true))
				.Should().Throw<ArgumentException>();
			new Func<NumberValidator>(() => new NumberValidator(1, 2, true))
				.Should().Throw<ArgumentException>();
		}

		[Test]
		public void ValidateShouldBeFalseWhenValueIsEmptyStringOrNull()
		{
			Assert.IsFalse(new NumberValidator(10, 9, true).IsValidNumber(""));
			Assert.IsFalse(new NumberValidator(10, 9, true).IsValidNumber(null));
		}

		[Test]
		public void SeparatingSymbolShouldNotBeTakenInCountOfDigits()
		{
			Assert.IsTrue(new NumberValidator(2, 1, false).IsValidNumber("0.1"));
			Assert.IsTrue(new NumberValidator(2, 1, false).IsValidNumber("0,1"));
		}

		[Test]
		public void MinusShouldBeTakenInCountOfDigits()
		{
			Assert.IsFalse(new NumberValidator(1, 0, false).IsValidNumber("-1"));
			Assert.IsFalse(new NumberValidator(4, 3, false).IsValidNumber("-1.002"));
		}

		[Test]
		public void PlusShouldBeTakenInCountOfDigits()
		{
			Assert.IsFalse(new NumberValidator(1, 0, false).IsValidNumber("+1"));
			Assert.IsFalse(new NumberValidator(4, 3, false).IsValidNumber("+1.002"));
		}

		[TestCase(17, 2, true, "0.0")]
		[TestCase(17, 2, true, "0")]
		[TestCase(17, 2, false, "-1")]
		[TestCase(17, 2, false, "-1.1")]
		[TestCase(17, 2, false, "+1.1")]
		[TestCase(17, 2, false, "-1,1")]
		[TestCase(17, 2, false, "+1,1")]
		[TestCase(17, 2, false, "+1,11")]
		public void CorrectValidateWithCorrectNumber(int precision, int scale, bool onlyPositive, string number)
		{
			Assert.IsTrue(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number));
		}

		[TestCase(4, 3, false, "12212")]
		[TestCase(4, 3, false, "0.2212")]
		public void CorrectValidateWhenDigitsInNumberGreaterThenPrecision
			(int precision, int scale, bool onlyPositive, string number)
		{
			Assert.IsFalse(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number));
		}

		[TestCase(10, 3, false, "0.1234")]
		[TestCase(10, 2, false, "-12.1234")]
		[TestCase(10, 0, false, "0.1")]
		public void CorrectValidateWhenDigitsInFractionalPartGreaterThenScale
			(int precision, int scale, bool onlyPositive, string number)
		{
			Assert.IsFalse(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number));
		}

		[TestCase(10, 9, "-12.1234")]
		[TestCase(10, 9, "-0,1")]
		[TestCase(10, 9, "-0")]
		public void CorrectValidateWhenNegativeNumberAndOnlyPositiveMod
			(int precision, int scale, string number)
		{
			Assert.IsFalse(new NumberValidator(precision, scale, true).IsValidNumber(number));
		}

		[TestCase("ad.g")]
		[TestCase("ad.1")]
		[TestCase("1.g")]
		[TestCase("ad")]
		public void CorrectValidateWhenStrokeHasNotDigits
			(string stroke)
		{
			Assert.IsFalse(new NumberValidator(10, 9, true).IsValidNumber(stroke));
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