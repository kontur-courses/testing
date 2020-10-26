using FluentAssertions;
using NUnit.Framework;
using System;
using HomeExercises;

namespace TestsForHomeExercises
{
    public class NumberValidatorTests
    {
	    [Test]
        public void Ctor_ThrowArgumentException_WhenPrecisionNotPositive()
        {
	        var exception = Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
	        exception.Message.Should().Be("precision must be a positive number");
        }

        [TestCase (1,2,true, TestName = "Ctor_ThrowArgumentException_ScaleMoreThenPrecision")]
        [TestCase(1, -1, true, TestName = "Ctor_ThrowArgumentException_ScaleNegative")]

        public void CtorTest(int precision, int scale, bool onlyPositive)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
        }

        [Test]
        public void Ctor_NotThrowException_WhenDataIsCorrect()
        {
            Assert.DoesNotThrow(() => new NumberValidator(5, 3, true));
        }

        [TestCase(17, 2, true, "0.0", TestName = "IsValidNumber_True_PositiveFloatNumberWithDot")]
        [TestCase(17, 2, true, "0,0", TestName = "IsValidNumber_True_PositiveFloatNumberWithComma")]
        [TestCase(17, 2, true, "+0.0", TestName = "IsValidNumber_True_PositiveFloatNumberWhithPlus")]
        [TestCase(17, 2, true, "0", TestName = "IsValidNumber_True_PositiveIntNumber")]
        [TestCase(17, 2, true, "+0", TestName = "IsValidNumber_True_PositiveIntNumberWhithPlus")]
        [TestCase(17, 2, false, "-0.0", TestName = "IsValidNumber_True_NegativeFloatNumber")]
        [TestCase(17, 2, false, "-0", TestName = "IsValidNumber_True_NegativeIntNumber")]

        public void IsValidNumber_True(int precision, int scale, bool onlyPositive, string numberToValidate)
        {
            new NumberValidator(precision,scale,onlyPositive).IsValidNumber(numberToValidate).Should().BeTrue();
        }

        [TestCase(17, 2, true, "abc", TestName = "IsValidNumber_False_NotNumber")]
        [TestCase(17, 2, true, "-1.0", TestName = "IsValidNumber_False_NegativeFloat_WhenOnlyPositive")]
        [TestCase(17, 2, true, "-1", TestName = "IsValidNumber_False_NegativeInt_WhenOnlyPositive")]
        [TestCase(17, 2, true, "0.0.0", TestName = "IsValidNumber_False_WhenMultipleDots")]
        [TestCase(3, 2, true, "+0.00", TestName = "IsValidNumber_False_WhenLengthMoreThenPrecision")]
        [TestCase(17, 2, true, "0.000", TestName = "IsValidNumber_False_WhenFracPartMoreThenScale")]
        [TestCase(17, 2, true, null, TestName = "IsValidNumber_False_Null")]
        [TestCase(17, 2, true, "", TestName = "IsValidNumber_False_EmptyString")]
        public void IsValidNumber_False(int precision, int scale, bool onlyPositive, string numberToValidate)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numberToValidate).Should().BeFalse();
        }
    }
}
