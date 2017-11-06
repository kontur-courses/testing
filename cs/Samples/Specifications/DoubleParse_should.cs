using System.Globalization;
using NUnit.Framework;

namespace Samples.Specifications
{
	[TestFixture]
	public class DoubleParse_should
	{
		[TestCase("123", ExpectedResult = 123, TestName = "integer")]
		[TestCase("1.1", ExpectedResult = 1.1, TestName = "fraction")]
		[TestCase("1.1e1", ExpectedResult = 1.1e1, TestName = "scientific with positive exp")]
		[TestCase("1.1e-1", ExpectedResult = 1.1e-1, TestName = "scientific with negative exp")]
		[TestCase("-0.1", ExpectedResult = -0.1, TestName = "negative fraction")]
		public double withInvariantCulture_parse(string input)
		{
			return double.Parse(input, CultureInfo.InvariantCulture);
		}
	}
}