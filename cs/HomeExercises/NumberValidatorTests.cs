using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 0, true,TestName = "When precision < 0")]
		[TestCase(0, TestName = "When precision == 0")]
		[TestCase(2, -1, TestName = "When scale < 0, onlyPositive don't matter")]
		[TestCase(2, 2, false, TestName = "When scale == precision")]
		[TestCase(2, 3, true, TestName = "When scale > precision")]
		public void СreateValidator_ThrowArgumentException_FlagOnlyPositiveDoesNotMatter(int precision, int scale = 0, 
			bool onlyPositive = false)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
		}
		
		[TestCase("", false,17, 2, 
			TestName = "False when value is empty")]
		[TestCase(null, false,2, 1, 
			TestName = "False when value is null")]
		[TestCase("aaa", false,5, 1, 
			TestName = "False when value consists only of letters")]
		[TestCase("\n\t\r", false,5, 1, 
			TestName = "False when value consists only of special symbols")]
		[TestCase("+aa", false,5, 2, 
			TestName = "False when value consists of + and letters")]
		[TestCase("-aa", false,5, 2, 
			TestName = "False when value consists of - and letters")]
		[TestCase("+1.0aa", false,5, 2, 
			TestName = "False when value consists of +, number and letters at the end")]
		[TestCase("ff+1.0", false,5, 2, 
			TestName = "False when value consists of letters at the beginning, + and number")]
		[TestCase("., -1.0.", false,5, 2, 
			TestName = "False when value consists of white space or other symbols, - and number")]
		[TestCase("-1.0", true,5, 2, 
			TestName = "True when value consists of -, number and . between int and fracPart, onlyPositive is false")]
		[TestCase("+1.0", true,5, 2, 
			TestName = "True when value consists of +, number and . between int and fracPart, onlyPositive is false")]
		[TestCase("-1,0", true,5, 2, 
			TestName = "True when value consists of -, number and , between int and fracPart, onlyPositive is false")]
		[TestCase("-12.00", false,3, 2, 
			TestName = "False when int+fracPart > precision, onlyPositive don't matter")]
		[TestCase("-14.00", false,4, 1, 
			TestName = "False when fracPart > scale, onlyPositive don't matter")]
		[TestCase("+55555.00", true,17, 2, true, 
			TestName = "True when value consists of + and onlyPositive is true")]
		[TestCase("55555.00", true,17, 2, true, 
			TestName = "True when value doesn't consist of + or - and onlyPositive is true")]
		[TestCase("55555.00", true,17, 2, 
			TestName = "True when value doesn't consist of + or - and onlyPositive is false")]
		[TestCase("-55555.00", true,17, 2, 
			TestName = "True when value consists of - and onlyPositive is false")]
		[TestCase("-55555.00", false,17, 2, true,
			TestName = "False when value consists of - and onlyPositive is true")]
		[TestCase("+0.12", true,17, 2, 
			TestName = "True when int+fracPart <= precision and positive is false")]
		[TestCase("+55.010", true,17, 3, true, 
			TestName = "True when int+fracPart < precision and " + "positive is true")]
		public void CheckValidNumber_TrueOrFalse(string value, bool actualResult, 
			int precision, int scale = 0, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			var expectedResult = validator.IsValidNumber(value);
			
			Assert.AreEqual(actualResult, expectedResult);
		}
	}
}