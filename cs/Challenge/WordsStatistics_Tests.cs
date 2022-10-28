using System;
using System.Collections.Generic;
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
		public void ArgumentNullException_IfNullInput()
		{
			Action addNull = () => wordsStatistics.AddWord(null);
			addNull.Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void GetStatistics_Return_WhenSpaceInput()
		{
			wordsStatistics.AddWord(" ");
			wordsStatistics.GetStatistics().Should().HaveCount(0);
		}

		[Test]
		public void GetStatistics_AddSubstring_IfEnterLengthIsMoreThanTenSymbols()
		{
			wordsStatistics.AddWord("123456789nbjsnb;nv");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("123456789n", 1));
		}

		[Test]
		public void GetStatistics_UppercaseIsEqualLowercase()
		{
			wordsStatistics.AddWord("UPPERCASE");
			wordsStatistics.AddWord("uppercase");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("uppercase", 2));
		}

		[Test]
		public void GetStatistics_ByDescendingCount_WhenDifferentWords()
		{
			wordsStatistics.AddWord("zdouble");
			wordsStatistics.AddWord("single");
			wordsStatistics.AddWord("zdouble");
			wordsStatistics.GetStatistics().Should().Equal(new List<WordCount>(){
				new WordCount("zdouble", 2),
				new WordCount("single", 1)
			});
		}
		
		[Test]
		public void GetStatistics_ByDescendingWord_WhenSameCount()
		{
			wordsStatistics.AddWord("a");
			wordsStatistics.AddWord("b");
			wordsStatistics.AddWord("a");
			wordsStatistics.AddWord("b");
			wordsStatistics.GetStatistics().Should().Equal(new List<WordCount>(){
				new WordCount("a", 2),
				new WordCount("b", 2)
			});
		}

		[Test]
		public void GetStatistics_Empty_IfAddEmptyString()
		{
			wordsStatistics.AddWord("");
			wordsStatistics.GetStatistics().Should().HaveCount(0);
		}

		// [Test]
		// public void GetStatistics_AreEqual_StringWithLengthMoreThenTenAndLessThenTen_WhenTheyHaveSamePrefix()
		// {
		// 	wordsStatistics.AddWord("123456789nLongString");
		// 	wordsStatistics.AddWord("123456789n");
		// 	wordsStatistics.GetStatistics().Should().Equal(new WordCount("123456789n", 2));;
		// }

		[Test]
		public void WordCount_Create_ReturnCorrectInstance()
		{
			var wordCount = WordCount.Create(new KeyValuePair<string, int>("key", 1));
			wordCount.Word.Should().Be("key");
			wordCount.Count.Should().Be(1);
		}

		[Test]
		public void WordCount_Add10LengthSubstringIfInputLength11()
		{
			wordsStatistics.AddWord("123456789nm");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("123456789n", 1));
		}

		[Test]
		public void WordCount_SkipSpaces()
		{
			wordsStatistics.AddWord("          g");
			wordsStatistics.GetStatistics().Should().HaveCount(1);
		}

		[Test]
		public void WordsCount_HasNotCollisionOnBigData()
		{
			for (var i = 0; i < 10000; i++)
			{
				wordsStatistics.AddWord(i.ToString());
			}

			wordsStatistics.GetStatistics().Should().HaveCount(10000);
		}
		
		
		


		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	}
}