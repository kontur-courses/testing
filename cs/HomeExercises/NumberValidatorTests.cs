using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1)]
		[TestCase(0)]
		public void Constructor_NotPositivePrecision_ShouldThrow(int precision)
		{
			new Action(() => new NumberValidator(precision))
				.Should().Throw<ArgumentException>();
		}

		[Test]
		public void Constructor_PositivePrecision_NoThrow()
		{
			new Action(() => new NumberValidator(2))
				.Should().NotThrow();
		}

		[TestCase(1, -1, TestName = "NegativeScale")]
		[TestCase(1, 5, TestName = "ScaleGreaterThenPrecision")]
		[TestCase(5, 5, TestName = "ScaleEqualPrecision")]
		public void Constructor_NotValidScale_Throw(int precision, int scale)
		{
			new Action(() => new NumberValidator(precision, scale))
				.Should().Throw<ArgumentException>();
		}

		[TestCase("")]
		[TestCase(null)]
		public void IsValidNumber_EmptyOrNullString_BeFalse(string value)
		{
			var validator = new NumberValidator(5);
			validator.IsValidNumber(value).Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_NegativeNumInOnlyPositiveNumberValidator_BeFalse()
		{
			var validator = new NumberValidator(5, 2, true);
			validator.IsValidNumber("-1.0").Should().BeFalse();
		}
		
		[TestCase(5, 1, "1.12")]
		[TestCase(5, 2, "1.123")]
		[TestCase(5, 3, "1.1234")]
		public void IsValidNumber_FracPartGreaterScale_BeFalse(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale);
			validator.IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCase(5, 1, "0.1")]
		[TestCase(5, 2, "0.12")]
		[TestCase(5, 4, "0.123")]
		public void IsValidNumber_FracPartLessOrEqualsScale_BeTrue(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale);
			validator.IsValidNumber(value).Should().BeTrue();
		}
		
		[TestCase(5,1, "1223123123.1")]
		[TestCase(4,2, "1234.56")]
		[TestCase(4,2, "-123.1")]
		[TestCase(5,2,"+123.45")]
		public void IsValidNumber_IntAndFracPartsGreaterThenPrecision_BeFalse(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		
		[TestCase(5, 2, "-123.1")]
		[TestCase(5, 2, "+123.1")]
		public void IsValidNumber_SignedNum_BeTrue(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale);
			validator.IsValidNumber(value).Should().BeTrue();
		}
		
		[TestCase(7, 4, "abc.def")]
		[TestCase(7, 4, "-abc.1w3")]
		[TestCase(7, 4, "a1c.123")]
		[TestCase(7, 4, "123.bef")]
		[TestCase(7, 4, "123.b4f")]
		[TestCase(7, 4, "@!.123")]
		[TestCase(7, 4, "123.$#%")]
		[TestCase(7, 4, "-.123")]
		[TestCase(7, 4, "-+.123")]
		public void IsValidNumber_LetterOrSignInNum_BeFalse(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale);
			validator.IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCase(5, "123")]
		[TestCase(10, "1234567890")]
		public void IsValidNumber_ZeroScale_BeTrue(int precision, string value)
		{
			var validator = new NumberValidator(precision);
			validator.IsValidNumber(value).Should().BeTrue();
		}
		
		[TestCase(5, 2, "12.25")]
		[TestCase(5, 2, "12,25")]
		public void IsValidNumber_DotOrComma_BeTrue(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale);
			validator.IsValidNumber(value).Should().BeTrue();
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