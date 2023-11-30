using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {
        [Category("NumberValidator.Constructor; Exception")]
        [TestCase(-5, 5, TestName = "Precision < 0")]
        [TestCase(0, 5, TestName = "Precision = 0")]
        [TestCase(5, -5, TestName = "Scale < 0")]
        [TestCase(5, 5, TestName = "Precision = Scale")]
        [TestCase(5, 10, TestName = "Precision < Scale")]
        public void Constructor_With_IncorrectArguments_Should_ThrowException(int precision, int scale)
        {
            var action = new Action(() => new NumberValidator(precision, scale));
            action.Should().Throw<ArgumentException>();
        }

        [Category("NumberValidator.Constructor; No exception")]
        [TestCase(10, 5, false, TestName = "Correct parameters")]
        public void Constructor_With_CorrectArguments_ShouldNot_ThrowException(int precision,
            int scale, bool onlyPositive)
        {
            var action = new Action(() => new NumberValidator(precision, scale, onlyPositive));
            action.Should().NotThrow();
        }

        [Category("NumberValidator.Constructor; No exception")]
        [TestCase(15, TestName = "Only precision is given")]
        public void Constructor_With_OneArgument_ShouldNot_ThrowException(int precision)
        {
            var action = new Action(() => new NumberValidator(precision));
            action.Should().NotThrow();
        }

        [Category("NumberValidator.IsValid(...); Correct values")]
        [TestCase(1, 0, true, "5", TestName = "No fraction part")]
        [TestCase(3, 0, true, "+99", TestName = "Sign and no fraction part")]
        [TestCase(4, 2, false, "-1.25", TestName = "Negative number")]
        public void IsValid_ShouldReturn_True_When_CorrectValue(int precision, int scale,
            bool onlyPositive, string value)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            validator.IsValidNumber(value).Should().BeTrue();
        }

        [Category("NumberValidator.IsValid(...); Separators")]
        [TestCase(4, 2, true, "+5,33", TestName = "Works with dot as separator")]
        [TestCase(4, 2, true, "+5.33", TestName = "Works with comma as separator")]
        public void IsValid_ShouldWork_WithDotAndComma_Separators(int precision, int scale,
            bool onlyPositive, string value)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            validator.IsValidNumber(value).Should().BeTrue();
        }

        [Category("NumberValidator.IsValid(...); Incorrect values")]
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
        public void IsValid_ShouldReturn_False_When_IncorrectValue(int precision, int scale,
            bool onlyPositive, string value)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            validator.IsValidNumber(value).Should().BeFalse();
        }
    }

    public class NumberValidator
    {
        private readonly Regex numberRegex;
        private readonly bool onlyPositive;
        private readonly int precision;
        private readonly int scale;

        public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
        {
            this.precision = precision;
            this.scale = scale;
            this.onlyPositive = onlyPositive;

            if (precision <= 0)
                throw new ArgumentException("precision must be a positive number");

            if (scale < 0 || scale >= precision)
                throw new ArgumentException("precision must be a non-negative number less or equal than precision");

            numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
        }

        public bool IsValidNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            var match = numberRegex.Match(value);

            if (!match.Success)
                return false;
            
            var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
            var fracPart = match.Groups[4].Value.Length;

            if (intPart + fracPart > precision || fracPart > scale)
                return false;

            if (onlyPositive && match.Groups[1].Value == "-")
                return false;

            return true;
        }
    }
}