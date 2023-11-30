using FluentAssertions;
using HomeExercises.NumberValidatorExercise;
using NUnit.Framework;
using System;

namespace HomeExercisesTests.NumberValidatorExercise
{
    public class NumberValidatorTests
    {
        [TestCase(-1, 2, true, TestName = "Constructor Should Throw When Negative Precision")]
        [TestCase(0, 2, true, TestName = "Constructor Should Throw When Zero Precision")]
        [TestCase(4, -1, true, TestName = "Constructor Should Throw When Negative Scale")]
        [TestCase(3, 3, true, TestName = "Constructor Should Throw When Scale Equal Precision")]
        [TestCase(3, 4, true, TestName = "Constructor Should Throw When Scale Greater Precision")]
        public void ConstructorShouldThrowArgumentExceptionWhenInvalidArgumentsGiven(int precision, int scale, bool onlyPositive)
        {
            Action numberValidator = () => new NumberValidator(precision, scale, onlyPositive);

            numberValidator.Should().Throw<ArgumentException>();
        }

        [TestCase(2, 0, true, TestName = "Constructor Should Not Throw When Three Valid Arguments Given")]
        [TestCase(2, 1, true, TestName = "Constructor Should Not Throw When Three Valid Arguments Given")]
        [TestCase(2, 0, false, TestName = "Constructor Should Not Throw When Three Valid Arguments Given")]
        [TestCase(2, 1, false, TestName = "Constructor Should Not Throw When Three Valid Arguments Given")]
        public void ConstructorShouldNotThrowWhenThreeValidArgumentsGiven(int precision, int scale, bool onlyPositive)
        {
            Action numberValidator = () => new NumberValidator(precision, scale, onlyPositive);

            numberValidator.Should().NotThrow<ArgumentException>();
        }

        [TestCase(2, 0, TestName = "Constructor Should Not Throw When Two Valid Arguments Given")]
        [TestCase(2, 1, TestName = "Constructor Should Not Throw When Two Valid Arguments Given")]
        public void ConstructorShouldNotThrowWhenTwoValidArgumentsGiven(int precision, int scale)
        {
            Action numberValidator = () => new NumberValidator(precision, scale);

            numberValidator.Should().NotThrow<ArgumentException>();
        }

        [TestCase(2, TestName = "Constructor Should Not Throw When One Valid Argument Given")]
        public void ConstructorShouldNotThrowWhenOneValidArgumentGiven(int precision)
        {
            Action numberValidator = () => new NumberValidator(precision);

            numberValidator.Should().NotThrow<ArgumentException>();
        }

        [TestCase(4, 2, true, "a.sd", TestName = "IsValidNumber Should Be False When IntPart And FracPart Is Not Numerical")]
        [TestCase(4, 2, true, "0.a", TestName = "IsValidNumber Should Be False When FracPart Is Not Numerical")]
        [TestCase(4, 2, true, "a.0", TestName = "IsValidNumber Should Be False When IntPart Is Not Numerical")]
        [TestCase(4, 2, true, ".0", TestName = "IsValidNumber Should Be False When Value Has No IntPart")]
        [TestCase(4, 2, true, "0.", TestName = "IsValidNumber Should Be False When Value Has Delimiter But No FracPart")]
        [TestCase(4, 2, true, "", TestName = "IsValidNumber Should Be False When Value Is Empty")]
        [TestCase(4, 2, true, null, TestName = "IsValidNumber Should Be False When Value Is Null")]
        [TestCase(4, 2, true, "12345.12", TestName = "IsValidNumber Should Be False When Value Length Is Greater Precision")]
        [TestCase(4, 2, true, "1.123", TestName = "IsValidNumber Should Be False When Frac Length Is Greater Scale")]
        [TestCase(4, 2, true, "-1.1", TestName = "IsValidNumber Should Be False When Value Is Negative And OnlyPositive Is True")]
        public void IsValidNumberShouldBeFalseWhenValueIsInvalid(int precision, int scale, bool onlyPositive, string value)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);

            var result = numberValidator.IsValidNumber(value);

            result.Should().BeFalse();
        }

        [TestCase(4, 2, true, "12.12", TestName = "IsValidNumber Should Be True When Value Delimiter Is Dot")]
        [TestCase(4, 2, true, "12,12", TestName = "IsValidNumber Should Be True When Value Delimiter Is Comma")]
        [TestCase(4, 2, false, "12.12", TestName = "IsValidNumber Should Be True When Value is Positive And OnlyPositive Is False")]
        [TestCase(4, 2, false, "-1.12", TestName = "IsValidNumber Should Be True When Value is Negative And OnlyPositive Is False")]
        [TestCase(4, 2, true, "0", TestName = "IsValidNumber Should Be True When Value Has No FracPart")]
        public void TestIsValidNumberValidCases(int precision, int scale, bool onlyPositive, string value)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);

            var result = numberValidator.IsValidNumber(value);

            result.Should().BeTrue();
        }
    }
}
