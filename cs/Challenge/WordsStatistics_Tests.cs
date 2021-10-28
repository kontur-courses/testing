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
		public void AddWord_ShouldThrowNullException_AddNullWord()
		{
			Action act = () => wordsStatistics.AddWord(null);
			act.Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void AddWord_Ignore_IfWhiteSpace()
		{
			wordsStatistics.AddWord(" ");
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}


		[Test]
		public void AddWord_AddFullWord_IfWordLengthLessThanTen()
		{
			wordsStatistics.AddWord("123456");
			wordsStatistics.GetStatistics().Should().Contain(new WordCount("123456", 1));
		}

		[Test]
		public void AddWord_AddWhitespaces_IfWordContainsTenWhitespacesAndChar()
		{
			wordsStatistics.AddWord("          0");
			wordsStatistics.GetStatistics().Should().Contain(new WordCount("          ", 1));
		}

		[Test]
		public void AddWord_CorrectCalculate_AfterDoubleAddition()
		{
			wordsStatistics.AddWord("a");
			wordsStatistics.AddWord("a");

			wordsStatistics.GetStatistics().Should().Contain(new WordCount("a", 2));
		}


		[Test]
		public void AddWord_AddWordInLowerCase_AfterAdditionUpperCaseWord()
		{
			wordsStatistics.AddWord("A");

			wordsStatistics.GetStatistics().Should().Contain(new WordCount("a", 1));
		}


		[Test]
		public void GetStatistics_ReturnsSortedByWords_IfSameCounts()
		{
			wordsStatistics.AddWord("b");
			wordsStatistics.AddWord("b");
			wordsStatistics.AddWord("c");
			wordsStatistics.AddWord("c");
			wordsStatistics.AddWord("a");

			var firstElem = wordsStatistics.GetStatistics().First();

			firstElem.Count.Should().Be(2);
			firstElem.Word.Should().Be("b");
		}


		[Test]
		public void GetStatistics_ReturnsWordCount()
		{
			wordsStatistics.AddWord("a");

			wordsStatistics.GetStatistics().First().Should().BeOfType<WordCount>();
		}

		[Test, Timeout(1000)]
		public void AddWord_ShouldWorkOnSecond_IfAddLotOfDifferentWords()
		{
			for (var i = 0; i < 1000000; i++)
			{
				wordsStatistics.AddWord(i.ToString());
			}
		}
		[Test, Timeout(10000)]
		public void GetStat_ShouldWorkOnSecond_IfAddLotOfDifferentWords()
		{
			for (var i = 0; i < 1000000; i++)
			{
				wordsStatistics.AddWord(i.ToString());
			}

			wordsStatistics.GetStatistics().Count().Should().Be(1000000);
		}
		
		/*
		[Test]
		public void AddWord_ShouldWorkOnSecond_IfAddLotOfSameWords()
		{
			for (var i = 0; i < 10000; i++)
			{
				wordsStatistics.AddWord("a");
				wordsStatistics.AddWord("b");
			}

			wordsStatistics.GetStatistics().Count().Should().Be(2);
			wordsStatistics.GetStatistics().First().Count.Should().Be(10000);
		}*/
		[Test]
		public void GetStatistics_should_beEmpty_ifClear()
		{
			wordsStatistics.AddWord("a");
			wordsStatistics.GetStatistics().Count().Should().Be(1);
			wordsStatistics.AddWord("b");
			var wordsStatistics1 = new WordsStatistics();
			wordsStatistics.GetStatistics().Count().Should().Be(2);
		}
		[Test]
		public void AddWord_ShouldCountCorrectly_ifAddLotOfDifferentWords()
		{
			for (var i = 0; i < 10000; i++)
			{
				wordsStatistics.AddWord(i.ToString());
			}

			wordsStatistics.GetStatistics().Count().Should().Be(10000);
		}

		/*[Test]
		public void AddWord_ShouldWorkOnSecond_IfAddLotOfSamWords()
		{
			for (var i = 0; i < 10000; i++)
			{
				wordsStatistics.AddWord((i % 5000).ToString());
			}
			
			wordsStatistics.GetStatistics().Count().Should().Be(5000);
			wordsStatistics.GetStatistics().First().Count.Should().Be(2);
			wordsStatistics.GetStatistics().Last().Count.Should().Be(2);
		}*/


		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	}
}