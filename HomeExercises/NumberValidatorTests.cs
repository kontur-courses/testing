using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
	    [TestCase(3, 2, false, ".00")]
	    [TestCase(2, 1, false, ",0")]
	    public void IsValidNumber_ShouldBeFalse_IfCommaOrPointIsNotLeadByDigits(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
	    }

        [TestCase(3, 2, false, "-1.")]
	    [TestCase(2, 1, false, "0,")]
	    public void IsValidNumber_ShouldBeFalse_IfCommaOrPointIsNotFollowedByDigits(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
	    }

        [TestCase(3, 2, false, "1,23")]
	    [TestCase(2, 1, false, "0,0")]
	    public void IsValidNumber_ShouldBeTrue_IfCommaInsteadOfPoint(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
	    }

        [TestCase(3, 2, false, "1.23")]
	    [TestCase(2, 1, false, "0.0")]
	    public void IsValidNumber_ShouldBeTrue_IfPointExceedsPrecisionButDigitsDont(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
	    }

        [TestCase(20, 19, false, "-")]
	    [TestCase(20, 19, false, "+")]
	    public void IsValidNumber_ShouldBeFalse_ForBareSign(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
	    }

        [TestCase(20, 19, false, "-a.sd")]
	    [TestCase(20, 19, false, "+Sample.Text")]
	    public void IsValidNumber_ShouldBeFalse_ForNotNumbers(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
	    }

        [TestCase(3, 2, true, "00.00")]
	    [TestCase(1, 0, true, "00")]
	    public void IsValidNumber_ShouldBeFalse_IfPrecisionExceedsLimitWithLeadingZeroes(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
	    }

        [TestCase(4, 2, false, "-1.23")]
	    [TestCase(2, 0, true, "+0")]
	    public void IsValidNumber_ShouldBeTrue_IfPushingLimitsWithSign(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
	    }

        [TestCase(3, 2, true, "+1.23")]
	    [TestCase(1, 0, true, "+0")]
	    public void IsValidNumber_ShouldBeFalse_IfPrecisionExceedsLimitWithPlusSign(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
	    }

        [TestCase(3, 2, "-1.23")]
	    [TestCase(1, 0, "-0")]
	    public void IsValidNumber_ShouldBeFalse_IfPrecisionExceedsLimitWithMinusSign(int precision, int scale, string value)
	    {
	        new NumberValidator(precision, scale).IsValidNumber(value).Should().BeFalse();
	    }

        [TestCase(3, 2, true, "41.23")]
	    [TestCase(1, 0, true, "00")]
	    public void IsValidNumber_ShouldBeFalse_ForPositiveIfPrecisionExceedsLimit(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
	    }

        [TestCase(3, 2, true, "1.234")]
	    [TestCase(1, 0, true, "0.0")]
	    public void IsValidNumber_ShouldBeFalse_IfScaleExceedsLimit(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
	    }

        [TestCase(3, 2, true, "0.0")]
	    [TestCase(1, 0, true, "0")]
	    public void IsValidNumber_ShouldBeTrue_ForZeroIfOnlyPositive(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
	    }

        [TestCase(3, 2, true, "1.23")]
	    [TestCase(1, 0, true, "3")]
	    public void IsValidNumber_ShouldBeTrue_ForPositiveIfOnlyPositive(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
	    }

        [TestCase(3, 2, true, "-1.23")]
	    [TestCase(1, 0, true, "-3")]
	    public void IsValidNumber_ShouldBeFalse_ForNegativeIfOnlyPositive(int precision, int scale, bool onlyPositive, string value)
	    {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
        }

        [TestCase(-1, 2, true)]
	    [TestCase(-1, -2, true)]
        public void NumberValidator_ThrowsArgumentException_WhileConstructingFromNegativePrecison(int precision, int scale, bool onlyPositive)
	    {
	        Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
        }

	    [TestCase(7, 7, true)]
	    [TestCase(1, 1, true)]
        public void NumberValidator_ThrowsArgumentException_WhileConstructingFromPrecisonEqualToScale(int precision, int scale, bool onlyPositive)
	    {
	        Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
	    }

	    [TestCase(1, 2, true)]
	    [TestCase(3, 4, true)]
        public void NumberValidator_ThrowsArgumentException_WhileConstructingFromPrecisonLessThanScale(int precision, int scale, bool onlyPositive)
	    {
	        Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
	    }

        [TestCase(1, 0, true)]
        [TestCase(3, 1, true)]
        public void NumberValidator_DoesntThrowException_WhileConstructingFromNonNegativePrecisonGreaterThanNonNegativeScale(int precision, int scale, bool onlyPositive)
	    {
	        Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
        }

	    [TestCase(1, -2, true)]
	    [TestCase(3, -1, true)]
	    public void NumberValidator_ThrowsArgumentException_WhileConstructingFromNegativePrecisonAndNonNegativeScale(int precision, int scale, bool onlyPositive)
	    {
	        Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
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
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

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
