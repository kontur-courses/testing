using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(0)]
		[TestCase(-1)]
		public void Constructor_ThrowsArgumentException_OnNonPositivePrecision(int precision)
		{
			Action act = () => new NumberValidator(precision);

			act.Should().Throw<ArgumentException>().WithMessage("precision must be a positive number");
		}

		[TestCase(2, TestName = "Scale is greater than precision")]
		[TestCase(-1, TestName = "Negative scale")]
		[TestCase(1, TestName = "Scale is equal a precision")]
		public void Constructor_ThrowsArgumentException_OnWrongScale(int scale)
		{
			Action act = () => new NumberValidator(1, scale, true);

			act.Should().Throw<ArgumentException>()
				.WithMessage("scale must be a non-negative number less than precision");
		}

		[TestCase(1, 0)]
		[TestCase(3, 2, false)]
		[TestCase(3, 2, true)]
		public void Constructor_DoesNotThrowArgumentException_OnCorrectParameters(int precision, int scale,
			bool onlyPositive = false)
		{
			Action act = () => new NumberValidator(2, 1, true);

			act.Should().NotThrow<ArgumentException>();
		}

		[TestCase("")]
		[TestCase(null)]
		public void IsValidNumber_ReturnFalse_OnNullOrEmptyValue(string value)
		{
			var sut = new NumberValidator(2);
			sut.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("abc")]
		[TestCase("!@#")]
		[TestCase("1.")]
		[TestCase("-1.")]
		[TestCase("1.0!")]
		[TestCase("+1,")]
		[TestCase("++1")]
		[TestCase("1..0")]
		public void IsValidNumber_ReturnFalse_OnWrongValueFormat(string value)
		{
			var sut = new NumberValidator(2, 1);
			sut.IsValidNumber(value).Should().BeFalse();
		}


		[TestCase("12.34")]
		[TestCase("+12.3")]
		[TestCase("-1234")]
		public void IsValidNumber_ReturnFalse_WhenValuePrecisionMoreThanNumberValidatorPrecision(string value)
		{
			var sut = new NumberValidator(3, 2);
			sut.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("12.34")]
		[TestCase("+1.23")]
		public void IsValidNumber_ReturnFalse_WhenValueScaleMoreThanNumberValidatorScale(string value)
		{
			var sut = new NumberValidator(3, 1);
			sut.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("1.23", true)]
		[TestCase("+1.2", true)]
		[TestCase("1.23", false)]
		[TestCase("-1.2", false)]
		public void IsValidNumber_ReturnTrue_OnCorrectValue(string value, bool onlyPositive)
		{
			var sut = new NumberValidator(3, 2, onlyPositive);
			sut.IsValidNumber(value).Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnFalse_ForNegativeNumber_WhenOnlyPositive()
		{
			var sut = new NumberValidator(2, 1, true);
			sut.IsValidNumber("-1.0").Should().BeFalse();
		}
	}

	public class NumberValidator
	{
		private static readonly Regex NumberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
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
				throw new ArgumentException("scale must be a non-negative number less than precision");
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

			var match = NumberRegex.Match(value);
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