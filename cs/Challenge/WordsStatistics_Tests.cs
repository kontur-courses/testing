using System;
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
		public void GetStatistics_ShouldBeOrderByCount_AfterAddition()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("def");
			wordsStatistics.AddWord("def");
			
			wordsStatistics.GetStatistics().Select(x => x.Word).Should().ContainInOrder("def","abc");
		}

		[Test]
		public void GetStatistics_ShouldBeInLexicographicOrder_AfterAddition()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("def");

			wordsStatistics.GetStatistics().Select(x => x.Word).Should().ContainInOrder("abc", "def");
		}

		[Test]
		public void GetStatistics_ShouldOrderCaseInsensetive_AfterAddition()
		{
			wordsStatistics.AddWord("DEF");
			wordsStatistics.AddWord("abc");

			wordsStatistics.GetStatistics().Select(x => x.Word).Should().ContainInOrder("abc", "def");
		}

		[Test]
		public void AddWord_ShouldThrow_NullArgumentException_AfterAddition_Null()
		{
			wordsStatistics.Invoking(x => x.AddWord(null)).Should().Throw<ArgumentNullException>();
		}
		[Test]
		public void AddWord_ShouldNotThrow_NullArgumentException_AfterAddition_WhiteSpace()
		{
			wordsStatistics.Invoking(x => x.AddWord(null)).Should().NotThrow<ArgumentNullException>();
		}

        [Test]
		public void AddWord_ShouldNotAddWhiteSpace()
		{
			wordsStatistics.AddWord(" ");
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void AddWord_ShouldIncreaseCount()
		{
			wordsStatistics.AddWord("a");
			wordsStatistics.GetStatistics().Where(x => x.Word == "a").Select(x => x.Count).Should().BeEquivalentTo(1);
		}
		[Test]
        public void AddWord_ShouldTruncToTenWhenLargerThenTen()
		{
			wordsStatistics.AddWord("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
			wordsStatistics.GetStatistics().Select(x => x.Word).First().Should().BeEquivalentTo("aaaaaaaaaa");
		}
		[Test]
        public void AddWord_ShouldTruncWhenBetweenFiveAndTen()
		{
			wordsStatistics.AddWord("aaaaaa");
			wordsStatistics.GetStatistics().Select(x => x.Word).First().Should().BeEquivalentTo("aaaaaa");
		}


        // Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
    }
}