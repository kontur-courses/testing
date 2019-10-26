using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test, Category("ConstructorTests")]
		public void Constructor_Smokie()
		{
			Action action = () => new NumberValidator(2, 1, true);
			action.ShouldNotThrow<ArgumentException>();
		}
		[Test, Category("ConstructorTests")]
		public void Constructor_ShouldThrowArgumentException_OnNegativeOrZeroPrecision()
		{
			Action action1 = () => new NumberValidator(-1, 2, true);
			action1.ShouldThrow<ArgumentException>();
			Action action2 = () => new NumberValidator(0, 2, true);
			action2.ShouldThrow<ArgumentException>();
		}
		
		[Test, Category("ConstructorTests")]
		public void Constructor_ShouldThrowArgumentException_OnNegativeScale()
		{
			Action action = () => new NumberValidator(1, -1, true);
			action.ShouldThrow<ArgumentException>();
		}

		[Test, Category("ConstructorTests")]
		public void Constructor_ShouldThrowArgumentException_OnPrecisionBiggerThanScale()
		{
			Action action = () => new NumberValidator(1, 2, true);
			action.ShouldThrow<ArgumentException>();
		}

		[Test, Category("IsValidNumberTests")]
		public void IsValidNumber_ReturnsFalse_OnNotNumberArgument()
		{
			var nv = new NumberValidator(3, 2, true);
			nv.IsValidNumber("a.sd").Should().Be(false);
		}

		[Test, Category("IsValidNumberTests")]
		public void IsValidNumber_ReturnsFalse_OnOnlyPositiveAndNegativeArgument()
		{
			var nv = new NumberValidator(3, 2, true);
            nv.IsValidNumber("-1.23").Should().Be(false);
		}

		[Test, Category("IsValidNumberTests")]
		public void IsValidNumber_SameResult_OnPositiveWithPlusOrNot()
		{
			var nv = new NumberValidator(17, 10, true);
            nv.IsValidNumber("+1.23").Should().Be(nv.IsValidNumber("1.23"));
		}

		[Test, Category("IsValidNumberTests")]
		public void IsValidNumber_ReturnsFalse_OnNumberBiggerThanPrecision()
		{
			var nv = new NumberValidator(3, 2, true);
            nv.IsValidNumber("00.00").Should().Be(false);
		}
		
		[Test, Category("IsValidNumberTests")]
		public void IsValidNumber_ReturnsFalse_OnNumberFractionalPartBiggerThanScale()
		{
			var nv = new NumberValidator(17, 2, true);
			nv.IsValidNumber("0.000").Should().Be(false);
		}
		
		[Test, Category("IsValidNumberTests")]
		public void IsValidNumber_ReturnsFalse_OnNumberWithSignBiggerThanPrecision()
		{
			var nv = new NumberValidator(3, 2, true);
			nv.IsValidNumber("+0.00").Should().Be(false);
		}
		
		[Test, Category("IsValidNumberTests")]
		public void IsValidNumber_ReturnsTrue_OnValidNumberWithFractionalPart()
		{
			var nv = new NumberValidator(4, 2, true);
			nv.IsValidNumber("+0.00").Should().Be(true);
		}

		[Test, Category("IsValidNumberTests")]
		public void IsValidNumber_ReturnsTrue_OnValidNumberWithoutFractionalPart()
		{
			var nv = new NumberValidator(4, 2, true);
			nv.IsValidNumber("0").Should().Be(true);
		}
		
		[Test, Category("IsValidNumberTests")]
		public void IsValidNumber_WorksWithPointAndComma()
		{
			var nv = new NumberValidator(4, 2, true);
			nv.IsValidNumber("0.01").Should().Be(nv.IsValidNumber("0,01"));
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