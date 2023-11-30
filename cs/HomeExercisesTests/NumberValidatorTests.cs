using FluentAssertions;
using HomeExercises;
using NUnit.Framework;

namespace HomeExercisesTests
{
    public class NumberValidatorTests
    {
        [TestCase(-1, 2, true, TestName = "Constructor_PrecisionNotPositive_ThrowsArgumentException")]
        [TestCase(0, 1, true, TestName = "Constructor_PrecisionIsZero_ThrowsArgumentException")]
        [TestCase(1, -1, true, TestName = "Constructor_ScaleNegative_ThrowsArgumentException")]
        [TestCase(1, 2, true, TestName = "Constructor_SacleBiggerPrecision_ThrowsArgumentException")]
        [TestCase(1, 1, true, TestName = "Constructor_ScaleEqualsPrecision_ThrowsArgumentException")]
        public void Throws(int precision, int scale, bool onlyPostive)
        {
            Action action = () => new NumberValidator(precision, scale, onlyPostive);
            action.Should().Throw<ArgumentException>();
        }

        [TestCase(2, 1, true, TestName = "Constructor_PrecisionBiggerScale_NotTrow")]
        [TestCase(17, 2, false, TestName = "Constructor_OnlyPositiveFalse_NotTrow")]
        public void NotTrow(int precision, int scale, bool onlyPostive)
        {
            Action action = () => new NumberValidator(precision, scale, onlyPostive);
            action.Should().NotThrow<ArgumentException>();
        }

        [TestCase(17, 2, true, " ", TestName = "IsValidNumber_NumberEmpty_Fails")]
        [TestCase(8, 2, true, "1/43", TestName = "IsValidNumber_ValueNotMatch_Fails")]
        [TestCase(3, 2, true, "00.00", TestName = "IsValidNumber_PrecisionLessCountNumbers_Fails")]
        [TestCase(3, 2, false, "-0.00", TestName = "IsValidNumber_PrecisionLessCountNumbersAndSign_Fails")]
        [TestCase(17, 2, true, "0.001", TestName = "IsValidNumber_ScaleLessFractPartLength_Fails")]
        [TestCase(5, 4, true, "a.12", TestName = "IsValidNumber_LettersInValue_Fails")]
        [TestCase(3, 2, true, ".12", TestName = "IsValidNumber_WithoutIntPart_Fails")]
        [TestCase(3, 2, true, "-1.2", TestName = "IsValidNumber_OnlyPositiveTrueAndNegativeNumber_Fails")]
        [TestCase(3, 2, true, null, TestName = "IsValidNumber_ValueIsNull_Fails")]
        public void Fails(int precision, int scale, bool onlyPostive, string value)
        {
            var validator = new NumberValidator(precision, scale, onlyPostive);
            var result = validator.IsValidNumber(value);
            result.Should().BeFalse();
        }

        [TestCase(4, 2, true, "+1.23", TestName = "IsValidNumber_PrecisionEqualsCountNumbersAndSign_Success")]
        [TestCase(3, 2, true, "5.13", TestName = "IsValidNumber_PrecisionEqualsCountNumbers_Success")]
        [TestCase(4, 2, true, "2.25", TestName = "IsValidNumber_PrecisionBiggerCountNumbers_Success")]
        [TestCase(4, 2, true, "9999", TestName = "IsValidNumber_PrecisionEqualsIntPart_Success")]
        [TestCase(17, 2, true, "0.23", TestName = "IsValidNumber_ScaleEqualsFractPart_Success")]
        [TestCase(17, 3, true, "0.12", TestName = "IsValidNumber_ScaleBiggerFractPart_Success")]
        [TestCase(4, 3, true, "1,1", TestName = "IsValidNumber_SeparatorIsComma_Success")]
        [TestCase(5, 4, false, "-1.5", TestName = "IsValidNumber_NumberIsNegativeAndOnlyPositiveIsFalse_Success")]
        public void Success(int precision, int scale, bool onlyPostive, string value)
        {
            var validator = new NumberValidator(precision, scale, onlyPostive);
            var result = validator.IsValidNumber(value);
            result.Should().BeTrue();
        }
    }
}