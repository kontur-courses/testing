using System.Collections;
using NUnit.Framework;

namespace Samples.Specifications
{
    [TestFixture]
    public class Integer_should
    {
        [Test, TestCaseSource("DivideTestCases")]
        public int divide(int n, int d)
        {
            return n / d;
        }

        public static IEnumerable DivideTestCases
        {
            get
            {
                yield return new TestCaseData(12, 3).Returns(4);
                yield return new TestCaseData(12, 2).Returns(6);
                yield return new TestCaseData(12, 4).Returns(3);
            }
        }
    }
}