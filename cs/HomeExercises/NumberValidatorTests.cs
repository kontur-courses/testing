using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 1, TestName = "Negative precision")]
		[TestCase(0, 1, TestName = "Zero precision")]
		[TestCase(1, 2, TestName = "Precision below scale")]
		[TestCase(10, 10, TestName = "Scale same as precision")]
		[TestCase(1, 0, true, false, TestName = "Valid does not throw")]
		public void ConstructorTests(
			int precision,
			int scale,
			bool onlyPositive = false,
			bool numberValidatorThrows = true)
		{
			void Construct() => new NumberValidator(precision, scale, onlyPositive);
			if (numberValidatorThrows)
				Assert.Throws<ArgumentException>(Construct);
			else
				Assert.DoesNotThrow(Construct);
		}


		private static IEnumerable<TestCaseData> PrepareTestCases()
		{
			var data = new[]
			{
				GetTestCases(ValidatorLittleFraction(), new[]
				{
					("0.000", false, "Excess fraction"),
					("0", true, "No fraction"),
					("0.0", true, "Less fraction"),
				}),
				GetTestCases(ValidatorSmallPrecision(), new[]
				{
					("00.00", false, "Excess overall length"),
					("+0.00", false, "Excess overall including positive sign"),
					("-0.00", false, "Excess overall including negative sign"),
					("0.00", true, "Exclude sign"),
					("a.sd", false, "Illegal characters"),
				}),
				GetTestCases(ValidatorCustomSign(), new[]
				{
					("+1.23", true, "Positive number"),
					("-1.23", false, "Negative when only positive"),
				}),
				GetTestCases(ValidatorCustomSign(false), new[]
				{
					("-1.23", true, "Negative number allowed"),
				}),
				GetTestCases(ValidatorSmallPrecision(), new[]
				{
					("", false, "Empty"),
				}),
			};
			return data.SelectMany(testCases => testCases);
		}

		private static IEnumerable<TestCaseData> GetTestCases(
			NumberValidator validator,
			IEnumerable<(string number, bool isValidExpected, string testName)> testData)
		{
			return testData.Select(data =>
			{
				var (number, isValidExpected, testName) = data;
				var testCase = new TestCaseData(validator, number, isValidExpected) {TestName = testName};
				return testCase;
			});
		}

		private static NumberValidator ValidatorSmallPrecision()
			=> new NumberValidator(3, 2, true);

		private static NumberValidator ValidatorLittleFraction()
			=> new NumberValidator(17, 2, true);

		private static NumberValidator ValidatorCustomSign(bool onlyPositive = true)
			=> new NumberValidator(4, 2, onlyPositive);


		[TestCaseSource(nameof(PrepareTestCases))]
		public void TestNumberValidation(NumberValidator sut, string input, bool isValid)
		{
			var actual = sut.IsValidNumber(input);

			Assert.That(actual, Is.EqualTo(isValid), $"Validation of '{input}' was not correct");
		}
	}
}