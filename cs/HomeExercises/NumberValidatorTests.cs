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
		[TestCase(null, 5, 3, false, TestName = "number is null")]
		[TestCase("", 5, 3, false, TestName = "empty number")]
		[TestCase("    ", 5, 3, false, TestName = "only whitespace")]
		[TestCase("abc", 5, 3, false, TestName = "letters instead of numbers")]
		[TestCase("aa.bc", 5, 3, false, TestName = "letters instead of numbers with separator")]

		[TestCase("?0.00", 5, 3, false, TestName = "starts with special symbol '?' ")]
		[TestCase("!0.00", 5, 3, false, TestName = "starts with special symbol '!' ")]
		[TestCase("@0.00", 5, 3, false, TestName = "starts with special symbol '@' ")]
		[TestCase("^0.00", 5, 3, false, TestName = "starts with special symbol '^' ")]
		[TestCase("#0.00", 5, 3, false, TestName = "starts with special symbol '#' ")]
		[TestCase("$0.00", 5, 3, false, TestName = "starts with special symbol '$' ")]
		[TestCase("%0.00", 5, 3, false, TestName = "starts with special symbol '%' ")]
		[TestCase("&0.00", 5, 3, false, TestName = "starts with special symbol '&' ")]
		[TestCase("*0.00", 5, 3, false, TestName = "starts with special symbol '*' ")]
		[TestCase("№0.00", 5, 3, false, TestName = "starts with special symbol '№' ")]

		[TestCase("0 12", 5, 3, false, TestName = "separator is whitespace")]
		[TestCase("0:12", 5, 3, false, TestName = "separator is :")]
		[TestCase("0-12", 5, 3, false, TestName = "separator is -")]
		[TestCase("0.12.0", 5, 3, false, TestName = "second separator")]

		[TestCase(" 12.34", 5, 3, false, TestName = "starts with whitespace")]
		[TestCase("12.34 ", 5, 3, false, TestName = "end with space")]

		[TestCase("-12.345", 5, 3, false, TestName = "length number with '-' > precision")]
		[TestCase("+12.345", 5, 3, false, TestName = "length number with '+' > precision")]
		[TestCase("123.456", 5, 3, false, TestName = "integer length + fraction length > precision")]
		[TestCase("1.2345", 5, 3, false, TestName = "fraction length > scale")]
		[TestCase("-12.34", 5, 3, true, TestName = "negative number with onlyPositive")]
		public void Test_IsValidNumber_ShouldBeFalse(string number, int precision, int scale, bool onlyPositive)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeFalse();
		}


		[TestCase("12,345", 5, 3, false, TestName = "separator ,")]
		[TestCase("12.345", 5, 3, false, TestName = "separator .")]
		[TestCase("1.234", 5, 3, false, TestName = "number is`n full length")]
		[TestCase("12", 5, 3, false, TestName = "number without fraction part")]
		[TestCase("12.34", 5, 3, false, TestName = "fraction part Length < scale")]
		[TestCase("0", 5, 3, false, TestName = "scale is zero")]

		[TestCase("+12.34", 5, 3, true, TestName = "positive number with onlyPositive")]
		[TestCase("+12.34", 5, 3, false, TestName = "positive number without onlyPositive")]
		[TestCase("-12.34", 5, 3, false, TestName = "negative number")]
		public void Test_IsValidNumber_ShouldBeTrue(string number, int precision, int scale, bool onlyPositive)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeTrue();
		}


		[TestCase(-1, 2, true, TestName = "negative precision With OnlyPositive")]
		[TestCase(-1, 2, false, TestName = "negative precision without OnlyPositive")]
		[TestCase(2, 3, false, TestName = "scale > precision")]
		[TestCase(1, -1, false, TestName = "negative scale without OnlyPositive")]
		[TestCase(1, -1, true, TestName = "negative scale with OnlyPositive")]
		[TestCase(5, 5, true, TestName = "precision == scale")]
		[TestCase(0, 0, true, TestName = "precision and scale are zero")]
		public void Test_Initialization_ShouldBeThrownException(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}
	}
}