using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.NumberValidatorTests
{
    class NumberValidator_IsValidNumberTests
    {
        [TestCase(
            17, 2, true,
            "0", "+0",
            TestName = "ShouldValid_OnSomePositiveInteger")]
        [TestCase(
            17, 2, false,
            "000", "+02.3", "-012",
            TestName = "ShouldValid_WhenNumberStartsWithZero_SignIsIgnored")]
        [TestCase(
            17, 2, false,
            "-0",
            TestName = "ShouldValid_OnSomeNegativeInteger")]
        [TestCase(
            17, 2, true,
            "0.0", "+0.0",
            TestName = "ShouldValid_OnSomePositiveFractionalNumber")]
        [TestCase(
            17, 10, false,
            "-0.0",
            TestName = "ShouldValid_OnSomeNegativeFractionalNumber")]
        [TestCase(
            4, 2, false,
            "1234", "123.4", "-123", "+123", "-12.3", "+1.23",
            TestName = "ShouldValid_WhenNumberLengthEqualsMaxValidatorLength")]
        [TestCase(
            5, 2, false,
            "1.23", "-1.23", "+1.23",
            TestName = "ShouldValid_WhenNumberFracPartLengthEqualsMaxValidatorFracPartLength")]
        [TestCase(
            10, 5, false,
            "1233.21", "-12.23", "+1.43",
            TestName = "ShouldValid_WhenSeparatorIsDot")]
        [TestCase(
            10, 5, false,
            "1233,21", "-12,23", "+1,43",
            TestName = "ShouldValid_WhenSeparatorIsComma")]
        [TestCase(
            10, 5, true,
            "+0", "+1.12", "+13,32",
            TestName = "ShouldValid_WhenSignIsPlus")]
        [TestCase(
            10, 5, false,
            "-0", "-1.12", "-13,32",
            TestName = "ShouldValid_WhenSignIsMinus")]
        [TestCase(
            3, 2, true,
            "12.3", "12,3",
            TestName = "SeparatorShouldNotCountInNumberLength")]
        [TestCase(
            10, 4, false,
            "5423", "32.123",
            TestName = "ShouldValid_PositiveNumberInNotOnlyPositiveValidator")]
        [TestCase(
            100, 50, false,
            "1234567891011121131415161718", "-1234567891011121131415161718",
            TestName = "ShouldValid_WhenNumberAbsValueMoreThanMaxUnsignedLongAbsValue")]
        public void ShouldValid_WhenNumberIsCorrectForValidator(
            int validatorInitArg_Precision, int validatorInitArg_Scale, bool validatorInitArg_OnlyPositive,
            params string[] numbers)
        {
            var validator = new NumberValidator(
                validatorInitArg_Precision,
                validatorInitArg_Scale,
                validatorInitArg_OnlyPositive);
            
            foreach (var number in numbers)
                validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase(
            3, 2, true,
            "00.00", "1234",
            TestName = "ShouldInvalid_WhenNumberLengthMoreThanMaxValidatorNumberLength")]
        [TestCase(
            3, 2, true,
            "-123", "-0.00",
            TestName = "ShouldInvalid_WithNegativeNumberInOnlyPositiveValidator")]
        [TestCase(
            3, 2, true,
            "+0.00",
            TestName = "PlusShouldCountInNumberLength")]
        [TestCase(
            3, 2, false,
            "-0.00",
            TestName = "MinusShouldCountInNumberLength")]
        [TestCase(
            17, 2, true,
            "0.000",
            TestName = "ShouldInvalid_WhenFracPartLengthMoreThanMaxValidatorFracPartLength")]
        [TestCase(
            17, 2, false,
            "Asd", "a.sd", "+asd", "-asd",
            TestName = "ShouldInvalid_WithNotANumber")]
        [TestCase(
            17, 2, false,
            "  123",
            TestName = "ShouldInvalid_WhenWhiteSpaceBeforeCorrectNumber")]
        [TestCase(
            17, 2, true,
            new string[] { null },
            TestName = "ShouldInvalid_WithNull")]
        [TestCase(
            17, 2, true,
            "",
            TestName = "ShouldInvalid_WithEmptyString")]
        [TestCase(
            17, 2, true,
            ".12", ",12",
            TestName = "ShouldInvalid_WhenArgumentStartsWithSeparator")]
        public void ShouldInvalid_WhenNumberIsIncorrectForValidator(
            int validatorInitArg_Precision, int validatorInitArg_Scale, bool validatorInitArg_OnlyPositive,
            params string[] numbers)
        {
            var validator = new NumberValidator(
                validatorInitArg_Precision,
                validatorInitArg_Scale,
                validatorInitArg_OnlyPositive);

            foreach (var number in numbers)
                validator.IsValidNumber(number).Should().BeFalse();
        }
    }
}