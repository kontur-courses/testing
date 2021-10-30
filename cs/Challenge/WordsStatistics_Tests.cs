using System.Linq;
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

		private IWordsStatistics wordsStatistics;

		[SetUp]
		public void SetUp()
		{
			wordsStatistics = CreateStatistics();
		}

		[Test]
		public void GetStatistics_IsEmpty_AfterCreation()
		{
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void GetStatistics_ContainsItem_AfterAddition()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("abc", 1));
		}

		[Test]
		public void GetStatistics_ContainsManyItems_AfterAdditionOfDifferentWords()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("def");
			wordsStatistics.GetStatistics().Should().HaveCount(2);
		}


		[Test]
		public void GG()
		{
			wordsStatistics.AddWord("aaa");
			wordsStatistics.AddWord("aaaa");
			wordsStatistics.GetStatistics().First().Word.Should().Be("aaaa");
		}

		[Test]
		public void YY()
		{
			wordsStatistics.AddWord("aaa");
			wordsStatistics.AddWord("bbb");
			wordsStatistics.GetStatistics().First().Word.Should().Be("aaa");
		}
		
		[Test]
		public void EE()
		{
			wordsStatistics.GetStatistics().Should().HaveCount(0);

		}
	}
}