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
		public void Creation_NegativePrecision_ShouldThrowException()
		{
			Action creation = () => new NumberValidator(-1, 2, true);
			creation.Should().Throw<ArgumentException>();
		}

		[Test]
		public void Creation_PositivePrecision_ShouldNotThrowException()
		{
			Action creation = () => new NumberValidator(1, 0, true);
			creation.Should().NotThrow<ArgumentException>();
		}

		[Test]
		public void Creation_PrecisionIsZero_ShouldThrowException()
		{
			Action creation = () => new NumberValidator(0, 0, true);
			creation.Should().Throw<ArgumentException>();
		}

		[Test]
		public void Creation_NegativeScale_ShouldThrowException()
		{
			Action creation = () => new NumberValidator(2, -1, true);
			creation.Should().Throw<ArgumentException>();
		}

		[Test]
		public void Creation_ScaleGreaterThenPrecision_ShouldThrowException()
		{
			Action creation = () => new NumberValidator(1, 2, true);
			creation.Should().Throw<ArgumentException>();
		}

		[Test]
		public void IsValidNumber_NullString_ShouldBeFalse()
		{
			NumberValidator validator = new NumberValidator(17, 2,true);
			validator.IsValidNumber(null).Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_EmptyString_ShouldBeFalse()
		{
			NumberValidator validator = new NumberValidator(17, 2,true);
			validator.IsValidNumber(String.Empty).Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_NotNumber_ShouldBeFalse()
		{
			NumberValidator validator = new NumberValidator(3, 2,true);
			validator.IsValidNumber("a.sd").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_WithoutFractionPart_ShouldBeTrue()
		{
			NumberValidator validator = new NumberValidator(17, 2,true);
			validator.IsValidNumber("0").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_WithoutIntPart_ShouldBeFalse()
		{
			NumberValidator validator = new NumberValidator(17, 2,true);
			validator.IsValidNumber(".0").Should().BeFalse();
		}

		[Test]
		[TestCase("00.00")]
		[TestCase("+1.23")]
		public void IsValidNumber_NumberPrecisionGreaterThanValidatorPrecision_ShouldBeFalse(string number)
		{
			NumberValidator validator = new NumberValidator(3, 2,true);
			validator.IsValidNumber(number).Should().BeFalse();
		}
		
		[Test]
		[TestCase("00.00")]
		[TestCase("+1.23")]
		public void IsValidNumber_NumberPrecisionEqualValidatorPrecision_ShouldBeTrue(string number)
		{
			NumberValidator numberValidator = new NumberValidator(4, 2, true);
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}
		
		[Test]
		public void IsValidNumber_NumberScaleGreaterThanValidatorScale_ShouldBeFalse()
		{
			NumberValidator validator = new NumberValidator(17, 2,true);
			validator.IsValidNumber("0.000").Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_NegativeNumberWithOnlyPositiveValidator_ShouldBeFalse()
		{
			NumberValidator numberValidator = new NumberValidator(10, 5, true);
			numberValidator.IsValidNumber("-1.23").Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_ValidNumber_ShouldBeTrue()
		{
			NumberValidator numberValidator = new NumberValidator(17, 2, true);
			numberValidator.IsValidNumber("0.0").Should().BeTrue();
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