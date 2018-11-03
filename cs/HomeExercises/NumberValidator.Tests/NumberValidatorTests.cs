using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.NumberValidator_Tests
{
    [TestFixture]
    public class NumberValidatorTests
    {
        [TestCase(-1, TestName = "ShouldFallOn_NegativePrecision")]
		[TestCase(0, TestName = "ShouldFallOn_ZeroPrecision")]
        public void Constructor_Precision_Tests(int precision, int scale=0)
        {
            Action act = () => new NumberValidator(-1);

            act.ShouldThrow<ArgumentException>()
                .WithMessage("precision must be a positive number");
        }

        [TestCase(1, -1, TestName = "ShouldFallOn_NegativeScaleValue")]
		[TestCase(1, 2, TestName = "ShouldFallOn_PrecisionLessThanScale")]
        public void Constructor_Scale_Tests(int precision, int scale)
        {
            Action act = () => new NumberValidator(precision, scale);

            act.ShouldThrow<ArgumentException>();
        }

	    [TestCase(5, 2, null, TestName = "ShouldBeFalseOn_NullValue")]
	    [TestCase(5, 2, "", TestName = "ShouldBeFalseOn_EmptyValue")]
	    [TestCase(5, 2, "a.bc", TestName = "ShouldBeFalseOn_NotANumber")]
	    [TestCase(10, 1, "-12", true, TestName = "ShouldBeFalseOn_OnlyPositiveValidator_WithNegativeValue")]
        public void IsValidNumber_IncorrectInput(int precision, int scale, string value, bool onlyPositive = false)
	    {
		    new NumberValidator(precision, scale, onlyPositive)
			    .IsValidNumber(value).Should().BeFalse();
        }

	    [TestCase(5, 2, "1.23", ExpectedResult = true, TestName = "TrueOn_PositiveDoubleWithoutSign")]
	    [TestCase(5, 2, "+1.23", ExpectedResult = true, TestName = "TrueOn_PositiveDoubleWithSign")]
	    [TestCase(5, 2, "-1.23", ExpectedResult = true, TestName = "TrueOn_NegativeDouble")]
	    [TestCase(5, 0, "123456", ExpectedResult = false, TestName = "FalseWhen_ExcessPrecision")]
        public bool IsValidNumber_TestPrecision(int precision, int scale, string value)
	    {
		    return new NumberValidator(precision, scale).IsValidNumber(value);

	    }

	    [TestCase(2, 0, "12", ExpectedResult = true, TestName = "TrueOn_PositiveValueWithZeroScale")]
	    [TestCase(2, 0, "-1", ExpectedResult = true, TestName = "TrueOn_NegativeValueWithZeroScale")]
	    [TestCase(2, 1, ".12", ExpectedResult = false, TestName = "FalseOn_ExcessScaleByPositiveValue")]
	    [TestCase(2, 0, "-.1", ExpectedResult = false, TestName = "FalseOn_ExcessScaleByNegativeValue")]
        public bool IsValidNumber_TestScale(int precision, int scale, string value)
	    {
		    return new NumberValidator(precision, scale).IsValidNumber(value);
	    }
    }
}
