using NUnit.Framework;

namespace Samples.Specifications
{
    public class Summator
    {
        public int Sum(int a, int b)
        {
            return a + b;
        }
    }

    [TestFixture]
    public class Summator_Should
    {
        [Test]
        public void DoSomething_WhenSomething()
        {

        }
    }

}