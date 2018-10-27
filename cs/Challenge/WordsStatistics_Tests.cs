using System;
using System.Linq;
using System.Text;
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
		public void ThrowsException_WhenSomething()
		{
			Action action = () => wordsStatistics.AddWord(null);
			action.ShouldThrow<ArgumentNullException>();
		}

		[Test]
		public void Return_whenEmptyString()
		{
            wordsStatistics.AddWord(" ");
			wordsStatistics.GetStatistics().Should().BeEmpty();
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
		public void CutWordsLongerThen10Symbols()
		{
			wordsStatistics.AddWord("12345678901");
			wordsStatistics.GetStatistics().First().Word.Should().BeEquivalentTo("1234567890");
		}

		[Test]
		public void IndifferentRegisterPerception()
		{
            wordsStatistics.AddWord("wOrd");
            wordsStatistics.AddWord("WorD");
			wordsStatistics.GetStatistics().First().Count.ShouldBeEquivalentTo(2);
		}

		[Test]
		public void AddsStringWithMoreThen10SpacePrefix()
		{
			wordsStatistics.AddWord("          qwe");
			wordsStatistics.GetStatistics().Count().ShouldBeEquivalentTo(1);
		}

		[Test]
		public void DoNotCutWord_WithLengthMoreThen5AndLessThen10()
		{
			var word = "123456789";

            wordsStatistics.AddWord(word);
			wordsStatistics.GetStatistics().First().Word.ShouldBeEquivalentTo(word);
		}

		[Test]
		public void SortByFrequency()
		{
			wordsStatistics.AddWord("b");
			wordsStatistics.AddWord("b");
			wordsStatistics.AddWord("c");
			wordsStatistics.AddWord("a");

			wordsStatistics.GetStatistics().Select(w => w.Word).First().ShouldBeEquivalentTo("b");
		}

		[Test]
		public void CorrectSortByAlphabetic_WhenFreqAreSame()
		{
			wordsStatistics.AddWord("b");
			wordsStatistics.AddWord("a");
			wordsStatistics.GetStatistics().First().Word.ShouldBeEquivalentTo("a");
		}

		[Test]
		public void ALotOfStrings()
		{
			var strBuilder = new StringBuilder();
			int a = 0;
			for (int i = 0; i < 1300; i++)
			{
				wordsStatistics.AddWord(a.ToString());
				a++;

			}
			wordsStatistics.GetStatistics().Count().Should().Be(1300);
		}

		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	}
}