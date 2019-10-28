using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void Constructor_ThrowArgumentException_WhenPrecisionIsNegative()
		{
			Action act = () => new NumberValidator(-1, 2);
			act.ShouldThrow<ArgumentException>();
		}

		[Test]
		public void Constructor_ThrowArgumentException_WhenPrecisionIsZero()
		{
			Action act = () => new NumberValidator(0, 2);
			act.ShouldThrow<ArgumentException>();
		}

		[Test]
		public void Constructor_DoesNotThrow_WhenPrecisionIsPositive()
		{
			Action act = () => new NumberValidator(1, 0);
			act.ShouldNotThrow();
		}

		[Test]
		public void Constructor_ThrowArgumentException_WhenScaleIsNegative()
		{
			Action act = () => new NumberValidator(1, -1);
			act.ShouldThrow<ArgumentException>();
		}

		[Test]
		public void Constructor_ThrowArgumentException_WhenScaleGreaterThanPrecision()
		{
			Action act = () => new NumberValidator(1, 2);
            act.ShouldThrow<ArgumentException>();
		}
		
		[Test]
		public void Constructor_ThrowArgumentException_WhenScaleEqualsPrecision()
		{
			Action act = () => new NumberValidator(1, 1);
			act.ShouldThrow<ArgumentException>();
		}

		[Test]
		public void Constructor_DoesNotThrow_WhenScaleLessThanPrecisionAndPositive()
		{
			Action act = () => new NumberValidator(2, 1);
			act.ShouldNotThrow();
		}
		
		[Test]
		public void Constructor_DoesNotThrow_WhenPrecisionPositiveAndScaleIsZero()
		{
			Action act = () => new NumberValidator(1, 0);
			act.ShouldNotThrow();
		}

		[Test]
		public void IsValidNumber_ReturnFalse_WhenValueIsNull()
		{
			var numberValidator = new NumberValidator(1);
			numberValidator.IsValidNumber(null).Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_ReturnFalse_WhenValueIsEmpty()
		{
			var numberValidator = new NumberValidator(1);
			numberValidator.IsValidNumber("").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnFalse_WhenValueIsNotRepresentationOfNumber()
		{
			var numberValidator = new NumberValidator(3, 2);
			numberValidator.IsValidNumber("a.sd").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnFalse_WhenIntPartAndFracPartGreaterThanPrecision()
		{
			var numberValidator = new NumberValidator(3, 2);
			numberValidator.IsValidNumber("00.00").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnFalse_WhenSignAndIntPartAndFracPartGreaterThanPrecision()
		{
			var numberValidator = new NumberValidator(3, 2, true);
			numberValidator.IsValidNumber("+1.23").Should().BeFalse();	
		}
		
		[Test]
		public void IsValidNumber_ReturnFalse_WhenFracPartGreaterThanScale()
		{
			var numberValidator = new NumberValidator(17, 1);
			numberValidator.IsValidNumber("0.00").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnFalse_WhenOnlyPositiveAndHaveNegativeSign()
		{
			var numberValidator = new NumberValidator(17, 2, true);
			numberValidator.IsValidNumber("-1.23").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnTrue_WhenValueValidAndHavePositiveSign()
		{
			var numberValidator = new NumberValidator(17, 2);
			numberValidator.IsValidNumber("+1.23").Should().BeTrue();
		}
		
		[Test]
		public void IsValidNumber_ReturnTrue_WhenOnlyPositiveAndValueValidAndHavePositiveSign()
		{
			var numberValidator = new NumberValidator(17, 2, true);
			numberValidator.IsValidNumber("+1.23").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnTrue_WhenValueValidWithoutFracPart()
		{
			var numberValidator = new NumberValidator(17);
			numberValidator.IsValidNumber("0").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnTrue_WhenValueValidWithCommaDelimiter()
		{
			var numberValidator = new NumberValidator(17, 2);
			numberValidator.IsValidNumber("98765432,1").Should().BeTrue();
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
				throw new ArgumentException("scale must be a non-negative number less than precision");
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