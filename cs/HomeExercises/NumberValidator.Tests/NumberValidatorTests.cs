using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.NumberValidator_Tests
{
    [TestFixture]
    public class NumberValidatorTests
    {
        [TestCase(-1, TestName = "Constructor_ShouldFallOn_NegativePrecision")]
		[TestCase(0, TestName = "Constructor_ShouldFallOn_ZeroPrecision")]
        public void Constructor_Precision_Tests(int precision, int scale=0)
        {
            Action act = () => new NumberValidator(-1);

            act.ShouldThrow<ArgumentException>()
                .WithMessage("precision must be a positive number");
        }

        [Test]
        public void Constructor_ShouldNotFallOn_PositivePrecision()
        {
            Action act = () => new NumberValidator(1);

            act.ShouldNotThrow();
        }

        [TestCase(1, -1, TestName = "Constructor_ShouldFallOn_NegativeScaleValue")]
		[TestCase(1, 2, TestName = "Constructor_ShouldFallOn_PrecisionLessThanScale")]
        public void Constructor_Scale_Tests(int precision, int scale)
        {
            Action act = () => new NumberValidator(precision, scale);

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Constructor_ShouldNotFallOn_PrecisionEqualToScale()
        {
            Action act = () => new NumberValidator(2, 2);

            act.ShouldNotThrow();
        }

        [TestCase(5, 2, "", TestName = "IsValidNumber_ShouldBeFalseOn_NullOrEmptyValue")]
		[TestCase(5, 2, "a.sd", TestName = "IsValidNumber_ShouldBeFalseOn_NotANumberValue")]
        [TestCase(10, 1, "-12", true, TestName = "IsValidNumber_ShouldBeFalseOn_OnlyPositiveValidator_WithNegativeValue")]
		[TestCase(5, 0, "123456", TestName = "IsValidNumber_ShouldBeFalseOn_ExcessPrecision")]
        [TestCase(2,1, ".123", TestName = "IsValidNumber_ShouldBeFalseOn_ExcessScale")]
        public void IsValidNumber_TestsToBeFalse(int precision, int scale, string value, bool onlyPositive = false)
        {
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
            numberValidator.IsValidNumber(value)
                .Should().BeFalse();
        }

        [TestCase(5, 2, "0.0", TestName = "IsValidNumber_ShouldBeTrueOn_PositiveDoubleValue")]
		[TestCase(5, 2, "-1.23", TestName = "IsValidNumber_ShouldBeTrueOn_NegativeDoubleValue")]
		[TestCase(2, 0, "12", TestName = "IsValidNumber_ShouldBeTrueOn_PositiveValueWithZeroScale")]
        public void IsValidNumber_TestsToBeTrue(int precision, int scale, string value)
        {
	        var numberValidator = new NumberValidator(precision, scale);
            numberValidator.IsValidNumber(value)
                .Should().BeTrue();
        }
    }
}
