using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Category("throw ArgumentException")]
		[TestCase(3, 4, TestName = "ScaleGreaterThanPrecision")]
		[TestCase(10, 10, TestName = "ScaleEqualPrecision")]
		[TestCase(-1, 2, TestName = "NegativePrecision")]
		[TestCase(17, -1, TestName = "NegativeScale")]
		public void Fails_When_BadPrecisionOrScale(int precision, int scale)
		{
			var action = new Func<NumberValidator>(() =>
				NumberValidator.Create(precision, scale));
			action.Should().Throw<ArgumentException>();
		}

		[Category("invalid data")]
		[TestCase(17, 2, "ы", TestName = "BadRegex")]
		[TestCase(17, 2, null, TestName = "NullString")]
		[TestCase(17, 2, "", TestName = "EmptyString")]
		public void ShouldReturn_False_When_InvalidData(int precision, int scale, string value)
		{
			NumberValidator.Create(precision, scale).IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		public void ShouldReturn_False_MinusValue_And_OnlyPositive()
		{
			NumberValidator.Create(17, 2, true).IsValidNumber("-0.0").Should().BeFalse();
		}

		[Category("wrong value")]
		[TestCase(17, 0, "0.0", TestName = "FracPartWithZeroScale")]
		[TestCase(17, 2, "0.000", TestName = "FracPartLongerThenScale")]
		[TestCase(2, 1, "+0.0", TestName = "SymbolsMoreThenPrecisionWithPlusSign")]
		[TestCase(2, 1, "-0.0", TestName = "SymbolsMoreThenPrecisionWithMinusSign")]
		[TestCase(4, 3, "0.0000", TestName = "SymbolsMoreThenPrecisionWithScale")]
		[TestCase(2, 0, "189", TestName = "SymbolsMoreThenPrecisionWithoutScale")]
		public void ShouldReturn_False_When_IncorrectValue(int precision, int scale, string value)
		{
			NumberValidator.Create(precision, scale).IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		public void CorrectValidation()
		{
			NumberValidator.Create(17, 2, true).IsValidNumber("0.0").Should().BeTrue();
			NumberValidator.Create(17, 2, true).IsValidNumber("0.0").Should().BeTrue();
			NumberValidator.Create(17, 2, true).IsValidNumber("0").Should().BeTrue();
			NumberValidator.Create(4, 2, true).IsValidNumber("+1.23").Should().BeTrue();
		}
	}
}