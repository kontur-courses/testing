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
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true)); //валидатор должен выкидывать исключение
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));//валидатор не должен выкидывать исключение
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false)); 
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true)); // два одинаковых зачем то

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0")); // просто проверка номера
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
		public void NumberValidator_WrongPrecision_ThrowsArgumentException()
        {
			Action action = () => new NumberValidator(-1, 2, true);
			action.Should().Throw<ArgumentException>();
        }

		[Test]
		public void NumberValidator_RightParameters_DoesNotThrows()
        {
			Action action = () => new NumberValidator(1, 0, true);
			action.Should().NotThrow();
        }

		[TestCase(17, 2, "0.0")]
		[TestCase(17, 2, "0")]
		[TestCase(4, 2, "+1.23")]
		public void NumberValidator_Validate_True(int precision, int scale, string number)
        {
			var validator = new NumberValidator(precision, scale, true);
			validator.IsValidNumber(number).Should().BeTrue();
        }

		[TestCase(3, 2, "00.00")]
		[TestCase(3, 2, "-0.00")]
		[TestCase(3, 2, "+0.00")]
		[TestCase(3, 2, "+1.23")]
		[TestCase(17, 2, "0.000")]
		[TestCase(3, 2, "-1.23")]
		[TestCase(3, 2, "a.sd")]
		public void NumberValidator_Validate_False(int precision, int scale, string number)
        {
			var validator = new NumberValidator(precision, scale, true);
			validator.IsValidNumber(number).Should().BeFalse();
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