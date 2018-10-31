using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidator_Should
	{
		[TestCase(-1, TestName = "precision is negative")]
		[TestCase(0, TestName = "precision is zero")]
		[TestCase(1, -1, TestName = "scale is negative")]
		[TestCase(1, 2, TestName = "scale is higher than precision")]
		[TestCase(1, 1, TestName = "scale is equal to precision")]
		public void Constructor_ThrowsArgumentExceptionWhen(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action createValidator = () => new NumberValidator(precision, scale, onlyPositive);
			createValidator.ShouldThrow<ArgumentException>($"arguments are ({precision}, {scale}, {onlyPositive})");
		}

		[TestCase("1.23", TestName = "number has no sign")]
		[TestCase("+1.2", TestName = "positive number has a sign")]
		[TestCase("-1.2", TestName = "number is negative")]
		[TestCase("1", 1, 0, false, TestName = "number is an integer")]
		[TestCase("1,23", TestName = "number uses a comma separator")]
		[TestCase("1.23", int.MaxValue, 2, false, TestName = "length is less than precision")]
		[TestCase("1.23", int.MaxValue, int.MaxValue - 1, false, TestName = "fractional part shorter than scale")]
		public void IsValidNumber_ReturnsTrueWhen(string value, int precision = 3, int scale = 2, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue(
				$"arguments are ({value}, {precision}, {scale}, {onlyPositive})");
		}

		[TestCase(null, TestName = "value is null")]
		[TestCase("", TestName = "value is empty")]
		[TestCase("-1.23", TestName = "precision is higher than allowed")]
		[TestCase("1.234", TestName = "scale is higher than allowed")]
		[TestCase("-1.2", 3, 2, true, TestName = "number is negative when only positive numbers are allowed")]
		[TestCase(".12", TestName = "integer part is missing")]
		[TestCase("+.12", TestName = "integer part is missing with sign")]
		[TestCase("1.", TestName = "fractional part is missing")]
		[TestCase("1.2.3", TestName = "two separators present")]
		[TestCase("a1.2", TestName = "illegal characters present before number")]
		[TestCase("1.2a", TestName = "illegal characters present after number")]
		[TestCase("1a.2", TestName = "illegal characters present in integer part")]
		[TestCase("1.a2", TestName = "illegal characters present in fractional part")]
		public void IsValidNumber_ReturnsFalseWhen(string value, int precision = 3, int scale = 2, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse(
				$"arguments are ({value}, {precision}, {scale}, {onlyPositive})");
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