using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
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
			var validator = new NumberValidator(int.MaxValue);
			var valueToValidate = $"-{new string('1', 1000)}.{new string('2', 1000)}";

			Action manyValidations = () =>
			{
				for (var i = 0; i < 20_000; i++)
					validator.IsValidNumber(valueToValidate);
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
			var validator = new NumberValidator(int.MaxValue, int.MaxValue - 1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase(" 1 ")]
		[TestCase("+ 1")]
		[TestCase("- 1")]
		[TestCase("1.222.333")]
		[TestCase("1,222,333")]
		public void IsValidNumber_False_WithIncorrectInteger(string value)
		{
			var validator = new NumberValidator(int.MaxValue);
			var actual = validator.IsValidNumber(value);
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
		public void IsValidNumber_False_WithIncorrectFraction(string value)
		{
			var validator = new NumberValidator(int.MaxValue, int.MaxValue - 1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_False_WithScaleLessThanFractionLength()
		{
			var validator = new NumberValidator(2, 1);
			var actual = validator.IsValidNumber("1.23");
			actual.Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_False_WithPrecisionLessThanIntegerLength()
		{
			var validator = new NumberValidator(1);
			var actual = validator.IsValidNumber("12");
			actual.Should().BeFalse();
		}

		[TestCase("1.100")]
		[TestCase("1,100")]
		public void IsValidNumber_False_WithUnaccountedInScale_FollowingZerosInFraction(string value)
		{
			var validator = new NumberValidator(2, 1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_False_WithUnaccountedInPrecision_LeadingZeros()
		{
			var validator = new NumberValidator(3);
			var actual = validator.IsValidNumber("0001");
			actual.Should().BeFalse();
		}

		[TestCase("+1")]
		[TestCase("-1")]
		public void IsValidNumber_False_WithUnaccountedInPrecision_IntegerSign(string value)
		{
			var validator = new NumberValidator(1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("100.12")]
		[TestCase("100,12")]
		public void IsValidNumber_False_WithUncountedInPrecision_Fraction(string value)
		{
			var validator = new NumberValidator(3, 2);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("+100.12")]
		[TestCase("-100,12")]
		[TestCase("+100,12")]
		[TestCase("-100.12")]
		public void IsValidNumber_False_WithUncountedInPrecision_SignedFraction(string value)
		{
			var validator = new NumberValidator(4, 2);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("-1.2", 3, 1)]
		[TestCase("-1", 2, 0)]
		[TestCase("-0")]
		public void IsValidNumber_False_WithOnlyPositiveAndValue(string value, int precision = 1, int scale = 0)
		{
			var validator = new NumberValidator(precision, scale, true);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_True_WithInteger()
		{
			var validator = new NumberValidator(3);
			var actual = validator.IsValidNumber("123");
			actual.Should().BeTrue();
		}

		[TestCase("-123")]
		[TestCase("+123")]
		public void IsValidNumber_True_WithSignedInteger(string value)
		{
			var validator = new NumberValidator(4);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_True_WithPrecisionGreaterThanIntegerLength()
		{
			var validator = new NumberValidator(10);
			var actual = validator.IsValidNumber("1");
			actual.Should().BeTrue();
		}

		[TestCase("1.1")]
		[TestCase("1,1")]
		public void IsValidNumber_True_WithFraction(string value)
		{
			var validator = new NumberValidator(2, 1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[TestCase("-1.1")]
		[TestCase("+1.1")]
		[TestCase("-1,1")]
		[TestCase("+1,1")]
		public void IsValidNumber_True_WithSignedFraction(string value)
		{
			var validator = new NumberValidator(3, 1);
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
			var validator = new NumberValidator(int.MaxValue, int.MaxValue - 1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[TestCase("1")]
		[TestCase("+1")]
		[TestCase("0")]
		public void IsValidNumber_True_WithOnlyPositiveInteger(string value)
		{
			var validator = new NumberValidator(int.MaxValue);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[TestCase("1.2")]
		[TestCase("+1.2")]
		[TestCase("0.0")]
		public void IsValidNumber_True_WithOnlyPositiveFraction(string value)
		{
			var validator = new NumberValidator(int.MaxValue, int.MaxValue - 1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[TestCaseSource(nameof(IsValidNumberTrueWithBigNumberCases))]
		public void IsValidNumber_True_WithBigNumber(string value)
		{
			var validator = new NumberValidator(int.MaxValue, int.MaxValue - 1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		private static IEnumerable<TestCaseData> IsValidNumberTrueWithBigNumberCases()
		{
			yield return new TestCaseData(new string('1', 1000)) {TestName = "Integer"};
			yield return new TestCaseData($"+{new string('1', 1000)}") {TestName = "+Integer"};
			yield return new TestCaseData($"-{new string('1', 1000)}") {TestName = "-Integer"};
			yield return new TestCaseData(
				$"{new string('1', 1000)}.{new string('2', 1000)}") {TestName = "Fraction with dot"};

			yield return new TestCaseData(
				$"{new string('1', 1000)},{new string('2', 1000)}") {TestName = "Fraction with comma"};

			yield return new TestCaseData(
				$"{new string('1', 1000)}.{new string('2', 1000)}") {TestName = "+Fraction with dot"};

			yield return new TestCaseData(
				$"{new string('1', 1000)}.{new string('2', 1000)}") {TestName = "-Fraction with dot"};

			yield return new TestCaseData(
				$"{new string('1', 1000)},{new string('2', 1000)}") {TestName = "+Fraction with comma"};

			yield return new TestCaseData(
				$"{new string('1', 1000)},{new string('2', 1000)}") {TestName = "-Fraction with comma"};
		}
	}
}