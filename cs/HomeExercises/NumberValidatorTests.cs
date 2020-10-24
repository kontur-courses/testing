using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {
        [TestCase(-1, TestName = "When_Precision_Is_Negative")]
        [TestCase(0, TestName = "When_Precision_Is_Zero")]
        [TestCase(1, 2, TestName = "When_Scale_Is_Bigger_Than_Precision")]
        public void NumberValidatorConstructor_ThrowsArgumentException(int precision, int scale = 0)
        {
            Assert.Throws<ArgumentException>(() => NumberValidator.AreFieldsCorrect(precision, scale));
        }

        [TestCase("0", true, 1, TestName = "When_Number_Match_Precision")]
        [TestCase("-1.23", false, 3, 2, TestName = "When_Number_Is_Negative_And_Float")]
        [TestCase("-1", false, 1, TestName = "When_Number_Is_Negative_And_Int")]
        [TestCase("-10.68", true, 5, 2, TestName = "When_Number_Is_Negative_And_Float")]
        [TestCase("+1.23", true, 3, 2, TestName = "Do_Not_Consider_Plus_And_Number_Is_Float")]
        [TestCase("+1", true, 1, TestName = "Do_Not_Consider_Plus_And_Number_Is_Int")]
        [TestCase("-1.23", false, 4, 2, true, TestName = "When_Number_Is_Negative_Float_And_OnlyPositive_Is_True")]
        [TestCase("-1", false, 2, 0, true, TestName = "When_Number_Is_Negative_Int_And_OnlyPositive_Is_True")]
        [TestCase("a.sd", false, 3, TestName = "When_Letters_Are_In_Int_Part_And_Frac_Part")]
        [TestCase("", false, 1, TestName = "When_Input_Is_Empty")]
        [TestCase(null, false, 1, TestName = "When_Input_Is_Null")]
        [TestCase("4a", false, 3, TestName = "When_Number_And_Letter_Are_In_Int_Part")]
        [TestCase("4.a", false, 3, TestName = "When_Number_In_Int_Part_And_Letter_In_Frac_Part")]
        public void IsValidNumber_WorksCorrectly(string number, bool expected, int precision,
            int scale = 0, bool onlyPositive = false)
        {
            var numberValidator = new NumberValidator(precision, scale, onlyPositive);
            numberValidator.IsValidNumber(number).Should().Be(expected);
        }
    }
}