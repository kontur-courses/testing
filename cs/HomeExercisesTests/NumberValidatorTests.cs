using System;
using FluentAssertions;
using HomeExercises;
using NUnit.Framework;

namespace HomeExercisesTests
{
    public class NumberValidatorTests
    {
        [TestCase(1)]
        [TestCase(1, 0, true)]
        [TestCase(1, 0, false)]
        public void Should_NotThrow_When_InstancedWithCorrectParameters(int precision,
            int scale = 0, bool onlyPositive = false)
        {
            FluentActions.Invoking(() => new NumberValidator(precision, scale, onlyPositive))
                .Should().NotThrow();
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_Throw_When_InstancedWithIncorrectPrecision(int precision)
        {
            FluentActions.Invoking(() => new NumberValidator(precision))
                .Should().Throw<ArgumentException>();
        }

        [TestCase(1, 1)]
        [TestCase(1, -1)]
        public void Should_Throw_When_InstancedWithIncorrectScale(int precision, int scale)
        {
            FluentActions.Invoking(() => new NumberValidator(precision, scale))
                .Should().Throw<ArgumentException>();
        }

        static object[] IntegerCases =
            {
            new object[] { 10, 0, true, "1" },
            new object[] { 10, 0, false, "+1" },
            new object[] { 10, 0, false, "1" },
            new object[] { 10, 0, false, "-1" },
            new object[] { 10, 0, false, "-0" }
        };

        static object[] DoubleCases =
            {
            new object[] { 10, 5, true, "1.001" },
            new object[] { 10, 5, true, "1,001" },
            new object[] { 10, 5, false, "-1.001" },
            new object[] { 10, 5, false, "+1.001" },
            new object[] { 10, 5, false, "-0.0"},
        };

        [TestCaseSource(nameof(IntegerCases))]
        [TestCaseSource(nameof(DoubleCases))]
        public void Should_Pass_When_CorrectNumber(int precision,
            int scale, bool onlyPositive, string inputNumber)
        {
            new NumberValidator(precision, scale, onlyPositive)
                .IsValidNumber(inputNumber)
                .Should().BeTrue();
        }

        [TestCase(2, 0, "123")]
        [TestCase(2, 1, "12.1")]
        [TestCase(2, 0, "-12")]
        [TestCase(2, 1, "-1.2")]
        [TestCase(2, 0, "+12")]
        public void Should_Fail_When_NumberExceedPrecision(int precision,
            int scale, string inputNumber)
        {
            new NumberValidator(precision, scale)
                .IsValidNumber(inputNumber)
                .Should().BeFalse();
        }

        [TestCase(10, 0, "123.1")]
        [TestCase(10, 0, "123.0")]
        [TestCase(10, 1, "123.01")]
        [TestCase(10, 1, "123.00")]
        public void Should_Fail_When_NumberExceedScale(int precision,
            int scale, string inputNumber)
        {
            new NumberValidator(precision, scale)
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
            new NumberValidator(10, 5, false)
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