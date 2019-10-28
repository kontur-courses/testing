using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.NumberValidatorTests
{
    class NumberValidator_IsValidNumberTests
    {
        [TestCase(
            17, 2, false,
            "-0",
            TestName = "ShouldValid_OnSomeNegativeInteger")]
        [TestCase(
            17, 10, false,
            "-0.0",
            TestName = "ShouldValid_OnSomeNegativeFractionalNumber")]
        public void ShouldValid_WhenNumberIsCorrectForValidator(
            int validatorInitArgPrecision, int validatorInitArgScale, bool validatorInitArgOnlyPositive,
            string number)
        {
            var validator = new NumberValidator(
                validatorInitArgPrecision,
                validatorInitArgScale,
                validatorInitArgOnlyPositive);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("1234567891011121131415161718")]
        [TestCase("-1234567891011121131415161718")]
        public void ShouldValid_WhenNumberAbsValueMoreThanMaxUnsignedLongAbsValue(string number)
        {
            var validator = new NumberValidator(100, 50, false);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("5423")]
        [TestCase("32.123")]
        public void ShouldValid_PositiveNumberInNotOnlyPositiveValidator(string number)
        {
            var validator = new NumberValidator(10, 4, false);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("12.3")]
        [TestCase("12,3")]
        public void SeparatorShouldNotCountInNumberLength(string number)
        {
            var validator = new NumberValidator(3, 2, true);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("-0")]
        [TestCase("-1.12")]
        [TestCase("-13,32")]
        public void ShouldValid_WhenSignIsMinus(string number)
        {
            var validator = new NumberValidator(10, 5, false);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("+0")]
        [TestCase("+1.12")]
        [TestCase("+13,32")]
        public void ShouldValid_WhenSignIsPlus(string number)
        {
            var validator = new NumberValidator(10, 5, true);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("1233,21")]
        [TestCase("-12,23")]
        [TestCase("+1,43")]
        public void ShouldValid_WhenSeparatorIsComma(string number)
        {
            var validator = new NumberValidator(10, 5, false);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("1233.21")]
        [TestCase("-12.23")]
        [TestCase("+1.43")]
        public void ShouldValid_WhenSeparatorIsDot(string number)
        {
            var validator = new NumberValidator(10, 5, false);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("1.23")]
        [TestCase("-1.23")]
        [TestCase("+1.23")]
        public void ShouldValid_WhenNumberFracPartLengthEqualsMaxValidatorFracPartLength(string number)
        {
            var validator = new NumberValidator(5, 2, false);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("1234")]
        [TestCase("123.4")]
        [TestCase("-123")]
        [TestCase("+123")]
        [TestCase("-12.3")]
        [TestCase("+1.23")]
        public void ShouldValid_WhenNumberLengthEqualsMaxValidatorLength(string number)
        {
            var validator = new NumberValidator(4, 2, false);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("0.0")]
        [TestCase("+0.0")]
        public void ShouldValid_OnSomePositiveFractionalNumber(string number)
        {
            var validator = new NumberValidator(17, 2, true);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("000")]
        [TestCase("+02.3")]
        [TestCase("-012")]
        public void ShouldValid_WhenNumberStartsWithZero_SignIsIgnored(string number)
        {
            var validator = new NumberValidator(17, 2, false);

            validator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("0")]
        [TestCase("+0")]
        public void ShouldValid_OnSomePositiveInteger(string number)
        {
            var validator = new NumberValidator(17, 2, true);

            validator.IsValidNumber(number).Should().BeTrue();
        }

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
            "  123",
            TestName = "ShouldInvalid_WhenWhiteSpaceBeforeCorrectNumber")]
        [TestCase(
            17, 2, true,
            null,
            TestName = "ShouldInvalid_WithNull")]
        [TestCase(
            17, 2, true,
            "",
            TestName = "ShouldInvalid_WithEmptyString")]
        public void ShouldInvalid_WhenNumberIsIncorrectForValidator(
            int validatorInitArgPrecision, int validatorInitArgScale, bool validatorInitArgOnlyPositive,
            string number)
        {
            var validator = new NumberValidator(
                validatorInitArgPrecision,
                validatorInitArgScale,
                validatorInitArgOnlyPositive);

            validator.IsValidNumber(number).Should().BeFalse();
        }

        [TestCase(".12")]
        [TestCase(",12")]
        public void ShouldInvalid_WhenArgumentStartsWithSeparator(string number)
        {
            var validator = new NumberValidator(17, 2, true);

            validator.IsValidNumber(number).Should().BeFalse();
        }

        [TestCase("Asd")]
        [TestCase("a.sd")]
        [TestCase("+asd")]
        [TestCase("-asd")]
        public void ShouldInvalid_WithNotANumber(string number)
        {
            var validator = new NumberValidator(17, 2, false);

            validator.IsValidNumber(number).Should().BeFalse();
        }

        [TestCase("-123")]
        [TestCase("-0.00")]
        public void ShouldInvalid_WithNegativeNumberInOnlyPositiveValidator(string number)
        {
            var validator = new NumberValidator(3, 2, true);

            validator.IsValidNumber(number).Should().BeFalse();
        }

        [TestCase("00.00")]
        [TestCase("1234")]
        public void ShouldInvalid_WhenNumberLengthMoreThanMaxValidatorNumberLength(string number)
        {
            var validator = new NumberValidator(3, 2, true);

            validator.IsValidNumber(number).Should().BeFalse();
        }
    }
}