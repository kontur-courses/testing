using System;
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
		public void GetStatistics_CheckIgnoreCaseC_AfterAddition()
		{
			wordsStatistics.AddWord("C");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("c", 1));
		}

		[Test]
		public void GetStatistics_CheckIgnoreCaseC2_AfterAddition()
		{
			wordsStatistics.AddWord("C");
			wordsStatistics.AddWord("c");
            wordsStatistics.GetStatistics().Should().Equal(new WordCount("c", 2));
		}

        /*[Test]
		public void GetStatistics_CheckIgnoreCaseE_AfterAddition()
		{
			var a = new WordCount("E", 1);
			wordsStatistics.AddWord("E");
			wordsStatistics.AddWord("e");
            wordsStatistics.GetStatistics().Should().NotEqual(new WordCount("E", 2));
		}*/

		[Test]
		public void AddWord_ThrowsNullExc()
		{
			Assert.Throws<ArgumentNullException>(()=>wordsStatistics.AddWord(word:null));
		}






        // Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
    }
}