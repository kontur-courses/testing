using System;
using System.Collections.Generic;
using FluentAssertions;
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

		[TestCase("")]
		[TestCase("    ")]
		[TestCase(null)]
		[TestCase("+")]
		[TestCase("-")]
		public void IsValidNumber_False_WithNotNumber(string value)
		{
			var validator = new NumberValidator(int.MaxValue);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase(" 1 ")]
		[TestCase("+ 1")]
		[TestCase("- 1")]
		[TestCase("1.222.333")]
		[TestCase("1,222,333")]
		[TestCase("a")]
		[TestCase("+a")]
		[TestCase("-a")]
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
		[TestCase("a.1")]
		[TestCase("a,1")]
		[TestCase("1.a")]
		[TestCase("1,a")]
		[TestCase("a.a")]
		[TestCase("a,a")]
		public void IsValidNumber_False_WithIncorrectFraction(string value)
		{
			var validator = new NumberValidator(int.MaxValue, int.MaxValue - 1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[TestCase("12")]
		[TestCase("00001")]
		[TestCase("1.2")]
		[TestCase("1,2")]
		[TestCase("+1")]
		[TestCase("-1")]
		public void IsValidNumber_False_WithPrecisionEqualsOneAndValue(string value)
		{
			var validator = new NumberValidator(1);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_False_WithScaleLessThanFractionLength()
		{
			var validator = new NumberValidator(2, 1);
			var actual = validator.IsValidNumber("1.22");
			actual.Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_False_WithUnaccountedFollowingZerosInPrecision()
		{
			var validator = new NumberValidator(2, 1);
			var actual = validator.IsValidNumber("1.000");
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

		[TestCase("1")]
		[TestCase("-1", 2)]
		[TestCase("+1", 2)]
		[TestCase("1", 10)]
		public void IsValidNumber_True_WithInteger(string value, int precision = 1)
		{
			var validator = new NumberValidator(precision);
			var actual = validator.IsValidNumber(value);
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

		[TestCase("123.12345")]
		[TestCase("123,12345")]
		public void IsValidNumber_True_WithLongFraction(string value)
		{
			var validator = new NumberValidator(9, 5);
			var actual = validator.IsValidNumber(value);
			actual.Should().BeTrue();
		}

		[TestCase("-123.12345")]
		[TestCase("+123.12345")]
		[TestCase("-123,12345")]
		[TestCase("+123,12345")]
		public void IsValidNumber_True_WithLongSignedFraction(string value)
		{
			var validator = new NumberValidator(9, 5);
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

		[TestCase("1.2")]
		[TestCase("+1.2")]
		[TestCase("0.0")]
		public void IsValidNumber_True_WithOnlyPositiveFraction(string value)
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
			yield return new TestCaseData(
				$"{new string('1', 1000)}.{new string('2', 1000)}") {TestName = "Fraction"};
		}
	}
}