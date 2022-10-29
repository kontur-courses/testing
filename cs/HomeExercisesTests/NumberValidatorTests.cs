using System;
using FluentAssertions;
using NUnit.Framework;
using HomeExercises;

namespace HomeExercisesTests
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(0, TestName = "Zero precision", Category = "Precision")]
		[TestCase(-1, TestName = "Negative precision", Category = "Precision")]
		public void Constructor_ThrowsArgumentException_OnNonPositivePrecision(int precision)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, 2, true));
		}
		
		[TestCase(Category = "Precision")]
		public void Constructor_DoesNotThrow_OnRightPrecision()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		}

		[TestCase(1, TestName = "Scale >= precision", Category = "Scale")]
		[TestCase(-1, TestName = "Negative scale", Category = "Scale")]
		public void Constructor_ThrowsArgumentException_OnWrongScale(int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, scale, true));
		}
		
		[TestCase(0, TestName = "Scale == 0", Category = "Scale")]
		[TestCase(1, TestName = "Scale > 0 && scale < precision", Category = "Scale")]
		public void Constructor_DoesNotThrow_OnRightScale(int scale)
		{
			Assert.DoesNotThrow(() => new NumberValidator(2, scale, true));
		}
		
		[TestCase(3, 2, "12.34", ExpectedResult = false, TestName = "Precision: actual > expected", Category = "Precision")]
		[TestCase(2, 0, "01", ExpectedResult = true, TestName = "Leading zero is is accepted", Category = "Format")]
		[TestCase(1, 0, "01", ExpectedResult = false, TestName = "Precision: leading zero is included", Category = "Precision")]
		[TestCase(2, 0, "+12", ExpectedResult = false, TestName = "Precision: + sign is included if only positive", Category = "Format")]
		[TestCase(2, 0, "+12", false, ExpectedResult = false, TestName = "Precision: + sign is included if not only positive", Category = "Format")]
		[TestCase(3, 2, "-1.23", false, ExpectedResult = false, TestName = "Precision: - sign is included", Category = "Precision")]
		[TestCase(17, 1, "100.1", ExpectedResult = true, TestName = "Precision: expected > actual", Category = "Precision")]
		[TestCase(4, 2, "1.123", ExpectedResult = false, TestName = "Scale: actual > expected", Category = "Scale")]
		[TestCase(3, 2, "1.00", ExpectedResult = true, TestName = "Trailing zero is accepted", Category = "Format")]
		[TestCase(3, 1, "1.00", ExpectedResult = false, TestName = "Scale: trailing zero is included", Category = "Scale")]
		[TestCase(4, 2, "100.1", ExpectedResult = true, TestName = "Scale: expected > actual", Category = "Scale")]
		[TestCase(3, 2, "1.34", ExpectedResult = true, TestName = "Scale: fractional part length == scale", Category = "Scale")]
		[TestCase(3, 0, "146", ExpectedResult = true, TestName = "Scale: without fractional part", Category = "Scale")]
		[TestCase(3, 1, "-1.2", ExpectedResult = false, TestName = "Sign: negative input is not accepted when only positive", Category = "Format")]
		[TestCase(1, 0, "+1", ExpectedResult = false, TestName = "Sign: + is accepted if only positive", Category = "Format")]
		[TestCase(3, 1, "-1.7", false, ExpectedResult = true, TestName = "Sign: - is accepted if not only positive ones", Category = "Format")]
		[TestCase(3, 2, "=1.2", false, ExpectedResult = false, TestName = "Sign: only '+' and '-' can be used", Category = "Format")]
		[TestCase(2, 0, "=5", ExpectedResult = false, TestName = "Sign: only '+' and '-' can be used if only positive", Category = "Format")]
		[TestCase(4, 2, "+-1.2", false, ExpectedResult = false, TestName = "Sign: Can't enter several signs", Category = "Format")]
		[TestCase(4, 2, "-", false, ExpectedResult = false, TestName = "Sign: only - sign is not accepted", Category = "Format")]
		[TestCase(4, 2, "+", ExpectedResult = false, TestName = "Sign: only + sign is not accepted", Category = "Format")]
		[TestCase(2, 0, "-0", false, ExpectedResult = true, TestName = "Sign: - Zero is accepted", Category = "Format")]
		[TestCase(2, 0, "+0", ExpectedResult = true, TestName = "Sign: + Zero is accepted", Category = "Format")]
		[TestCase(3, 1, " 1.1", ExpectedResult = false, TestName = "Leading spaces is not accepted", Category = "Format")]
		[TestCase(4, 1, "1 1", ExpectedResult = false, TestName = "Separator is only '.' or ','", Category = "Format")]
		[TestCase(2, 1, "1.1 ", ExpectedResult = false, TestName = "Space after number is not accepted", Category = "Format")]
		[TestCase(4, 1, "- 1.2", false, ExpectedResult = false, TestName = "Space between - sign and number is not accepted", Category = "Format")]
		[TestCase(4, 1, "+ 1.2", false, ExpectedResult = false, TestName = "Space between + sign and number is not accepted", Category = "Format")]
		[TestCase(4, 2, "1s.2d", ExpectedResult = false, TestName = "Not a number", Category = "Format")]
		[TestCase(1, 0, "", ExpectedResult = false, TestName = "Empty string is invalid", Category = "Format")]
		[TestCase(1, 0, null, ExpectedResult = false, TestName = "Null string is invalid", Category = "Format")]
		[TestCase(1, 0, "1.", ExpectedResult = false, TestName = "Fractional part is followed by a delimiter", Category = "Format")]
		[TestCase(2, 1, ".0", ExpectedResult = false, TestName = "There is integer part is always exists", Category = "Format")]
		[TestCase(2, 1, "1,1", ExpectedResult = true, TestName = "',' is accepted instead of '.'", Category = "Format")]
		[TestCase(5, 4, "1.1.1", ExpectedResult = false, TestName = "Several separators is not accepted", Category = "Format")]
		[TestCase(3, 2, "1.1;", ExpectedResult = false, TestName = "Extraneous characters after the number", Category = "Format")]
		[TestCase(40, 2, "1000000000000000000000000000000000000000", ExpectedResult = true, TestName = "Large numbers are accepted", Category = "Format")]
		[TestCase(40, 39, "0.000000000000000000000000000000000000001", ExpectedResult = true, TestName = "Small numbers are accepted", Category = "Format")]
		public bool IsValidNumber_ExpectedResult_OnValue(int precision, int scale, string value, bool onlyPositive = true)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
		}
	}
}