using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
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
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
		}

		[Test]
		public void NumberValidator_Should_Throw_ArgumentException_When_Precision_Is_Not_Positive()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
        }

		[Test]
		public void NumberValidator_Should_Throw_ArgumentException_When_Scale_Is_Negative()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(3, -2, true));
		}

		[Test]
		public void NumberValidator_Should_Throw_ArgumentException_When_Scale_More_Than_Precision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(12, 14, true));
		}

		[Test]
		public void NumberValidator_Should_Does_Not_Throw_When_Params_Is_Correct()
		{
			Assert.DoesNotThrow(()=> new NumberValidator(1,0,true));
		}

		[Test]
		public void IsValidNumber_Should_Return_False_When_Value_Is_Null()
		{
			var numberValidator = new NumberValidator(17, 3, true);

			numberValidator.IsValidNumber(null).Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_Should_Return_False_When_Value_Is_Empty()
		{
			var numberValidator = new NumberValidator(17, 3, true);

			numberValidator.IsValidNumber("").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_Should_Return_False_When_Value_Has_Incorrect_Form()
		{
			var numberValidatior = new NumberValidator(17, 3, true);
			numberValidatior.IsValidNumber("ab.c").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_Should_Return_False_When_Value_Length_More_Than_Precision()
		{
			var numberValidator = new NumberValidator(3, 1, true);
			numberValidator.IsValidNumber("000.0").Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_Should_Return_False_When_Value_FracPart_Length_More_Than_Scale()
		{
			var numberValidator = new NumberValidator(3, 1, true);
			numberValidator.IsValidNumber("+0.00").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_Should_Return_False_When_OnlyPositive_Is_True_And_Value_Is_Negative()
		{
			var numberValidator = new NumberValidator(3, 1, true);
			numberValidator.IsValidNumber("-0.0").Should().BeFalse();
		}
		[Test]
		public void IsValidNumber_Should_Return_False_When_OnlyPositive_Is_False_And_Value_Is_Negative()
		{
			var numberValidator = new NumberValidator(3, 1, false);
			numberValidator.IsValidNumber("-0.0").Should().BeTrue();
		}

		[TestCase("0.0")]
		[TestCase("+0.0")]
		[TestCase("-0.0")]
		public void IsValidNumeber_Should_Return_True_When_Value_IsCorrect(string value)
		{
			var numberValidator = new NumberValidator(3, 1, false);
			numberValidator.IsValidNumber(value).Should().BeTrue();
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