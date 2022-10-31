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
		public void AddWord_ThrowArgumentNullException_WhenNull()
		{
			Action act = ()=>wordsStatistics.AddWord(null);
			act.Should().Throw<ArgumentNullException>();
		}

		[TestCase("")]
		[TestCase(" ")]
		//[TestCase(@"\u2000")]
		//[TestCase(@"\t")]
		public void AddWord_Return_WhenWhitespace(string input)
		{
			wordsStatistics.AddWord(input);
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}
        [Test]
        public void AddWord_Trim10_WhenMore10()
        {
            string input = "abcdefghijk";
            wordsStatistics.AddWord(input);
            wordsStatistics.GetStatistics().Should().Equal(new WordCount("abcdefghij", 1));
            wordsStatistics.GetStatistics().ElementAt(0).Word.Length.Should().Be(10);
        }

        [Test]
        public void AddWord_WhenMore5()
        {
	        string input = "abcdef";
	        wordsStatistics.AddWord(input);
	        wordsStatistics.GetStatistics().ElementAt(0).Word.Length.Should().Be(6);
        }

        [Test]
        public void AddWord_CountMore1237()
        {
	        for (int i = 0; i < 1238; i++)
	        {
				wordsStatistics.AddWord(i.ToString());
			}

	        wordsStatistics.GetStatistics().Should().HaveCount(1238);
        }

		[Test]
        public void AddWord_Lower_WhenBeLarge()
        {
	        string input = "ABCD";
	        wordsStatistics.AddWord(input);
	        wordsStatistics.GetStatistics().Should().Equal(new WordCount("abcd", 1));
        }

        [Test]
        public void AddWord_Count2_When2Same2Word()
        {
	        string input = "ABCD";
	        wordsStatistics.AddWord(input);
	        wordsStatistics.AddWord(input);
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("abcd", 2));
        }

        [Test]
        public void GetStatistics_AscendingSortWord()
        {
	        wordsStatistics.AddWord("a");
	        wordsStatistics.AddWord("ab");
			wordsStatistics.AddWord("abc");
			wordsStatistics.GetStatistics().Select(w => w.Word).Should().BeInAscendingOrder();
        }

        [Test]
        public void GetStatistics_DescendingSortCount()
        {
	        wordsStatistics.AddWord("a");
	        wordsStatistics.AddWord("abc");
	        wordsStatistics.AddWord("abc");
	        wordsStatistics.GetStatistics().Select(w => w.Count).Should().BeInDescendingOrder();
        }

        [Test]
        public void AddWord()
        {
	        wordsStatistics.AddWord("          a");
	        wordsStatistics.GetStatistics().Should().HaveCount(1);
        }
		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	}
}