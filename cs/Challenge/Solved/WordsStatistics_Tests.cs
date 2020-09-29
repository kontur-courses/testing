using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Challenge.Solved
{
	[TestFixture]
	public class WordsStatistics_Tests
	{
		public virtual IWordsStatistics CreateStatistics()
		{
			// меняется на разные реализации при запуске exe
			return new Challenge.WordsStatistics();
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
		public void GetStatistics_SortsWordsByFrequency()
		{
			wordsStatistics.AddWord("aaaaaaaaaa");
			wordsStatistics.AddWord("bbbbbbbbbb");
			wordsStatistics.AddWord("bbbbbbbbbb");
			wordsStatistics.GetStatistics().Select(t => t.Word)
				.Should().BeEquivalentTo(new[] {"bbbbbbbbbb", "aaaaaaaaaa"},
					options => options.WithStrictOrdering());
		}

		[Test]
		public void GetStatistics_SortsWordsByAbc_WhenFrequenciesAreSame()
		{
			wordsStatistics.AddWord("cccccccccc");
			wordsStatistics.AddWord("aaaaaaaaaa");
			wordsStatistics.AddWord("bbbbbbbbbb");
			wordsStatistics.GetStatistics().Select(t => t.Word)
				.Should().ContainInOrder("aaaaaaaaaa", "bbbbbbbbbb", "cccccccccc");
		}

		[Test]
		public void GetStatistics_ReturnsSameResult_OnSecondCall()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("abc", 1));
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("abc", 1));
		}

		[Test]
		public void GetStatistics_BuildsResult_OnEveryCall()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.GetStatistics().Should().HaveCount(1);
			wordsStatistics.AddWord("def");
			wordsStatistics.GetStatistics().Should().HaveCount(2);
		}

		[Test]
		public void AddWord_AllowsShortWords()
		{
			wordsStatistics.AddWord("aaa");
		}

		[Test]
		public void AddWord_CountsOnce_WhenSameWord()
		{
			wordsStatistics.AddWord("aaaaaaaaaa");
			wordsStatistics.AddWord("aaaaaaaaaa");
			wordsStatistics.GetStatistics().Should().HaveCount(1);
		}

		[Test]
		public void AddWord_IncrementsCounter_WhenSameWord()
		{
			wordsStatistics.AddWord("aaaaaaaaaa");
			wordsStatistics.AddWord("aaaaaaaaaa");
			wordsStatistics.GetStatistics().Select(t => t.Count)
				.Should().BeEquivalentTo(2);
		}

		[Test]
		public void AddWordThrows_WhenWordIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => wordsStatistics.AddWord(null));
		}

		[Test]
		public void AddWord_Ignores_EmptyWord()
		{
			wordsStatistics.AddWord("");
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void AddWord_Ignores_WhitespaceWord()
		{
			wordsStatistics.AddWord(" ");
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void AddWord_CutsWords_LongerThan10()
		{
			wordsStatistics.AddWord("12345678901");
			wordsStatistics.GetStatistics().Select(t => t.Word)
				.Should().BeEquivalentTo("1234567890");
		}

		[Test]
		public void AddWord_CutsWordsJoined()
		{
			wordsStatistics.AddWord("12345678901");
			wordsStatistics.AddWord("1234567890");
			wordsStatistics.GetStatistics().Select(t => t.Count)
				.Should().BeEquivalentTo(2);
		}

		[Test]
		public void AddWord_AllowsWordAndCutItToWhitespaces_WhenWordPrecededByWhitespaces()
		{
			wordsStatistics.AddWord("          a");
			wordsStatistics.GetStatistics().Select(t => t.Word)
				.Should().BeEquivalentTo("          ");
		}

		[Test]
		public void AddWord_IsCaseInsensitive()
		{
			var counter = 0;
			for (char c = 'a'; c <= 'z'; c++)
			{
				wordsStatistics.AddWord(c.ToString());
				wordsStatistics.AddWord(c.ToString().ToUpper());
				counter++;
			}
			for (char c = 'а'; c <= 'я' || c <= 'ё'; c++)
			{
				wordsStatistics.AddWord(c.ToString());
				wordsStatistics.AddWord(c.ToString().ToUpper());
				counter++;
			}
			wordsStatistics.GetStatistics().Should().HaveCount(counter);
		}

		[Test]
		public void AddWord_HasNoCollisions()
		{
			const int wordCount = 500;
			for (int i = 0; i < wordCount; i++)
			{
				wordsStatistics.AddWord(i.ToString());
			}
			wordsStatistics.GetStatistics().Should().HaveCount(wordCount);
		}

		[Test, Timeout(1500)]
		public void AddWord_HasSufficientPerformance_OnAddingDifferentWords()
		{
			for (int i = 0; i < 5000; i++)
			{
				wordsStatistics.AddWord(i.ToString());
			}
			wordsStatistics.GetStatistics();
		}

		[Test, Timeout(1500)]
		public void AddWord_HasSufficientPerformance_OnAddingSameWord()
		{
			for (int i = 0; i < 20000; i++)
			{
				wordsStatistics.AddWord(i.ToString());
			}
			var sameWord = 9.ToString();
			for (int i = 0; i < 20000; i++)
			{
				wordsStatistics.AddWord(sameWord);
			}
			wordsStatistics.GetStatistics();
		}

		[Test]
		public void SupportsSeveralInstances()
		{
			var anotherStatistics = CreateStatistics();
			wordsStatistics.AddWord("aaaaaaaaaa");
			anotherStatistics.AddWord("bbbbbbbbbb");
			wordsStatistics.GetStatistics().Should().HaveCount(1);
		}
	}
}