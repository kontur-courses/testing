using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-123, 2, true, TestName = "Scale < 0")]
		public void CreateValidator_ThrowWhenIncorrectInput_Scale(int precision, int scale, bool onlyPositive = true)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.ShouldThrow<ArgumentException>($"scale {scale} isn't possible");
		}

		[TestCase(0, 2, true, TestName = "Precision == 0")]
		[TestCase(-123, 2, true, TestName = "Precision < 0")]
		public void CreateValidator_ThrowWhenIncorrectInput_Precision(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.ShouldThrow<ArgumentException>($"precision {precision} isn't possible");
		}

		[TestCase(2, 2, true, TestName = "Precision == Scale")]
		[TestCase(1, 2, true, TestName = "Precision < Scale")]
		public void CreateValidator_ThrowWhenIncorrectInput_PrecisionAndScaleCompare(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.ShouldThrow<ArgumentException>($"precision {precision} with scale {scale} isn't possible");
		}

		[Category("Correct Input")]
		[TestCase(3, 2, true, TestName = "Typical constructor init")]
		[TestCase(10, 0, true, TestName = "Scale zero in constructor")]
		public void CreateValidator_CorrectInput(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.ShouldNotThrow<ArgumentException>();
		}

		[Category("Correct Input")]
		[TestCase("+1.23", TestName = "With plus symbol and fractional part")]
		[TestCase("-1.23", TestName = "With minus symbol and fractional part")]
		[TestCase("0", TestName = "Int part zero")]
		[TestCase("0.0", TestName = "Int and frac part zero")]
		public void IsValidNumber_CorrectInput(string value)
		{
			new NumberValidator(5, 2, false).IsValidNumber(value).Should().BeTrue();
		}

		[Category("Incorrect parsing")]
        	[TestCase("", TestName = "Empty string")]
		[TestCase(null, TestName = "Null input")]
		[TestCase("ad.s", TestName = "Other symbols input")]
		[TestCase("0..0", TestName = "Double point")]
		[TestCase("0.", TestName = "Empty frac part")]
		[TestCase(".0", TestName = "Empty int part")]
		[TestCase("--0.0", TestName = "Double sign symbol")]
		
		public void IsValidNumber_IncorrectInputString_ParseFalse(string value)
		{
			new NumberValidator(3,2).IsValidNumber(value).Should().BeFalse();
		}

		[Category("Doesn't fit the value rule")]
		[TestCase("000.00", TestName = "Length more than precision")]
		[TestCase("00000", TestName = "Length more than precision without frac parth")]
		[TestCase("-00.00", TestName = "Length with sign more than precision")]
		[TestCase("0.000", TestName = "Too big scale")]
		[TestCase("-12.0", TestName = "Negative value when only positive")]
		[TestCase("-0.0", TestName = "Negative zero when only positive")]
        	public void IsValidNumber_IncorrectInputString_NotMatchForRules(string value)
		{
			new NumberValidator(4, 2, true).IsValidNumber(value).Should().BeFalse();
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
				throw new ArgumentException("scale must be a non-negative number, less than precision");
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
