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
		[TestCase(-1, 2, true, TestName = "NegativePrecision")]
		[TestCase(0, 0, true, TestName = "PrecisionIsZero")]
		[TestCase(2, -1, true, TestName = "NegativeScale")]
		[TestCase(1, 2, true, TestName = "ScaleGreaterThenPrecision")]
		public void Creation_ShouldThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			Action creation = () => new NumberValidator(precision, scale, onlyPositive);
			creation.Should().Throw<ArgumentException>();
		}

		[Test]
		[TestCase(1, 0, true, TestName = "PositivePrecision")]
		public void Creation_ShouldNotThrowException(int precision, int scale, bool onlyPositive)
		{
			Action creation = () => new NumberValidator(precision, scale, onlyPositive);
			creation.Should().NotThrow<ArgumentException>();
		}

		[Test]
		[TestCase(17, 2, true, null, TestName = "NullString")]
		[TestCase(17, 2, true, "", TestName = "EmptyString")]
		[TestCase(3, 2, true, "a.sd", TestName = "NotNumber")]
		[TestCase(17, 2, true, ".0", TestName = "WithoutIntPart")]
		[TestCase(3, 2, true, "00.00", TestName = "NumberPrecisionGreaterThanValidatorPrecision")]
		[TestCase(3, 2, true, "+1.23", TestName = "NumberPrecisionGreaterThanValidatorPrecision")]
		[TestCase(17, 2, true, "0.000", TestName = "NumberScaleGreaterThanValidatorScale")]
		[TestCase(10, 5, true, "-1.23", TestName = "NegativeNumberWithOnlyPositiveValidator")]
		public void IsValidNumber_ShouldBeFalse(int precision, int scale, bool onlyPositive, string numberString)
		{
			NumberValidator validator = new NumberValidator(precision, scale,onlyPositive);
			validator.IsValidNumber(numberString).Should().BeFalse();
		}
		
		[Test]
		[TestCase(17, 2, true, "0.0", TestName = "ValidNumber_")]
		[TestCase(17, 2, true, "0", TestName = "WithoutFractionPart")]
		[TestCase(4, 2, true, "00.00", TestName = "NumberPrecisionEqualValidatorPrecision")]
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