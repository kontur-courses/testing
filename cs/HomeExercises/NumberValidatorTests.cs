using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(0, TestName = "NumberValidator_ZeroPrecision_ShouldThrowException",
			Description = "Precision must be a positive number")]
		[TestCase(-1, TestName = "NumberValidator_NegativePrecision_ShouldThrowException",
			Description = "Precision must be a positive number")]
		[TestCase(1, -1, TestName = "NumberValidator_ZeroScale_ShouldThrowException",
			Description = "Scale must be a non-negative number")]
		[TestCase(1, 1, TestName = "NumberValidator_PrecisionEqualScale_ShouldThrowException",
			Description = "Scale must be less than precision")]
		[TestCase(1, 2, TestName = "NumberValidator_PrecisionLessThanScale_ShouldThrowException",
			Description = "Scale must be less than precision")]
		public void NumberValidator_WrongArguments_ShouldThrowException(int precision, int scale = 0,
			bool onlyPositive = false)
		{
			Action buildValidator = () => { _ = new NumberValidator(precision, scale, onlyPositive); };
			buildValidator.Should()
				.Throw<ArgumentException>("validator params: precision - {0}, scale - {1}, onlyPositive - {2}",
					precision, scale, onlyPositive);
		}

		[TestCase("0.0", 17, 2, true)]
		[TestCase("0", 17, 2, true)]
		[TestCase("+1.23", 4, 2, true)]
		[TestCase("-1,0", 3, 1, false)]
		[TestCase("0", 1, 0, false)]
		public void IsValidNumber_CorrectCase_ShouldReturnTrue(string value, int precision, int scale,
			bool onlyPositive)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(value).Should()
				.BeTrue("input value was \"{0}\"; validator params: precision - {1}, scale - {2}, onlyPositive - {3}",
					value, precision, scale, onlyPositive);
		}

		[TestCase("00.00", 3, 2, true, TestName = "IsValidNumber_PrecisionLessThenInput_ShouldReturnFalse")]
		[TestCase("-0.00", 3, 2, true, TestName = "IsValidNumber_PrecisionLessThenInput_ShouldReturnFalse")]
		[TestCase("+0.00", 3, 2, true, TestName = "IsValidNumber_PrecisionLessThenInput_ShouldReturnFalse")]
		[TestCase("+1.23", 3, 2, true, TestName = "IsValidNumber_PrecisionLessThenInput_ShouldReturnFalse")]
		[TestCase("-1.23", 3, 2, true, TestName = "IsValidNumber_PrecisionLessThenInput_ShouldReturnFalse")]
		[TestCase("0.000", 17, 2, true, TestName = "IsValidNumber_ScaleLessThenInput_ShouldReturnFalse")]
		[TestCase("-1,0", 3, 1, true, TestName = "IsValidNumber_OnlyPositiveDoesNotMatchInput_ShouldReturnFalse")]
		[TestCase("a.sd", 3, 2, true, TestName = "IsValidNumber_ValueDoesNotMatchRegex_ShouldReturnFalse")]
		[TestCase(null, 3, 2, true, TestName = "IsValidNumber_NullValue_ShouldReturnFalse")]
		[TestCase("", 3, 2, true, TestName = "IsValidNumber_EmptyStringValue_ShouldReturnFalse")]
		[TestCase(".0", 3, 2, true, TestName = "IsValidNumber_ValueDoesNotMatchRegex_ShouldReturnFalse")]
		[TestCase("0.", 3, 2, true, TestName = "IsValidNumber_ValueDoesNotMatchRegex_ShouldReturnFalse")]
		public void IsValidNumber_IncorrectCase_ShouldReturnFalse(string value, int precision, int scale,
			bool onlyPositive)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(value).Should()
				.BeFalse("input value was \"{0}\"; validator params: precision - {1}, scale - {2}, onlyPositive - {3}",
					value, precision, scale, onlyPositive);
		}
	}
}