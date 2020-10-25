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
            Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
            try
            {
                var numberValidator = new NumberValidator(-1, 2, true);
            }
            catch (Exception e)
            {
                e.Message.Should().Be("precision must be a positive number");
            }
        }

        [Test]
        public void Ctor_ThrowArgumentException_WhenScaleMoreThenPrecision()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
        }

        [Test]
        public void Ctor_ThrowArgumentException_WhenScaleNegative()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(1, -1, true));
        }

        [Test]
        public void Ctor_NotThrowException_WhenDataIsCorrect()
        {
            Assert.DoesNotThrow(() => new NumberValidator(5, 3, true));
        }

        [Test]
        public void IsValidNumber_True_FloatNumberWithDot()
        {
            new NumberValidator(17, 2, true).IsValidNumber("0.0").Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_True_FloatNumberWithComma()
        {
            new NumberValidator(17, 2, true).IsValidNumber("0,0").Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_True_PositiveFloatNumberWhithPlus()
        {
            new NumberValidator(17, 2, true).IsValidNumber("+0.0").Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_True_PositiveIntNumber()
        {
            new NumberValidator(17, 2, true).IsValidNumber("0").Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_True_NegativeFloatNumber()
        {
            new NumberValidator(17, 2, false).IsValidNumber("-0.0").Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_False_NotNumber()
        {
            var line = "abc";
            new NumberValidator(17, 2, true).IsValidNumber("abc").Should().BeFalse(line);
        }

        [Test]
        public void IsValidNumber_False_Negative_WhenOnlyPositive()
        {
            new NumberValidator(17, 2, true).IsValidNumber("-1.0").Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_False_WhenMultipleDots()
        {
            new NumberValidator(17, 10, true).IsValidNumber("1.0.0").Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_False_WhenLengthMoreThenPrecision()
        {
            new NumberValidator(3, 2, true).IsValidNumber("+0.00").Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_False_WhenFracPartMoreThenScale()
        {
            new NumberValidator(17, 2, true).IsValidNumber("0.000").Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_False_Null()
        {
            new NumberValidator(17, 2, true).IsValidNumber(null).Should().BeFalse();
        }

        [Test]
        public void IsValidNumber_False_EmptyString()
        {
            new NumberValidator(17, 2, true).IsValidNumber("").Should().BeFalse();
        }
    }
}
