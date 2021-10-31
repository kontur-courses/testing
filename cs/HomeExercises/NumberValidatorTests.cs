using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1,0,true)]
		public void NumberValidator_CorrectArguments_DoesNotThrowException(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action buildValidator = () => { _ = new NumberValidator(precision, scale, onlyPositive); };
			buildValidator.Should().NotThrow("validator params: precision - {0}, scale - {1}, onlyPositive - {2}", precision, scale, onlyPositive);
		}
		
		[TestCase(0, TestName = "NumberValidator_ZeroPrecision_ShouldThrowException", Description = "Precision must be a positive number")]
		[TestCase(-1, TestName = "NumberValidator_NegativePrecision_ShouldThrowException", Description = "Precision must be a positive number")]
		[TestCase(1, -1, TestName = "NumberValidator_ZeroScale_ShouldThrowException", Description = "Scale must be a non-negative number")]
		[TestCase(1, 1, TestName = "NumberValidator_PrecisionEqualScale_ShouldThrowException", Description = "Scale must be less than precision")]
		[TestCase(1, 2, TestName = "NumberValidator_PrecisionLessThanScale_ShouldThrowException", Description = "Scale must be less than precision")]
		public void NumberValidator_WrongArguments_ShouldThrowException(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action buildValidator = () => { _ = new NumberValidator(precision, scale, onlyPositive); };
			buildValidator.Should().Throw<ArgumentException>("validator params: precision - {0}, scale - {1}, onlyPositive - {2}", precision, scale, onlyPositive);
		}

		[TestCase("0.0", 17, 2, true)]
		[TestCase("0", 17, 2, true)]
		[TestCase("+1.23", 4, 2, true)]
		[TestCase("-1,0", 3, 1, false)]
		[TestCase("0", 1, 0, false)]
		public void IsValidNumber_CorrectCase_ShouldReturnTrue(string value, int precision, int scale = 0, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(value).Should().BeTrue("input value was \"{0}\"; validator params: precision - {1}, scale - {2}, onlyPositive - {3}", value, precision, scale, onlyPositive);
		}
		
		[TestCase("00.00", 3, 2, true, TestName = "IsValidNumber_PrecisionLessThenInput_ShouldReturnFalse")]
		[TestCase("-0.00", 3, 2, true, TestName = "IsValidNumber_PrecisionLessThenInput_ShouldReturnFalse")]
		[TestCase("+0.00", 3, 2, true, TestName = "IsValidNumber_PrecisionLessThenInput_ShouldReturnFalse")]
		[TestCase("+1.23", 3, 2, true, TestName = "IsValidNumber_PrecisionLessThenInput_ShouldReturnFalse")]
		[TestCase("-1.23", 3, 2, true, TestName = "IsValidNumber_PrecisionLessThenInput_ShouldReturnFalse")]
		[TestCase("0.000", 17, 2, true, TestName = "IsValidNumber_ScaleLessThenInput_ShouldReturnFalse")]
		[TestCase("-1,0", 3, 1, true, TestName = "IsValidNumber_OnlyPositiveDoesNotMatchInput_ShouldReturnFalse")]
		[TestCase("a.sd", 3, 2, true, TestName = "IsValidNumber_ValueDoesNotMatchRegex_ShouldReturnFalse")]
		[TestCase(null, 3, 2, true, TestName = "IsValidNumber_ValueDoesNotMatchRegex_ShouldReturnFalse")]
		[TestCase("", 3, 2, true, TestName = "IsValidNumber_ValueDoesNotMatchRegex_ShouldReturnFalse")]
		[TestCase(".0", 3, 2, true, TestName = "IsValidNumber_ValueDoesNotMatchRegex_ShouldReturnFalse")]
		[TestCase("0.", 3, 2, true, TestName = "IsValidNumber_ValueDoesNotMatchRegex_ShouldReturnFalse")]
		public void IsValidNumber_IncorrectCase_ShouldReturnFalse(string value, int precision, int scale = 0, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(value).Should().BeFalse("input value was \"{0}\"; validator params: precision - {1}, scale - {2}, onlyPositive - {3}", value, precision, scale, onlyPositive); 
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