using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidator_Should
	{
		[Test]
		public void OnNegativePrecision_ThrowsException()
		{
			Action action = () => new NumberValidator(-1, 0, true);
			action.Should().Throw<ArgumentException>();
		}
		
		[Test]
		public void OnNegativeScale_ThrowsException()
		{
			Action act = () => new NumberValidator(17, -2, true);
			act.Should().Throw<ArgumentException>();
		}
		
		[TestCase(2, 2)]
		[TestCase(3, 3)]
		public void OnScaleGreaterOrEqualThanPrecision_ThrowsException(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision, scale);
			act.Should().Throw<ArgumentException>();
		}

		[TestCase(null)]
		[TestCase("")]
		public void OnNullOrEmptyValue_ReturnsFalse(string value)
		{
			var validator = new NumberValidator(17, 2, true);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("0.2")]
		[TestCase("0,3")]
		public void SeparatorShouldNotBeTakenInCount(string value)
		{
			var validator = new NumberValidator(2, 1);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase(1, 0, "-2")]
		[TestCase(5, 4, "-5.1004")]
		[TestCase(1, 0, "+2")]
		[TestCase(3, 2, "+2.12")]
		public void SignSymbolsShouldBeTakenInCount(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale, false);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		public void WhenOnlyPositiveIsTrue_OnNegativeValue_ReturnsFalse()
		{
			var validator = new NumberValidator(17, 2, true);
			validator.IsValidNumber("-2").Should().BeFalse();
		}

		[TestCase(true, "+0")]
		[TestCase(false, "-0")]
		[TestCase(true, "00.00")]
		[TestCase(true, "0.0")]
		[TestCase(true, "1")]
		[TestCase(false, "-1")]
		[TestCase(false, "1.1")]
		[TestCase(false, "-1.1")]
		[TestCase(false, "-123439824.21")]
		public void OnCorrectValue_ReturnsTrue(bool onlyPositive, string value)
		{
			var validator = new NumberValidator(17, 2, onlyPositive);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase(3, false, "10.124534")]
		[TestCase(2, false, "-123.12234")]
		[TestCase(0, false, "0.12424")]
		public void WhenFractDigitsCountGreaterThanScale_ReturnsFalse(int scale, bool onlyPositive, string value)
		{
			var validator = new NumberValidator(17, scale, onlyPositive);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(4, false, "102323.12")]
		[TestCase(4, false, "-2323.132")]
		[TestCase(4, false, "+91.15")]
		public void WhenValueDigitsCountGreaterThanPrecision_ReturnsFalse(int precision, bool onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, 3, onlyPositive);
			validator.IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCase("ad")]
		[TestCase("ad.g")]
		[TestCase("ad.1")]
		[TestCase("1.g")]
		public void WhenValueHasNonDigitsSymbols_ReturnsFalse(string value)
		{
			var validator = new NumberValidator(3, 2, true);
			validator.IsValidNumber(value).Should().BeFalse();
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
				throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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