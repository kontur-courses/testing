using FluentAssertions;
using NUnit.Framework;
using System;
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
		public void Fail_OnNullString()
		{

			Assert.Throws<ArgumentNullException>(() => wordsStatistics.AddWord(null));
		}

		[Test]
		public void AddWord_ShouldntAdd_SpaceStringArgument()
		{
			wordsStatistics.AddWord(" ");
			Assert.IsEmpty(wordsStatistics.GetStatistics());
		}

		[Test]
		public void GetStatistics_DescendingOrder()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("aaa");
			var stats = wordsStatistics.GetStatistics().ToArray();
			Assert.That(stats[0].Count, Is.EqualTo(2));
		}
		public void GetStatistics_LexicographicalOrder_WhenEqualFreq()
		{
			wordsStatistics.AddWord("a");
			wordsStatistics.AddWord("b");
			var stats = wordsStatistics.GetStatistics().ToArray();
			Assert.That(stats[0].Word, Is.EqualTo("a"));
		}

		[Test]
		public void AddWord_NotCaseInsensitive()
		{
			wordsStatistics.AddWord("AAA");
			wordsStatistics.AddWord("aaa");
			Assert.That(wordsStatistics.GetStatistics().ToArray()[0].Count, 
				Is.EqualTo(2));
		}

		[Test]
		public void AddWord_SubstingLengthShouldBeTen()
		{
			wordsStatistics.AddWord("0123456789x");
			Assert.That(wordsStatistics.GetStatistics().ToArray()[0].Word, 
				Is.EqualTo("0123456789"));
		}

		public void AddWord_DoesntChangeWord()
		{
			wordsStatistics.AddWord("012345678");
			wordsStatistics.AddWord("012345679");
            Assert.That(wordsStatistics.GetStatistics().ToArray()[0].Count,
				Is.EqualTo(1));
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


		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	}
}