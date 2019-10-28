using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

		//[Test]
		//public void GetStatistics_ContainsManyItems_AfterAdditionOfDifferentWords()
		//{
		//	wordsStatistics.AddWord("abc");
		//	wordsStatistics.AddWord("def");
		//	wordsStatistics.GetStatistics().Should().HaveCount(2);
		//}

		[Test]
		public void GetStatistics_ContainsSameItem_AfterLowerCaseAddition()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("ABC");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("abc", 2));
		}

		[Test]
		public void GetStatistics_ThrowsArgumentNullException_IfNull()
		{
			Action action = () =>
			{
				wordsStatistics.AddWord(null);
			};
			action.ShouldThrow<ArgumentNullException>();
		}

		[Test]
		public void GetStatistics_NoItems_IfWhitespace()
		{
			wordsStatistics.AddWord(" ");
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void GetStatistics_ContainsTwoWords_IfTheyAreLong()
		{
			wordsStatistics.AddWord("qwertyuiopa");
			wordsStatistics.AddWord("qwertyuiopz");
			wordsStatistics.GetStatistics().Should().HaveCount(1);
		}

		[Test]
		public void GetStatistics_LexicographicalOrder_IfSameFrequency()
		{
			wordsStatistics.AddWord("efd");
			wordsStatistics.AddWord("efd");
            wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("abc");
			wordsStatistics.GetStatistics().ShouldAllBeEquivalentTo(
				new[] { new WordCount("abc", 2), new WordCount("efd", 2) }, options => options.WithStrictOrdering());
		}

        [Test]
		public void GetStatistics_StorageAllElements_IfBigAmount()
		{
			for (var i = 0; i < 1000; i++)
				wordsStatistics.AddWord(i.ToString());
			wordsStatistics.GetStatistics().Should().HaveCount(1000);
		}

		[Test]
		public void GetStatistics_NotContain_RussianWords()
		{
			wordsStatistics.AddWord("Хай");
			wordsStatistics.GetStatistics().Should().NotContain(new WordCount("Хай", 1));
		}

        // Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
    }
}