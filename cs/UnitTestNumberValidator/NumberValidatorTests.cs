using System;
using FluentAssertions;
using HomeExercises;
using NUnit.Framework;

namespace UnitTestNumberValidator
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true, "precision must be a positive number", TestName = "Precision Is Negative")]
		[TestCase(2, -1, true, "precision must be a non-negative number less or equal than precision",
			TestName = "Scale Is Negative")]
		[TestCase(1, 2, true, "precision must be a non-negative number less or equal than precision",
			TestName = "Scale is More Precision")]
		[TestCase(2, 2, true, "precision must be a non-negative number less or equal than precision",
			TestName = "Scale is Equals Precision")]
		public void Init_ThrowWhen(int precision, int scale, bool onlyPositive, string errMessage)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);

			act.Should().Throw<ArgumentException>()
				.WithMessage(errMessage);
		}

		[TestCase(17, 2, true, TestName = "Usual parameters")]
		[TestCase(1, 0, true, TestName = "Scale is Zero")]
		public void Init_Сorrectly(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);

			act.Should().NotThrow();
		}

		[TestCase(17, 2, true, "0.0", ExpectedResult = true, TestName = "Input fractional number. Be True")]
		[TestCase(17, 2, true, "0", ExpectedResult = true, TestName = "Input integer. Be True")]
		[TestCase(4, 2, true, "+1.23", ExpectedResult = true, TestName = "Input plus sign. Be True")]
		[TestCase(4, 2, false, "-1.23", ExpectedResult = true, TestName = "Input Minus sign. Be True")]
		[TestCase(3, 2, true, "+0,0", ExpectedResult = true, TestName = "Input number with comma. Be True")]
		[TestCase(3, 2, true, "0.00", ExpectedResult = true, TestName = "IntPart equals precision. Be True")]
		[TestCase(3, 2, true, "+0.0", ExpectedResult = true,TestName = "IntPart with sign equals precision. Be True")]
		[TestCase(3, 2, true, "00.00", ExpectedResult = false, TestName = "IntPart more precision. Be False")]
		[TestCase(5, 2, true, "0.000", ExpectedResult = false, TestName = "FracPart more scale. Be False")]
		[TestCase(3, 2, true, "a.sd", ExpectedResult = false, TestName = "Input not number. Be False")]
		[TestCase(3, 2, true, "", ExpectedResult = false, TestName = "Input Empty. Be False")]
		[TestCase(3, 2, true, " ", ExpectedResult = false, TestName = "Input Space. Be False")]
		[TestCase(3, 2, true, ".", ExpectedResult = false, TestName = "Input Dot. Be False")]
		[TestCase(3, 2, true, null, ExpectedResult = false, TestName = "Input Null. Be False")]
		[TestCase(5, 2, true, "-2", ExpectedResult = false, TestName = "Input NegativeNumber. Be False")]
		public bool IsValidNumber_Test(int precision, int scale, bool onlyPositive, string value)
		{
			return  new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
		}
	}

}