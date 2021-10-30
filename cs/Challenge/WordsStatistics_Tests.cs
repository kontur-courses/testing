using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
		private static List<string> rndWords = new List<string>();

		static WordsStatistics_Tests()
		{
			for (var i = 0; i < 10000; i++)
			{
				rndWords.Add(Guid.NewGuid().ToString());
			}
		}

		[SetUp]
		public void SetUp()
		{
			wordsStatistics = CreateStatistics();
		}

		public void GetStatistics_IsEmpty_AfterCreation()
		{
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[TestCase("abc")]
		[TestCase("abcgdjkfjdflgdlk")]
		[TestCase("aDFDFDFbcgdjkfjdflgdlk")]
		[TestCase("12345678910")]
		[TestCase("          1")]
		public void GetStatistics_ContainsItem_AfterAddition(string input)
		{
			var actualAdded = input.ToLower();
			if (actualAdded.Length > 10)
				actualAdded = actualAdded.Substring(0, 10);
			
			wordsStatistics.AddWord(input);
			wordsStatistics.GetStatistics().Should().Equal(new WordCount(actualAdded, 1));
		}

		[Test]
		public void GetStatistics_ContainsManyItems_AfterAdditionOfDifferentWords()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("def");
			wordsStatistics.GetStatistics().Should().HaveCount(2);
		}
		
		[Test]
		public void GetStatistics_ContainsManyItems_AfterAdditionOfBigDifferentWords()
		{
			wordsStatistics.AddWord("abcabcabcabcmskldjfshfygdufscvgdvd");
			wordsStatistics.AddWord("abcabcabcabceyiwrhfnddjksf");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("abcabcabca", 2));
		}

		[TestCase("")]
		[TestCase(" ")]
		[TestCase("\t")]
		[TestCase("\n")]
		public void AddWord_ContainSameItems_AfterAdditionEmptyString(string m)
		{
			wordsStatistics.AddWord(m);
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void AddWord_ThrowArgumentNullException_AfterAdditionNull()
		{
			string word = null;
			Action action = () => wordsStatistics.AddWord(word);

			action.Should()
				.Throw<ArgumentNullException>();
			// .WithMessage(nameof(word));
		}

		[Test]
		public void GetStatistics_SortedCorrectly_AfterAdditionManyWord()
		{
			var correctWordStatistics = new List<WordCount>();

			var words = new List<string>
			{
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
			};

			var rnd = new Random();
			foreach (var word in words)
			{
				var count = rnd.Next(1, 999);
				correctWordStatistics.Add(new WordCount(word, count));
				for (var i = 0; i < count; i++)
					wordsStatistics.AddWord(word);
			}
			
			correctWordStatistics
				.OrderByDescending(wordCount => wordCount.Count)
				.ThenBy(wordCount => wordCount.Word)
				.Should()
				.BeEquivalentTo(wordsStatistics.GetStatistics());
		}

		[Test, Timeout(1000)]
		public void AddWord_Complexity_AdditionManyManySameWords()
		{
			for (var i = 0; i < 1000000; i++)
			{
				wordsStatistics.AddWord("sdsds");
			}
			wordsStatistics.GetStatistics();
		}
		
		[Test, Timeout(500)]
		public void AddWord_Complexity_AdditionManyManyDifferentWords()
		{
			foreach (var rndWord in rndWords)
			{
				wordsStatistics.AddWord(rndWord);
			}

			wordsStatistics.GetStatistics();
		}

		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	}
}