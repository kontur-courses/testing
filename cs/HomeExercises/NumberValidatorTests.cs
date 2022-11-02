using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1, 0, true, TestName = "Correct values does not throw exception")]
		public void NumberValidator_CorrectValues_DoesNotThrowException(int precision, int scale, bool onlyPositive)
		{
			Action createNumberValidator = () => new NumberValidator(precision, scale, onlyPositive);
			createNumberValidator.Should().NotThrow<ArgumentException>();
		}

		[TestCase(-1, 2, TestName = "Negative precision should throw exception")]
		[TestCase(0, 2, TestName = "Zero precision should throw exception")]
		[TestCase(3, 4, TestName = "Scale bigger then precision should throw exception")]
		[TestCase(3, 3, TestName = "Scale equal precision should throw exception")]
		[TestCase(3, -1, TestName = "Scale is negative should throw exception")]
		public void NumberValidator_InvalidValues_ThrowException(int precision, int scale)
		{
			Action createNumberValidator = () => new NumberValidator(precision, scale);
			createNumberValidator.Should().Throw<ArgumentException>();
		}
		
		[TestCase(17, 2, true, "0.0", TestName = "Correct values should be true")]
		public void IsValidNumber_CorrectValue_True(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(value)
				.Should()
				.BeTrue();
		}
		
		[TestCase(17, 2, true, "", TestName = "Empty value must be false")]
		[TestCase(17, 2, true, null, TestName = "Null value must be false")]
		[TestCase(3, 2, true, "a.sd", TestName = "Does not value must be false")]
		[TestCase(3, 2, true, "1.1231", TestName = "Frac part bigger then precision must be false")]
		[TestCase(3, 2, true, "1111.11", TestName = "Sum of parts bigger then precision must be false")]
		[TestCase(3, 2, true, "+1111.1", TestName = "Sum of parts bigger then precision must be false")]
		[TestCase(3, 2, true, "-11.1", TestName = "Negative value with only positive flag must be false")]
		public void IsInvalidNumber_IncorrectValues_False(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(value)
				.Should()
				.BeFalse();
		}
	}
}
