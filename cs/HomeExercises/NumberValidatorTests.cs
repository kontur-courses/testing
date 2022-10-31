using System;
using System.Text.RegularExpressions;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[Test]
		[TestCase(-1, 2, true, TestName = "Creation_NegativePrecision_ShouldThrowException")]
		[TestCase(0, 0, true, TestName = "Creation_PrecisionIsZero_ShouldThrowException")]
		[TestCase(2, -1, true, TestName = "Creation_NegativeScale_ShouldThrowException")]
		[TestCase(1, 2, true, TestName = "Creation_ScaleGreaterThenPrecision_ShouldThrowException")]
		
		public void Creation_ShouldThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			Action creation = () => new NumberValidator(precision, scale, onlyPositive);
			creation.Should().Throw<ArgumentException>();
		}

		[Test]
		[TestCase(1, 0, true, TestName = "Creation_PositivePrecision_ShouldNotThrowException")]
		public void Creation_ShouldNotThrowException(int precision, int scale, bool onlyPositive)
		{
			Action creation = () => new NumberValidator(precision, scale, onlyPositive);
			creation.Should().NotThrow<ArgumentException>();
		}

		[Test]
		[TestCase(17, 2, true, null, TestName = "IsValidNumber_NullString_ShouldBeFalse")]
		[TestCase(17, 2, true, "", TestName = "IsValidNumber_EmptyString_ShouldBeFalse")]
		[TestCase(3, 2, true, "a.sd", TestName = "IsValidNumber_NotNumber_ShouldBeFalse")]
		[TestCase(17, 2, true, ".0", TestName = "IsValidNumber_WithoutIntPart_ShouldBeFalse")]
		[TestCase(3, 2, true, "00.00",
			TestName = "IsValidNumber_NumberPrecisionGreaterThanValidatorPrecision_ShouldBeFalse")]
		[TestCase(3, 2, true, "+1.23",
			TestName = "IsValidNumber_NumberPrecisionGreaterThanValidatorPrecision_ShouldBeFalse")]
		[TestCase(17, 2, true, "0.000",
			TestName = "IsValidNumber_NumberScaleGreaterThanValidatorScale_ShouldBeFalse")]
		[TestCase(10, 5, true, "-1.23",
			TestName = "IsValidNumber_NegativeNumberWithOnlyPositiveValidator_ShouldBeFalse")]
		
		public void IsValidNumber_ShouldBeFalse(int precision, int scale, bool onlyPositive, string numberString)
		{
			NumberValidator validator = new NumberValidator(precision, scale,onlyPositive);
			validator.IsValidNumber(numberString).Should().BeFalse();
		}
		
		[Test]
		[TestCase(17, 2, true, "0.0", TestName = "IsValidNumber_ValidNumber_ShouldBeTrue")]
		[TestCase(17, 2, true, "0", TestName = "IsValidNumber_WithoutFractionPart_ShouldBeTrue")]
		[TestCase(4, 2, true, "00.00",
			TestName = "IsValidNumber_NumberPrecisionEqualValidatorPrecision_ShouldBeTrue")]
		
		public void IsValidNumber_ShouldBeTrue(int precision, int scale, bool onlyPositive, string numberString)
		{
			NumberValidator validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(numberString).Should().BeTrue();
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