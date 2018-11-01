using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true)]
		[TestCase(-1, 2, false)]
		[TestCase(0, 2, false)]
		[TestCase(12, -12, false)]
		[TestCase(12, 24, true)]
		[TestCase(12, 12, false)]
		public void ThrowsException_OnInvalidArguments(int precision, int scale, bool onlyPositive)
		{
			Action creation = () => new NumberValidator(precision, scale);
			creation.Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, true)]
		[TestCase(12, 11, false)]
		public void DoesnotThrowException_OnValidArguments(int precision, int scale, bool onlyPositive)
		{
			Action creation = () => new NumberValidator(precision, scale);
			creation.Should().NotThrow<ArgumentException>();
		}

		[TestCase(17, 2, true, "0.0")]
		[TestCase(17, 2, true, "0")]
		[TestCase(4, 2, true, "+1.23")]
		[TestCase(4, 2, true, "+1,23")]
		[TestCase(4, 2, false, "-1,23")]
		public void ValidNumber_OnValidArguments(int precision, int scale, bool onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase(3, 2, true, null)]
		[TestCase(3, 2, true, "")]
		[TestCase(3, 2, true, "a.sd")]
		[TestCase(3, 2, false, "12/2")]
		[TestCase(3, 2, true, "00.00")]
		[TestCase(3, 2, false, "-0.00")]
		[TestCase(3, 2, true, "+0.00")]
		[TestCase(17, 2, true, "0.000")]
		[TestCase(10, 2, true, "-12.00")]
		public void InvalidNumber_OnInvalidArguments(int precision, int scale, bool onlyPositive, string value)
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
				throw new ArgumentException("scale must be a non-negative number less than precision");
			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value)
		{
			if (string.IsNullOrEmpty(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			// Знак и целая часть
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}