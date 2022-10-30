using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidator_ThrowsException_OnPrecisionLessThanOne()
		{
			Action action = () => new NumberValidator(-1, 2, true);
			Action action2 = () => new NumberValidator(0, 2, true);

			action
				.Should()
				.Throw<ArgumentException>()
				.WithMessage("precision must be a positive number");
			
			action2
				.Should()
				.Throw<ArgumentException>()
				.WithMessage("precision must be a positive number");
		}
		
		
		[Test]
		public void NumberValidator_ThrowsException_OnPrecisionLessThanScale()
		{
			Action action = () => new NumberValidator(23, 24, true);

			action
				.Should()
				.Throw<ArgumentException>()
				.WithMessage("precision must be a non-negative number less or equal than precision");
		}
		
		
		[Test]
		public void NumberValidator_ThrowsException_OnPrecisionEqualsScale()
		{
			Action action = () => new NumberValidator(24, 24, true);

			action
				.Should()
				.Throw<ArgumentException>()
				.WithMessage("precision must be a non-negative number less or equal than precision");
		}


		[Test]
		public void IsValidNumber_GivesFalse_OnCharacters()
		{
			var v = new NumberValidator(3, 2, true);
			var result = v.IsValidNumber("a.sd"); 
			result.Should().Be(false);
		}
		
		
		[Test]
		public void IsValidNumber_GivesFalse_WhenScaleTooBig()
		{
			var v = new NumberValidator(17, 2, true);
			var result = v.IsValidNumber("0.000"); 
			result.Should().Be(false);
		}
		
		
		[Test]
		public void IsValidNumber_GivesFalse_OnMinusWhenOnlyPositive()
		{
			var v = new NumberValidator(17, 5, true);
			var result = v.IsValidNumber("-0.000"); 
			result.Should().Be(false);
		}
		
		
		[Test]
		public void IsValidNumber_GivesFalse_WhenPrecisionTooBig()
		{
			var v = new NumberValidator(3, 2, true);
			var result = v.IsValidNumber("00.00"); 
			result.Should().Be(false);
		}
		
		
		[Test]
		public void IsValidNumber_GivesTrue_WhenScaleIsZero()
		{
			var v = new NumberValidator(3, 2, true);
			var result = v.IsValidNumber("0"); 
			result.Should().Be(true);
		}
		
		
		[Test]
		public void IsValidNumber_GivesFalse_WhenSignedNumberIsTooLong()
		{
			var v = new NumberValidator(3, 2, true);
			var result = v.IsValidNumber("+0.00"); 
			result.Should().Be(false);
		}
		
		
		[Test]
		public void IsValidNumber_GivesFalse_OnEmptyString()
		{
			var v = new NumberValidator(3, 2, true);
			var result = v.IsValidNumber(""); 
			result.Should().Be(false);
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