using System;
using System.Collections.Generic;
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
			bool throws = true)
		{
			// ReSharper disable once ObjectCreationAsStatement
			void Construct() => new NumberValidator(precision, scale, onlyPositive);
			if (throws)
				Assert.Throws<ArgumentException>(Construct);
			else
				Assert.DoesNotThrow(Construct);
		}


		public static IEnumerable<TestCaseData> PrepareTestCases()
		{
			var data = new[]
			{
				GetTestCases(ValidatorLittleFraction, new[]
				{
					("0.000", false, "Excess fraction"),
					("0", true, "No fraction"),
					("0.0", true, "Less fraction"),
				}),
				GetTestCases(ValidatorSmallPrecision, new[]
				{
					("00.00", false, "Excess overall length"),
					("+0.00", false, "Excess overall including positive sign"),
					("-0.00", false, "Excess overall including negative sign"),
					("0.00", true, "Exclude sign"),
					("a.sd", false, "Illegal characters"),
				}),
				GetTestCases(() => ValidatorCustomSign(), new[]
				{
					("+1.23", true, "Positive number"),
					("-1.23", false, "Negative when only positive"),
				}),
				GetTestCases(() => ValidatorCustomSign(false), new[]
				{
					("-1.23", true, "Negative number allowed"),
				}),
				GetTestCases(ValidatorSmallPrecision, new[]
				{
					("", false, "Empty"),
				}),
			};
			foreach (var testCases in data)
			{
				foreach (var testCase in testCases)
				{
					yield return testCase;
				}
			}
		}

		private static IEnumerable<TestCaseData> GetTestCases(
			Func<NumberValidator> getValidator,
			IEnumerable<(string number, bool isValidExpected, string testName)> dataTuples)
		{
			foreach (var (number, isValidExpected, testName) in dataTuples)
			{
				var sut = getValidator();
				var testData = new TestCaseData(sut, number, isValidExpected) {TestName = testName};
				yield return testData;
			}
		}

		private static NumberValidator ValidatorSmallPrecision()
			=> new NumberValidator(3, 2, true);

		private static NumberValidator ValidatorLittleFraction()
			=> new NumberValidator(17, 2, true);

		private static NumberValidator ValidatorCustomSign(bool onlyPositive = true)
			=> new NumberValidator(4, 2, onlyPositive);


		[TestCaseSource(nameof(PrepareTestCases))]
		public void IsValidTests(NumberValidator sut, string input, bool isValid)
		{
			var actual = sut.IsValidNumber(input);

			Assert.That(actual, Is.EqualTo(isValid), $"Validation of '{input}' was not correct");
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