using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;


namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase("00.0", "int part can't contain more then one zero in highest digit when number is zero")]
		[TestCase("01", "int part can't start with zero when numbers isn't a zero")]
		public void ShouldBeFalse_WhenTooManyZero_InHighestDigitOfIntPart(string value, string whyNumberIsIncorrectMsg)
		{
			var validator = new NumberValidator(4, 2, true);
			validator.IsValidNumber(value).Should().BeFalse("Because {0}", whyNumberIsIncorrectMsg);
		}

		[Test]
		public void Precision_ShouldConsiderMinusSign()
		{
			var validator = new NumberValidator(4, 0, true);
			var value = "-1024";
			validator.IsValidNumber(value).Should().BeFalse("Because the minus sign is taken into account");
		}

		[Test]
		public void Precision_ShouldIgnorePlusSign()
		{
			var validator = new NumberValidator(4, 0, true);
			var value = "+1024";
			validator.IsValidNumber(value).Should().BeTrue("Because plus sign is ignored");
		}

		[TestCase("1.25.0", "has more then one delimiter")]
		[TestCase("1abc", "isn't a number")]
		[TestCase("-5", "is negative when onlyPositive flag is up")]
		public void ShouldBeFalse_WhenIncorrectValue(string value, string whyNumberIsIncorrectMsg)
		{
			var validator = new NumberValidator(10, 5, true);
			validator.IsValidNumber(value).Should().BeFalse(
				"Because {0} {1}", value, whyNumberIsIncorrectMsg);
        }

        [Category("Validate Fractals Delimiter")]
        [TestCase("1.5")]
        [TestCase("1,5")]
        public void ShouldBeTrue_WhenDelimiterIsDotOrComma(string value)
        {
            var validator = new NumberValidator(5, 1, true);
            validator.IsValidNumber(value).Should().BeTrue("Because the dot and comma are correct");
        }

        [Category("Ignore leading and trailing whitespaces")]
        [TestCase("    123")]
        [TestCase("\t123")]
        [TestCase("\n123")]
		[TestCase("\t123\t")]
        public void ShouldTrim_WhenLeadingOrTrailingWhitespaces(string value)
        {
	        var validator = new NumberValidator(10, 1);
	        validator.IsValidNumber(value).Should().BeTrue(
		        "Because the leading and trailing whitespaces were cut off");
		}

        [Category("Plus-minus zero")]
		[TestCase("-0")]
		[TestCase("-0.0")]
		[TestCase("+0")]
		[TestCase("+0.0")]
		public void ShouldBeFalse_WhenPlusOrMinusZero(string value)
		{
			var validator = new NumberValidator(3,1);
			validator.IsValidNumber(value).Should().BeFalse("Because a zero with a sign is incorrect.");
		}

		[Category("Plus-minus zero")]
		[TestCase("-0.1")]
		[TestCase("-0.000000001")]
		public void ShouldBeTrue_WhenNotPlusOrMinusZero(string value)
		{
			var validator = new NumberValidator(15, 10);
			validator.IsValidNumber(value).Should().BeTrue("Because {0} isn't a zero", value);
		}

		[Category("Constructor")]
		[TestCase(1, 1)]
		[TestCase(4, 5)]
		[TestCase(2, -1)]
		public void ShouldThrow_WhenScaleIsNotCorrect(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>("Because scale = {0} is incorrect", scale);
		}

		[Category("Constructor")]
		[TestCase(0)]
		[TestCase(-1)]
		public void ShouldThrow_WhenPrecisionIsNotCorrect(int precision)
		{
			Action action = () => new NumberValidator(precision);
			action.Should().Throw<ArgumentException>("Because precision = {0} is incorrect", precision);
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
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

			if (string.IsNullOrEmpty(value))
				return false;

			value = value.Trim();

			if (IsValuePlusOrMinusZero(value))
				return false;

			if (HasValueTooManyZeroAtHighestDigit(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			var intPart = GetIntPartLength(match);
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}

		private int GetIntPartLength(Match match)
		{
			var plusSign = "+";
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			intPart = match.Groups[1].Value == plusSign ? intPart - 1 : intPart;

			return intPart;
		}

		private bool IsValuePlusOrMinusZero(string value)
		{
			var regex = new Regex(@"^([+-]{1})(0{1})(([.,]{1})(0*)$|$)");
			return regex.IsMatch(value);
		}

		private bool HasValueTooManyZeroAtHighestDigit(string value)
		{
			var regex = new Regex(@"^([+-])?(0+\d)");
			return regex.IsMatch(value);
		}
	}
}