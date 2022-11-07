using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Mono.Cecil.Cil;
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

		[Test]
		public void AddWord_TrimsWordTo10Chars()
		{
			var sb= new StringBuilder();
			for (int i = 0; i < 11; i++) sb.Append('a');

			var longWord = sb.ToString();
			var longWordTrimmed = longWord.Substring(0, 10);

			wordsStatistics.AddWord(longWord);

			wordsStatistics.GetStatistics().Should().Equal(new WordCount(longWordTrimmed, 1));
		}



        [Test]
        public void GetStatistics_CheckIgnoreCaseE_AfterAddition()
        {
            wordsStatistics.AddWord("E");
            wordsStatistics.GetStatistics().Should().NotContain(new WordCount("E", 0));
        }

        [Test]
		public void AddWord_ThrowsNullExc()
		{
			Assert.Throws<ArgumentNullException>(()=>wordsStatistics.AddWord(word:null));
		}

		[Test]
		public void AddWord_DoesntThrowException_OnWhitespace()
		{
			Assert.DoesNotThrow(() => wordsStatistics.AddWord(" "));
		}

		[Test]
		public void AddWord_IgnoresEmptyString()
		{
			Assert.DoesNotThrow(() => wordsStatistics.AddWord(""));
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void AddWord_DoesntAdd_Whitespaces()
		{
			wordsStatistics.AddWord(" ");
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void AddWord_DoesntIgnoreWord_With10WhitespacesPrefix()
		{
			wordsStatistics.AddWord("          a");
			wordsStatistics.GetStatistics().Should().NotBeEmpty();
		}

		[Test]
		public void AddWord_AddsAsIs_WordShorterThan11()
		{
			var word = "abcdefghij";
			wordsStatistics.AddWord(word);
			wordsStatistics.GetStatistics().Should().Equal(new WordCount(word, 1));
		}

		[Test]
		public void GetStatistics_OrdersByCountDesc()
		{
			wordsStatistics.AddWord("a");
			
			wordsStatistics.AddWord("c");
			wordsStatistics.AddWord("c");

			wordsStatistics.AddWord("b");
			wordsStatistics.AddWord("b");
			wordsStatistics.AddWord("b");

			wordsStatistics.GetStatistics().Should().Equal(
				new WordCount("b", 3),
				new WordCount("c", 2),
				new WordCount("a", 1)
			);
		}

		[Test]
		public void GetStatistics_OrdersByWordAsc_ForEqualCounts()
		{
			wordsStatistics.AddWord("a");
			wordsStatistics.AddWord("a");

			wordsStatistics.AddWord("c");
			wordsStatistics.AddWord("c");

			wordsStatistics.GetStatistics().Should().Equal(
				new WordCount("a", 2),
				new WordCount("c", 2)
			);

		}

		[Test]
		public void WordStatistics_ResistsCollision_OnManyWords()
		{
			var words = new List<string>();
			for (int i = 0; i < 1500; i++) 
				words.Add(i.ToString());
			var wordCounts = words
				.Select(w => new WordCount(w, 1))
				.OrderBy(wc => wc.Word);

			foreach (var word in words) 
				wordsStatistics.AddWord(word);

			wordsStatistics.GetStatistics().Should().Equal(wordCounts);
		}

        // Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
    }
}