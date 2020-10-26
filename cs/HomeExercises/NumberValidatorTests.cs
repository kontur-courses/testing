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
		public void ShouldntThrowException_WhenPositivePrecisionAndNonNegativeScale()
		{
			Action action = () => { new NumberValidator(1, 0, true); };
			action.Should().NotThrow();
		}
		
		[Test]
		public void ShouldThrowException_WhenNegativePrecision()
		{
			Action action = () => { new NumberValidator(-1, 0, true); };
			action.Should().Throw<ArgumentException>().WithMessage("precision must be a positive number");
		}

		[Test]
		[TestCase(1, -1, true)]
		[TestCase(2, 3, true)]
		[TestCase(3, 3, true)]
		public void ShouldThrowException_WhenPrecisionLessThenOrEqualToScale(int precision, int scale, bool onlyPositive)
		{
			Action action = () => { new NumberValidator(precision, scale, onlyPositive); };
			action.Should().Throw<ArgumentException>().WithMessage("precision must be a non-negative number less or equal than precision");
		}
		
		[Test]
		[TestCase(null)]
		[TestCase("")]
		public void False_WhenEmptyOrNullValue(string value)
		{
			new NumberValidator(3, 2, true).IsValidNumber(value).Should().BeFalse();
		}
		
		[Test]
		public void False_WhenGivenNegativeNumberToOnlyPositiveValidator()
		{
			new NumberValidator(3, 2, true).IsValidNumber("-2.0").Should().BeFalse();
		}
		
		[Test]
		[TestCase("-2.0a")]
		[TestCase("a2.0")]
		[TestCase("   ")]
		[TestCase("string")]
		[TestCase("s.g")]
		[TestCase("string\n2.0")]
		public void False_WhenGivenNoneNumericString(string value)
		{
			new NumberValidator(3, 2, true).IsValidNumber(value).Should().BeFalse();
		}
		
		[Test]
		public void False_WhenFractionalLengthGreaterThenScale()
		{
			new NumberValidator(5, 2, true).IsValidNumber("2.011").Should().BeFalse();
		}
		
		[Test]
		[TestCase(3, 2, true, "00.00")]
		[TestCase(3, 2, false, "+1.23")]
		[TestCase(3, 2, false, "-0.00")]
		[TestCase(3, 0, true, "0000")]
		[TestCase(4, 0, false, "-0000")]
		public void False_WhenNumberLengthGreaterThenPrecision(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		[TestCase(5, 3, true, "0.0.0")]
		[TestCase(5, 3, true, ".0")]
		[TestCase(5, 3, false, "-.0")]
		[TestCase(5, 1, true, "0.")]
		[TestCase(5, 1, true, ".")]
		public void False_OnInvalidNumber(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		[TestCase(5, 3, true, "0.0")]
		[TestCase(5, 3, false, "-1.000")]
		[TestCase(5, 1, true, "+110.0")]
		[TestCase(5, 1, false, "-110.0")]
		[TestCase(5, 1, false, "+110,0")]
		[TestCase(5, 0, true, "+110")]
		public void True_OnCorrectNumbers(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
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
				throw new ArgumentException("precision must be a non-negative number less or equal than precision"); // а еще тут очень странное сообщение в ошибке
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