using System;
using System.Diagnostics.SymbolStore;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		[TestCase(-1, 2, true, TestName = "Creation_ShouldThrowArgumentException_WhenNegativePrecision")]
		[TestCase(0,0,true, TestName = "Creation_ShouldThrowArgumentException_WhenPrecisionIsZero")]
		[TestCase(3,-1,true, TestName = "Creation_ShouldThrowArgumentException_WhenNegativeScale")]
		[TestCase(1,2,true, TestName = "Creation_ShouldThrowArgumentException_WhenScaleIsGreaterThanPrecision")]
		public void Creation_ShouldThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			Action creation = () => new NumberValidator(precision, scale, onlyPositive);
			creation.Should().Throw<ArgumentException>();
		}

		[Test]
		[TestCase(1, 0, true, TestName = "Creation_ShouldDoesNotThrow_WhenCorrectValue")]
		public void Creation_ShouldDoesNotThrow(int precision, int scale, bool onlyPositive)
		{
			Action creation = () => new NumberValidator(precision, scale, onlyPositive);
			creation.Should().NotThrow<ArgumentException>();
		}

		[Test]
		[TestCase(3,2,true, "-+0.0", TestName = "IsValidNumber_ReturnsFalseWhenTwoSigns")]
		[TestCase(3,2,true, " -0.0", TestName = "IsValidNumber_ReturnsFalseWhenSpaceFirstDigitInNumber")]
		[TestCase(3,2,true, "0,", TestName ="IsValidNumber_ReturnsFalseWhenAren'tDigitsAfterCommas")]
		[TestCase(3,2,true, "0.0.0", TestName = "IsValidNumber_ReturnsFalseWhenThreeDigitsInNumberByTwoDots")]
		[TestCase(3,2,true, "0,0,0", TestName = "IsValidNumber_ReturnsFalseWhenThreeDigitsInNumberByTwoCommas")]
		[TestCase(3, 2, true, "00.00", TestName = "IsValidNumber_ReturnsFalseWhenNumberPrecisionGreaterThanValidatorPrecision")]
		[TestCase(3, 2, true, null, TestName = "IsValidNumber_ReturnsFalseWhenNumberNull")]
		[TestCase(3, 2, true, "", TestName = "IsValidNumber_ReturnsFalseWhenNumberEmpty")]
		[TestCase(3, 2, true, "+1.23", TestName = "IsValidNumber_ReturnsFalseWhenNumberPrecisionGreaterThanValidatorPrecision")]
		[TestCase(3, 2, true, "-1.23", TestName = "IsValidNumber_ReturnsFalseWhenNumberNegativeInNumberOnlyPositive")]
		[TestCase(17, 2, true, "0.000", TestName = "IsValidNumber_ReturnsFalseWhenNumberScaleGreaterThanValidatorScale")]
		[TestCase(17, 2, true, "a.sd", TestName = "IsValidNumber_ReturnsFalseWhenIncorrectNumberFormatBecauseLettersArePresent")]
		public void IsValidNumber_False(int precision, int scale, bool onlyPositive, string number)
		{
			NumberValidator validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(number).Should().BeFalse();
		}
		[Test]
		[TestCase(17, 2, true, "0.0", TestName = "IsValidNumber_ReturnTrueWhenNumberWithFractionalPart")]
		[TestCase(17, 2, true, "0", TestName = "IsValidNumber_ReturnTrueWhenOnlyInteger")]
		[TestCase(4, 2, true, "+1.23", TestName = "IsValidNumber_ReturnTrueWhenNumberWithSign")]
		public void IsValidNumber_True(int precision, int scale, bool onlyPositive, string number)
		{
			NumberValidator validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(number).Should().BeTrue();
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