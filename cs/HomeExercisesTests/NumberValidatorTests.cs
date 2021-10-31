using System;
using FluentAssertions;
using HomeExercises;
using NUnit.Framework;

namespace HomeExercisesTests
{
    public class NumberValidatorTests
    {
        [TestCase(1, 0, false)]
        [TestCase(1, 0, true)]
        public void Should_NotThrow_When_InstantiatedWithCorrectParameters(int precision,
            int scale, bool onlyPositive)
        {
            Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_Throw_When_InstantiatedWithIncorrectPrecision(int precision)
        {
            FluentActions.Invoking(() => new NumberValidator(precision))
                .Should().Throw<ArgumentException>();
        }

        [TestCase(10)]
        [TestCase(20)]
        public void Should_Throw_When_InstantiatedWithScaleThatIsNotLessThanPrecision(
            int scale)
        {
            FluentActions.Invoking(() => new NumberValidator(10, scale))
                .Should().Throw<ArgumentException>();
        }

        [TestCase(-1)]
        public void Should_Throw_When_InstantiatedWithNegativeScale(int scale)
        {
            FluentActions.Invoking(() => new NumberValidator(10, scale))
                .Should().Throw<ArgumentException>();
        }

        static object[] IntegerCases =
        {
            new object[] { true, "1" },
            new object[] { false, "+1" },
            new object[] { false, "1" },
            new object[] { false, "-1" },
            new object[] { false, "-0" }
        };

        static object[] DoubleCases =
        {
            new object[] { true, "1.001" },
            new object[] { true, "1,001" },
            new object[] { false, "-1.001" },
            new object[] { false, "+1.001" },
            new object[] { false, "-0.0" }
        };

        [TestCaseSource(nameof(IntegerCases))]
        [TestCaseSource(nameof(DoubleCases))]
        public void Should_Pass_When_CorrectNumber(bool onlyPositive, string inputNumber)
        {
            new NumberValidator(10, 5, onlyPositive)
                .IsValidNumber(inputNumber)
                .Should().BeTrue();
        }

        [TestCase(2, "123")]
        [TestCase(2, "12.1")]
        [TestCase(2, "-12")]
        [TestCase(2, "-1.2")]
        [TestCase(2, "+12")]
        public void Should_Fail_When_NumberExceedPrecision(int precision,
            string inputNumber)
        {
            new NumberValidator(precision, 1)
                .IsValidNumber(inputNumber)
                .Should().BeFalse();
        }

        [TestCase(0, "123.1")]
        [TestCase(0, "123.0")]
        [TestCase(1, "123.01")]
        [TestCase(1, "123.00")]
        public void Should_Fail_When_NumberExceedScale(int scale, string inputNumber)
        {
            new NumberValidator(10, scale)
                .IsValidNumber(inputNumber)
                .Should().BeFalse();
        }

        [TestCase("-0")]
        [TestCase("-0.0")]
        [TestCase("-12.3")]
        public void Should_Fail_When_OnlyPositiveValidator_ReceivedNegativeNumber(
            string inputNumber)
        {
            new NumberValidator(10, 5, true)
                .IsValidNumber(inputNumber)
                .Should().BeFalse();
        }

        [TestCase("")]
        [TestCase(null)]
        public void Should_Fail_When_NumberIsEmpty(string inputNumber)
        {
            new NumberValidator(10, 5)
                .IsValidNumber(inputNumber)
                .Should().BeFalse();
        }

        [TestCase("a")]
        [TestCase("a.b")]
        [TestCase("--1")]
        [TestCase("a-12.3")]
        [TestCase("12..3")]
        [TestCase("1.2.3")]
        [TestCase("12.")]
        [TestCase(".3")]
        [TestCase(" ")]
        public void Should_Fail_When_NumberIsInIncorrectFormat(string inputNumber)
        {
            new NumberValidator(10, 5)
                .IsValidNumber(inputNumber)
                .Should().BeFalse();
        }
    }
}