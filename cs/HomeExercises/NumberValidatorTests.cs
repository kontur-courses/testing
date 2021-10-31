using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private static readonly NumberValidator DefaultNumberValidator = new NumberValidator(100, 99, false);

		[Test]
		public void NumberValidator_ShouldThrowException_IfIncorrectArguments()
		{
			((Action)(() => new NumberValidator(-3, 2, true)))
				.Should()
				.Throw<ArgumentException>()
				.WithMessage("precision must be a positive number");

			((Action)(() => new NumberValidator(1, 2, false)))
				.Should()
				.Throw<ArgumentException>("because scale must be less or equal than precision");

			((Action)(() => new NumberValidator(1, -2, false)))
				.Should()
				.Throw<ArgumentException>("because scale should be non-negative number");
		}

		[Test]
		public void NumberValidator_DoesNotThrow_IfCorrectArguments()
		{
			((Action)(() => new NumberValidator(1, 0, true))).Should().NotThrow();
		}

		[Test]
		public void IsValidNumber_IsTrue_IfCheckZero()
		{
			DefaultNumberValidator.IsValidNumber("0.0").Should().BeTrue();
			DefaultNumberValidator.IsValidNumber("0").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_IsTrue_IfNumberInBounds()
		{
			DefaultNumberValidator.IsValidNumber("1.5")
				.Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_IsFalse_IfFracPartGreaterThanScale()
		{
			var scale = 2;
			var fracPart = "000";

			(new NumberValidator(17, scale, true).IsValidNumber($"0.{fracPart}"))
				.Should().BeFalse("because fracPart greater than scale");
		}

		[Test]
		public void IsValidNumber_IsFalse_IfDigitsNumberGreaterThanPrecision()
		{
			var precision = 3;
			var number = "123.123";

			(new NumberValidator(precision, 2, true).IsValidNumber(number))
				.Should().BeFalse("because count of digits should not be grater than precision");
		}

		[Test]
		public void IsValidNumber_IsFalse_IfSignIsOutOfBounds()
		{
			var numWithoutSign = "12.22";
			var numberValidator = new NumberValidator(4, 2, false);

			numberValidator.IsValidNumber(numWithoutSign)
				.Should().BeTrue();

			numberValidator.IsValidNumber("-" + numWithoutSign)
				.Should().BeFalse("because num with sign length greater than precision");
		}

		[Test]
		public void IsValidNumber_IsFalse_IfValueIsNotNumber()
		{
			DefaultNumberValidator.IsValidNumber("a.sd")
				.Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_IsFalse_IfNumberIsNegativeButShouldBePositive()
		{
			(new NumberValidator(3, 2, false).IsValidNumber("-1.2")).Should().BeTrue();
			(new NumberValidator(3, 2, true).IsValidNumber("-1.2")).Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_IsTrue_IfNumberHasCommaInsteadPoint()
		{
			DefaultNumberValidator.IsValidNumber("1.2").Should().BeTrue();
			DefaultNumberValidator.IsValidNumber("1,2").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_IsFalse_IfNumberIsNullOrEmpty()
		{
			DefaultNumberValidator.IsValidNumber("")
				.Should().BeFalse();
			DefaultNumberValidator.IsValidNumber(null)
				.Should().BeFalse();
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