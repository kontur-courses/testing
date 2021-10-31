using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidator_Throws_WhenPrecisionIsNotPositive()
		{
			Action act = () => new NumberValidator(-1, 2, true);

			act.Should().Throw<ArgumentException>();
		}
		
		[Test]
		public void NumberValidator_DoesntThrow_WhenPrecisionIsPositive()
		{
			Action act = () => new NumberValidator(1, 0, true);
			
			act.Should().NotThrow<ArgumentException>();
		}
		
		[Test]
		public void NumberValidator_Throws_WhenScaleIsMoreThanPrecision()
		{
			Action act = () => new NumberValidator(-1, 2);
			
			act.Should().Throw<ArgumentException>();
		}
		
		[Test]
		public void NumberValidator_DoesntThrow_WhenScaleIsLessThanPrecision()
		{
			Action act = () => new NumberValidator(1, 0, true);
			
			act.Should().NotThrow<ArgumentException>();
		}
		
		[Test]
		public void IsValidNumber_ReturnsFalse_WhenStringIsNull()
		{
			var validator = new NumberValidator(17, 2, true);
			
			TestIsValidNumber(validator, null!, false);
		}
		
		[Test]
		public void IsValidNumber_ReturnsFalse_WhenStringIsEmty()
		{
			var validator = new NumberValidator(17, 2, true);
			
			TestIsValidNumber(validator, "", false);
		}
		
		[Test]
		public void IsValidNumber_ReturnsFalse_WhenStringIsInvalid()
		{
			var validator = new NumberValidator(17, 2, true);
			
			TestIsValidNumber(validator, "a.sd", false);
		}
		
		[Test]
		public void IsValidNumber_ReturnsFalse_WhenNumberIsLongerThanPrecision()
		{
			var validator = new NumberValidator(3, 2, true);
			
			TestIsValidNumber(validator, "000.000", false);
		}
		
		[Test]
		public void IsValidNumber_ReturnsFalse_WhenFractionalPartIsLongerThanScale()
		{
			var validator = new NumberValidator(17, 2, true);
			
			TestIsValidNumber(validator, "000.000", false);
		}
		
		[Test]
		public void IsValidNumber_ReturnsFalse_WhenNumberHasMinusAndValidatorIsOnlyPositive()
		{
			var validator = new NumberValidator(17, 2, true);
			
			TestIsValidNumber(validator, "-1.0", false);
		}
		
		[Test]
		public void IsValidNumber_ReturnsTrue_WhenStringIsCorrectNumberWithDot()
		{
			var validator = new NumberValidator(17, 2, true);
			
			TestIsValidNumber(validator, "1.0", true);
		}
		
		[Test]
		public void IsValidNumber_ReturnsTrue_WhenStringIsCorrectNumberWithoutFractionalPart()
		{
			var validator = new NumberValidator(17, 2, true);
			
			TestIsValidNumber(validator, "1", true);
		}
		
		[Test]
		public void IsValidNumber_ReturnsTrue_WhenStringIsCorrectNumberWithComma()
		{
			var validator = new NumberValidator(17, 2, true);
			
			TestIsValidNumber(validator, "1,0", true);
		}
		
		[Test]
		public void IsValidNumber_ReturnsTrue_WhenPositiveNumberHasPlusInTheBeginning()
		{
			var validator = new NumberValidator(17, 2, true);
			
			TestIsValidNumber(validator, "+1,0", true);
		}
		
		[Test]
		public void IsValidNumber_ReturnsTrue_WhenNegativeNumberHasMinusInTheBeginning()
		{
			var validator = new NumberValidator(17, 2);
			
			TestIsValidNumber(validator, "-1,0", true);
		}

		private static void TestIsValidNumber(NumberValidator validator, string numberToCheck, bool expectedResult)
		{
			validator.IsValidNumber(numberToCheck).Should().Be(expectedResult);
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