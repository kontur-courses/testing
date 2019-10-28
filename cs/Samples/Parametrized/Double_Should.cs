using System.Collections;
using System.Globalization;
using NUnit.Framework;

namespace Samples.Parametrized
{
	[TestFixture]
	public class Double_Should
	{
		[Test]
		[TestCaseSource(nameof(DivideTestCases))]
		public double Divide(double a, double b)
		{
			return a / b;
		}

		public static IEnumerable DivideTestCases
		{
			get
			{
				yield return new TestCaseData(12.0, 3.0).Returns(4);
				yield return new TestCaseData(12.0, 2.0).Returns(6);
				yield return new TestCaseData(12.0, 4.0).Returns(3);
			}
		}

		[TestCase("123", ExpectedResult = 123, TestName = "integer")]
		[TestCase("1.1", ExpectedResult = 1.1, TestName = "fraction")]
		[TestCase("1.1e1", ExpectedResult = 1.1e1, TestName = "scientific with positive exp")]
		[TestCase("1.1e-1", ExpectedResult = 1.1e-1, TestName = "scientific with negative exp")]
		[TestCase("-0.1", ExpectedResult = -0.1, TestName = "negative fraction")]
		public double Parse_WithInvariantCulture(string input)
		{
			return double.Parse(input, CultureInfo.InvariantCulture);
		}
	}
}