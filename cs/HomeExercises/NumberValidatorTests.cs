using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private NumberValidator defaultIntegerValidator;
		private NumberValidator defaultFractionValidator;

		[SetUp]
		public void SetUp()
		{
			defaultIntegerValidator = new NumberValidator(int.MaxValue);
			defaultFractionValidator = new NumberValidator(int.MaxValue, int.MaxValue - 1);
		}

		[TestCase(-1, TestName = "precision is negative")]
		[TestCase(1, -1, TestName = "scale is negative")]
		[TestCase(2, 2, TestName = "scale >= precision")]
		public void Constructor_ThrowException_When(int precision, int scale = 0)
		{
			Action act = () => new NumberValidator(precision, scale);
			act.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_PrecisionAndScale_NotStatic()
		{
			var initialValidator = new NumberValidator(4, 1);

			new NumberValidator(1, 0);
			var actual = initialValidator.IsValidNumber("123.1");

			actual.Should().BeTrue();
		}

		[Test]
		public void NumberValidator_OnlyPositive_NotStatic()
		{
			var initialValidator = new NumberValidator(2, onlyPositive: true);

			new NumberValidator(2, onlyPositive: false);
			var actual = initialValidator.IsValidNumber("-1");

			actual.Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ShouldBeFastEnough()
		{
			var valueToValidate = $"-{new string('1', 1000)}.{new string('2', 1000)}";

			Action manyValidations = () =>
			{
				for (var i = 0; i < 20_000; i++)
					defaultIntegerValidator.IsValidNumber(valueToValidate);
			};

			GC.Collect();

			manyValidations.ExecutionTime().Should().BeLessThan(3.Seconds());
		}

		[TestCase("")]
		[TestCase("    ")]
		[TestCase(null)]
		[TestCase("+")]
		[TestCase("-")]
		[TestCase("a")]
		[TestCase("+a")]
		[TestCase("-a")]
		[TestCase("a.1")]
		[TestCase("a,1")]
		[TestCase("1.a")]
		[TestCase("1,a")]
		[TestCase("a.a")]
		[TestCase("a,a")]
		public void IsValidNumber_False_WithNotNumber(string value)
		{
			var actual = defaultFractionValidator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase(" 1 ")]
		[TestCase("+ 1")]
		[TestCase("- 1")]
		[TestCase("1.222.333")]
		[TestCase("1,222,333")]
		public void IsValidNumber_False_WithIncorrectInteger(string value)
		{
			var actual = defaultIntegerValidator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("1.")]
		[TestCase("1,")]
		[TestCase(".1")]
		[TestCase(",1")]
		[TestCase("-.1")]
		[TestCase("-,1")]
		[TestCase("+.1")]
		[TestCase("+,1")]
		[TestCase("1,222.333")]
		[TestCase("1.222,333")]
		[TestCase("1;1")]
		public void IsValidNumber_False_WithIncorrectFraction(string value)
		{
			var actual = defaultFractionValidator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("1.23", 3, 1)]
		[TestCase("1,23", 3, 1)]
		public void IsValidNumber_False_WithScaleLessThanFractionLength(string value, int precision, int scale)
		{
			var validator = new NumberValidator(precision, scale);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("1.100")]
		[TestCase("1,100")]
		public void IsValidNumber_False_WithUnconsideredFollowingZerosInScale(string value)
		{
			var validator = new NumberValidator(int.MaxValue, 1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("12")]
		[TestCase("001", TestName = "with leading zeros")]
		public void IsValidNumber_False_WhenPrecisionUnconsideredIntegerLength(string value)
		{
			var validator = new NumberValidator(1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("+123", 3)]
		[TestCase("-123", 3)]
		[TestCase("-1.1", 2, 1)]
		[TestCase("+1.1", 2, 1)]
		[TestCase("-1,1", 2, 1)]
		[TestCase("+1,1", 2, 1)]
		public void IsValidNumber_False_WithUnconsideredSingInPrecision(string value, int precision, int scale = 0)
		{
			var validator = new NumberValidator(precision, scale);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("100.12")]
		[TestCase("100,12")]
		public void IsValidNumber_False_WithUncountedFractionLengthInPrecision(string value)
		{
			var validator = new NumberValidator(3, 2);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("-1.2", 3, 1)]
		[TestCase("-1", 2)]
		[TestCase("-0", 2)]
		public void IsValidNumber_False_WithOnlyPositiveAndValue(string value, int precision, int scale = 0)
		{
			var validator = new NumberValidator(precision, scale, true);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("123", 3, TestName = "equals to integer length")]
		[TestCase("123", 10, TestName = "greater than integer length")]
		public void IsValidNumber_True_WithIntegerAndPrecision(string value, int precision)
		{
			var validator = new NumberValidator(precision);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[TestCase("+12", 0)]
		[TestCase("-12", 0)]
		[TestCase("-1.1")]
		[TestCase("+1.1")]
		[TestCase("-1,1")]
		[TestCase("+1,1")]
		public void IsValidNumber_True_WithConsideredSingInPrecision(string value, int scale = 1)
		{
			var validator = new NumberValidator(3, scale);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[TestCase("1.1", TestName = "has dot separator")]
		[TestCase("1,1", TestName = "has comma separator")]
		public void IsValidNumber_True_WhenFraction(string value, int precision = 2, int scale = 1)
		{
			var validator = new NumberValidator(precision, scale);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[TestCase("1.1")]
		[TestCase("1,1")]
		[TestCase("-1.1")]
		[TestCase("+1.1")]
		[TestCase("-1,1")]
		[TestCase("+1,1")]
		public void IsValidNumber_True_WithPrecisionAndScaleGreaterThanFraction(string value)
		{
			var actual = defaultFractionValidator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[TestCase("1")]
		[TestCase("+1")]
		[TestCase("0")]
		public void IsValidNumber_True_WithOnlyPositiveInteger(string value)
		{
			var validator = new NumberValidator(int.MaxValue, onlyPositive: true);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[TestCase("1.2")]
		[TestCase("+1.2")]
		[TestCase("0.0")]
		public void IsValidNumber_True_WithOnlyPositiveFraction(string value)
		{
			var validator = new NumberValidator(int.MaxValue, int.MaxValue - 1, true);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[TestCaseSource(nameof(IsValidNumberTrueWithBigNumberCases))]
		public void IsValidNumber_True_WithBigNumber(string value)
		{
			var actual = defaultFractionValidator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		private static IEnumerable<TestCaseData> IsValidNumberTrueWithBigNumberCases()
		{
			var longNumberString = new string('1', 1000);
			yield return new TestCaseData(longNumberString) {TestName = "Integer"};
			yield return new TestCaseData($"+{longNumberString}") {TestName = "+Integer"};
			yield return new TestCaseData($"-{longNumberString}") {TestName = "-Integer"};
			yield return new TestCaseData($"{longNumberString}.{longNumberString}") {TestName = "Fraction with dot"};
			yield return new TestCaseData($"{longNumberString},{longNumberString}") {TestName = "Fraction with comma"};
			yield return new TestCaseData($"+{longNumberString}.{longNumberString}") {TestName = "+Fraction with dot"};
			yield return new TestCaseData($"-{longNumberString}.{longNumberString}") {TestName = "-Fraction with dot"};
			yield return new TestCaseData($"+{longNumberString},{longNumberString}")
				{TestName = "+Fraction with comma"};

			yield return new TestCaseData($"-{longNumberString},{longNumberString}")
				{TestName = "-Fraction with comma"};
		}
	}
}