using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(null, TestName = "Number is null")]
		[TestCase("", TestName = "emptyNumber")]
		[TestCase("    ",TestName = "only whiteSpace")]
		[TestCase("abc",TestName = "letters instead of numbers")]
		[TestCase("aa.bc", TestName = "letters instead of numbers with separator")]
		[TestCase("?0.00", TestName = "startsWith special symbol '?' ")]
		[TestCase("!0.00", TestName = "startsWith special symbol '!' ")]
		[TestCase("@0.00", TestName = "startsWith special symbol '@' ")]
		[TestCase("^0.00", TestName = "startsWith special symbol '^' ")]
		[TestCase("#0.00", TestName = "startsWith special symbol '#' ")]
		[TestCase("$0.00", TestName = "startsWith special symbol '$' ")]
		[TestCase("%0.00", TestName = "startsWith special symbol '%' ")]
		[TestCase("0 12", TestName = "separator is whiteSpace")]
		[TestCase("0:12", TestName = "separator is :")]
		[TestCase("0-12", TestName = "separator is -")]
		[TestCase("0.12.0", TestName = "second separator")]
		[TestCase("-12.345", TestName = "length number with '-' > precision")]
		[TestCase("+12.345", TestName = "length number with '+' > precision")]
		[TestCase("123.456", TestName = "intLength + fracLength > precision")]
		[TestCase("1.2345", TestName = "fracLength > scale")]
		[TestCase("-1.23", true, TestName = "negative number with onlyPositive")]

		public void Test_IsValidNumber_ShouldBeFalse(string number, bool onlyPositive = false, int precision= 5, int scale =3)
		{
			 new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeFalse();
		}


		[TestCase("12,345", TestName = "Separator .")]
		[TestCase("12.345", TestName = "Separator ,")]
		[TestCase("1.234", TestName = "number is`n full length")]
		[TestCase("12", TestName = "number without fraction part")]
		[TestCase("12.34", TestName = "fraction part Length < scale")]
		[TestCase("0", false, 1, 0, TestName = "scale is zero")]
		[TestCase("+12.34", true, TestName = "positive number with onlyPositive")]
		[TestCase("+12.34", TestName = "positive number without onlyPositive")]
		[TestCase("-12.34", TestName = "negative number")]
		public void Test_IsValidNumber_ShouldBeTrue(string number, bool onlyPositive = false, int precision=5, int scale=3)
		{
			 new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeTrue();
		}


		[TestCase(-1, 2, true, TestName = "negative Precision With OnlyPositive")]
		[TestCase(-1, 2, false, TestName = "negative Precision without OnlyPositive")]
		[TestCase(2, 3, false, TestName = "scale > precision")]
		[TestCase(1, -1, false, TestName = "negative scale without OnlyPositive")]
		[TestCase(1, -1, true, TestName = "negative scale with OnlyPositive")]
		[TestCase(5, 5, true, TestName = "precision == scale")]
		[TestCase(0, 0, true, TestName ="precision and scale are zero")]
		public void Test_Initialization_ShouldBeThrownException(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}
	}
}