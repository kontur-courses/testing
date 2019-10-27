using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.NumberValidatorTests
{
    class NumberValidator_InitializationTests
    {
        [TestCase(1, 0, true)]
        [TestCase(int.MaxValue, 0, true)]
        [TestCase(int.MaxValue, int.MaxValue - 1, false)]
        public void ShouldNotThow_WithCorrectArguments(int precision, int scale, bool onlyPositive)
        {
            Action act = () => new NumberValidator(precision, scale, onlyPositive);

            act.Should().NotThrow();
        }

        [TestCase(2, 1)]
        public void ShouldNotThrow_WithTwoCorrectArguments(int precision, int scale)
        {
            Action act = () => new NumberValidator(precision, scale);

            act.Should().NotThrow();
        }

        [TestCase(2)]
        public void ShouldNotThrow_WithOnlyOneCorrectArgument(int precision)
        {
            Action act = () => new NumberValidator(precision);

            act.Should().NotThrow();
        }

        [TestCase(
            -1, 2, true,
            TestName = "ShouldThrow_WithNegativePrecision")]
        [TestCase(
            0, 0, false,
            TestName = "ShouldThrow_WithZeroPrecision")]
        [TestCase(
            10, -1, true,
            TestName = "ShouldThrow_WithNegativeScale")]
        [TestCase(
            10, 10, true,
            TestName = "ShouldThrow_WhenScaleEqualsPrecision")]
        [TestCase(
            10, 15, true,
            TestName = "ShouldThrow_WhenScaleMoreThanPrecision")]
        public void ShouldThow_WithIncorrectArguments(int precision, int scale, bool onlyPositive)
        {
            Action act = () => new NumberValidator(precision, scale, onlyPositive);

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void ValidatorObjectsShouldBeIndependOfEachOther()
        {
            var firstValidator = new NumberValidator(10, 3, false);
            var secondValidator = new NumberValidator(3, 2, true);

            firstValidator.IsValidNumber("-13456.678").Should().BeTrue();
            secondValidator.IsValidNumber("-3").Should().BeFalse();
        }
    }
}