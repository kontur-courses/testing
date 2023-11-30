using FluentAssertions;
using NUnit.Framework;

namespace NumberValidator.Tests;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(-5, 5, TestName = "Precision < 0")]
    [TestCase(0, 5, TestName = "Precision = 0")]
    [TestCase(5, -5, TestName = "Scale < 0")]
    [TestCase(5, 5, TestName = "Precision = Scale")]
    [TestCase(5, 10, TestName = "Precision < Scale")]
    public void Constructor_Should_ThrowException_When_ArgumentsIncorrect(int precision, int scale)
    {
        var action = new Action(() => _ = new HomeExercises.NumberValidator(precision, scale));
        action.Should().Throw<ArgumentException>();
    }

    [TestCase(10, 5, false, TestName = "Precision > 0, Scale >= 0, Precision > Scale")]
    public void Constructor_ShouldNot_ThrowException_When_ArgumentsAreCorrect(int precision, int scale,
        bool onlyPositive)
    {
        var action = new Action(() => _ = new HomeExercises.NumberValidator(precision, scale, onlyPositive));
        action.Should().NotThrow();
    }

    [TestCase(15, TestName = "Only precision is given")]
    public void Constructor_ShouldNot_ThrowException_When_OneArgumentGiven(int precision)
    {
        var action = new Action(() => _ = new HomeExercises.NumberValidator(precision));
        action.Should().NotThrow();
    }

    [TestCase(1, 0, true, "5", TestName = "No fractional part")]
    [TestCase(3, 0, true, "+99", TestName = "Sign and no fractional part")]
    [TestCase(4, 2, false, "-1.25", TestName = "Negative number")]
    public void IsValid_Should_ReturnTrue_When_CorrectValuesGiven(int precision, int scale,
        bool onlyPositive, string value)
    {
        var validator = new HomeExercises.NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeTrue();
    }

    [TestCase(4, 2, true, "+5,33", TestName = "Works with dot as separator")]
    [TestCase(4, 2, true, "+5.33", TestName = "Works with comma as separator")]
    public void IsValid_Should_Work_When_DotOrCommaAreSeparators(int precision, int scale,
        bool onlyPositive, string value)
    {
        var validator = new HomeExercises.NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeTrue();
    }

    [TestCase(3, 2, true, "00.00", TestName = "Actual precision < expected")]
    [TestCase(3, 2, true, "+0.00",
        TestName = "Sign was not taken into account when forming precision value")]
    [TestCase(17, 2, true, "0.000", TestName = "Actual scale < expected")]
    [TestCase(3, 2, true, "a.sd", TestName = "Letters instead of digits")]
    [TestCase(3, 2, true, "-1.25", TestName = "Negative number when (onlyPositive = true)")]
    [TestCase(10, 5, true, "+", TestName = "No number given")]
    [TestCase(10, 5, true, "+  5, 956", TestName = "Spaces are forbidden")]
    [TestCase(10, 5, true, "45!34", TestName = "Incorrect separator")]
    [TestCase(10, 5, true, "++3.45", TestName = "Two signs")]
    [TestCase(10, 5, true, "2,,66", TestName = "Two separators")]
    [TestCase(10, 5, true, "", TestName = "Empty string as number")]
    [TestCase(10, 5, true, null, TestName = "Null instead of string")]
    public void IsValid_Should_ReturnFalse_When_IncorrectValuesGiven(int precision, int scale,
        bool onlyPositive, string value)
    {
        var validator = new HomeExercises.NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeFalse();
    }
}