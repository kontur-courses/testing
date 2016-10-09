using FluentAssertions;
using NUnit.Framework;

namespace Challenge
{
	[TestFixture]
	public class WordsStatistics_Tests
	{
		public virtual IWordsStatistics CreateStatistics()
		{
			// меняется на разные реализации при запуске exe
			return new WordsStatistics();
		}

	    private IWordsStatistics statistics;

		[SetUp]
		public void SetUp()
		{
			statistics = CreateStatistics();
		}

		[Test]
		public void GetStatistics_IsEmpty_AfterCreation()
		{
			statistics.GetStatistics().Should().BeEmpty();
		}

        [Test]
		public void AddWord_CountsOnce_WhenSameWord()
		{
			statistics.AddWord("aaaaaaaaaa");
			statistics.AddWord("aaaaaaaaaa");
			statistics.GetStatistics().Should().HaveCount(1);
		}
        
        // FluentAssetions docs: http://www.fluentassertions.com/
        // AssertionOptions.AssertEquivalencyUsing(options => options.WithStrictOrdering());
    }
}