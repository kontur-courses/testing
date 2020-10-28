using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(3, 2, true, "a.sd", ExpectedResult = false)]
		[TestCase(3, 2, true, "0.", ExpectedResult = false)]
		[TestCase(3, 2, true, ".0", ExpectedResult = false)]
		[TestCase(1, 0, true, "12", ExpectedResult = false)]
		[TestCase(1, 0, true, null, ExpectedResult = false)]
		[TestCase(2, 0, false, "-1", ExpectedResult = true)]
		[TestCase(3, 1, true, " 3.0", ExpectedResult = false)]
		[TestCase(3, 1, true, " ", ExpectedResult = false)]
		[TestCase(17, 2, true, "0", ExpectedResult = true)]
		[TestCase(17, 2, true, "0.0", ExpectedResult = true)]
		[TestCase(17, 2, true, "0,0", ExpectedResult = true)]
		[TestCase(3, 2, true, "00.00", ExpectedResult = false)]
		[TestCase(3, 2, false, "-0.00", ExpectedResult = false)]
		[TestCase(3, 2, false, "+0.00", ExpectedResult = false)]
		[TestCase(17, 2, true, "0.000", ExpectedResult = false)]
		[TestCase(4, 2, true, "+1.23", ExpectedResult = true)]
		[TestCase(4, 2, false, "-1.23", ExpectedResult = true)]
		public bool IsValidNumber(int precision, int scale, bool onlyPositive, string number)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number);
		}

		[TestCaseSource(typeof(ConstructorTestCases))]
		public void Constructor(int precision, int scale, bool onlyPositive, Type expectedException)
		{
			TestDelegate function = () => new NumberValidator(precision, scale, onlyPositive);
			Assert.Throws(expectedException, function);
		}
	}

	public class ConstructorTestCases : IEnumerable
	{
		private List<List<object>> cases = new List<List<object>>
		{
            new List<object> { -1, -3, true, typeof(ArgumentException) },
            new List<object> { 1, 2, true, typeof(ArgumentException) },
            new List<object> { 0, 0, true, typeof(ArgumentException) },
            new List<object> { 2, -1, true, typeof(ArgumentException) }
		};
		private Dictionary<int, string> descriptions = new Dictionary<int, string>()
		{
			{ 0, "Constructor_IsEmpty_PrecisionMustBeGreatThanScaleAndScaleMustBeGreatThanOrEqualZero" }
		};

		public IEnumerator GetEnumerator()
		{
			for (var i = 0; i < cases.Count; i++)
			{
				var testCase = cases[i];
				yield return GetTestCaseData(i, testCase.ToArray());
				testCase[2] = !(bool)testCase[2];
				yield return GetTestCaseData(i, testCase.ToArray());
			}
		}

		public TestCaseData GetTestCaseData(int index, object[] args)
		{
			var data = new TestCaseData(args);
            if (descriptions.ContainsKey(index))
                data.SetName(descriptions[index]);
            return data;
		}
	}
}
