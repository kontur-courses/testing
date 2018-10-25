using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests_Should
	{
		[TestFixture]
		public class InitNumberValidatorTests
		{
			[Test]
			public void NumberValidator_ThrowsException_WhenCreatingWithNegativePrecision()
			{
				Action action = () => new NumberValidator(-1, 2, true);
				action.Should().Throw<ArgumentException>();
			}

			[Test]
			public void NumberValidator_DoesNotThrowsException_WhenCreatingWithPositivePrecision()
			{
				Action action = () => new NumberValidator(1, 0, true);
				action.Should().NotThrow<ArgumentException>();
			}

			[Test]
			public void NumberValidator_ThrowsException_WhenCreatingWithNegativePrecisionAndOnlyPositiveFlag()
			{
				Action action = () => new NumberValidator(-1, 2, false);
				action.Should().Throw<ArgumentException>();
			}
		}

		[TestFixture]
		public class IsValidNumberTests
		{
			[Test]
			public void IsValidNumber_ReturnsTrue_ForValidNumber()
			{
				var validator = new NumberValidator(17, 2, true);
				validator.IsValidNumber("0.0").Should().BeTrue();
			}

			[Test]
			public void IsValidNumber_ReturnsTrue_ForIntegerLengthShorterPrecision()
			{
				var validator = new NumberValidator(17, 2, true);
				validator.IsValidNumber("0").Should().BeTrue();
			}

			[Test]
			public void IsValidNumber_ReturnsFalse_IfDoubleNumberLengthLongerPrecision()
			{
				var validator = new NumberValidator(3, 2, true);
				validator.IsValidNumber("00.00").Should().BeFalse();
			}

			[Test]
			public void IsValidNumber_ReturnsFalse_IfNumberWithNegativeSignLengthLongerPrecision()
			{
				var validator = new NumberValidator(3, 2, true);
				validator.IsValidNumber("-0.00").Should().BeFalse();
			}

			[Test]
			public void IsValidNumber_ReturnsTrue_ForNumberWithSignLengthEqualPrecision()
			{
				var validator = new NumberValidator(4, 2, true);
				validator.IsValidNumber("+1.23").Should().BeTrue();
			}

			[Test]
			public void IsValidNumber_ReturnsFalse_IfNumberWithPositiveSignLengthLongerPrecision()
			{
				var validator = new NumberValidator(3, 2, true);
				validator.IsValidNumber("+1.23").Should().BeFalse();
			}

			[Test]
			public void IsValidNumber_ReturnsFalse_IfNumberFractionPartLengthLongerScale()
			{
				var validator = new NumberValidator(17, 2, true);
				validator.IsValidNumber("0.000").Should().BeFalse();
			}

			[Test]
			public void IsValidNumber_ReturnsFalse_ForNonNumberArgument()
			{
				var validator = new NumberValidator(3, 2, true);
				validator.IsValidNumber("a.sd").Should().BeFalse();
			}

			[Test]
			public void IsValidNumber_ReturnsFalse_ForEmptyStringArgument()
			{
				var validator = new NumberValidator(3, 2, true);
				validator.IsValidNumber("").Should().BeFalse();
			}

			[Test]
			public void IsValidNumber_ReturnsFalse_ForNullArgument()
			{
				var validator = new NumberValidator(3, 2, true);
				string arg = null;
				validator.IsValidNumber(arg).Should().BeFalse();
			}
		}
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