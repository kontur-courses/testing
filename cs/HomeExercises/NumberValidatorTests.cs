using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		[TestCase(-1)]
		[TestCase(0)]
		public void Constructor_NotPositivePrecious_ShouldThrow(int precision)
		{
			new Action(() =>
				{
					var numberValidator = new NumberValidator(precision);
				})
				.Should().Throw<ArgumentException>();
		}

		[Test]
		[TestCase(1, TestName = "PositivePrecious_NoThrow")]
		public void Constructor_PositivePrecious_NoThrow(int precision)
		{
			new Action(() =>
				{
					var numberValidator = new NumberValidator(precision);
				})
				.Should().NotThrow();
		}

		[Test]
		[TestCase(1, -1, TestName = "NegativeScale_Throw")]
		[TestCase(1, 5, TestName = "ScaleGreaterThenPrecious_Throw")]
		[TestCase(5, 5, TestName = "ScaleEqualPrecious_Throw")]
		public void Constructor_NotValidScale_Throw(int precision, int scale)
		{
			new Action(() =>
				{
					var numberValidator = new NumberValidator(precision,scale);
				})
				.Should().Throw<ArgumentException>();
		}

		[Test]
		[TestCase("")]
		[TestCase(null)]
		public void IsValidNumber_EmptyOrNullString_BeFalse(string value)
		{
			var validator = new NumberValidator(5);
			validator.IsValidNumber(value).Should().BeFalse();
		}


		[Test]
		[TestCase("-1.0")]
		public void ValidOnlyPositiveNum_NegativeNum_BeFalse(string value)
		{
			var validator = new NumberValidator(17, 2, true);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		[TestCase(5, 1, "1.12")]
		[TestCase(5, 2, "1.123")]
		[TestCase(5, 3, "1.1234")]
		public void IsValidNumber_FracPartGreaterScale_BeFalse(int precious, int scale, string value)
		{
			var validator = new NumberValidator(precious, scale);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		[TestCase(5, 1, "0.1")]
		[TestCase(5, 2, "0.12")]
		[TestCase(5, 4, "0.123")]
		public void IsValidNumber_FracPartLessOrEqualsScale_BeTrue(int precious, int scale, string value)
		{
			var validator = new NumberValidator(precious, scale);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[Test]
		[TestCase(5, "1223123123.1")]
		[TestCase(2, "1223.1")]
		public void ValidNumber_GreaterPreciousPart_BeFalse(int precious, string value)
		{
			var validator = new NumberValidator(precious, 1);
			validator.IsValidNumber(value).Should().BeFalse();
		}


		[Test]
		[TestCase(5, 2, "-123.1")]
		[TestCase(5, 2, "+123.1")]
		[TestCase(5, 2, "122.1")]
		public void ValidNumber_SignedNum_BeTrue(int precious, int scale, string value)
		{
			var validator = new NumberValidator(precious, 1);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[Test]
		[TestCase(7, 4, "abc.def")]
		[TestCase(7, 4, "-abc.1w3")]
		[TestCase(7, 4, "a1c.123")]
		[TestCase(7, 4, "123.bef")]
		[TestCase(7, 4, "123.b4f")]
		[TestCase(7, 4, "@!.123")]
		[TestCase(7, 4, "123.$#%")]
		[TestCase(7, 4, "-.123")]
		[TestCase(7, 4, "-+.123")]
		public void ValidNumber_LetterOrSignInNum_BeFalse(int precious, int scale, string value)
		{
			var validator = new NumberValidator(precious, scale);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		[TestCase(5, "123")]
		public void ValidNumber_ZeroScale_BeTrue(int precious, string value)
		{
			var validator = new NumberValidator(precious);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[Test]
		[TestCase(5, 2, "12.25")]
		[TestCase(5, 2, "12,25")]
		public void ValidNumber_DotOrComma_BeTrue(int precious, int scale, string value)
		{
			var validator = new NumberValidator(precious, scale);
			validator.IsValidNumber(value).Should().BeTrue();
		}
		
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