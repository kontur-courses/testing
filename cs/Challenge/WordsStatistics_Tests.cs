using System;
using System.Threading;
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
		public void GetStatistics_ContainsItem_AfterAdditionOfWorldWithLengthLessThanTen()
		{
			wordsStatistics.AddWord("123456789");
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("123456789", 1));
		}

		[Test]
		public void GetStatistics_ContainsManyItems_AfterAdditionOfDifferentWords()
		{
			wordsStatistics.AddWord("abc");
			wordsStatistics.AddWord("def");
			wordsStatistics.GetStatistics().Should().HaveCount(2);
		}

		[Test]
		public void GetStatistics_ContainsOneItem_AfterAdditionOfSameWords()
		{
			var count = 3;

			for (var i = 0; i < count; i++)
				wordsStatistics.AddWord("abc");

			wordsStatistics.GetStatistics().Should().HaveCount(1);
			wordsStatistics.GetStatistics().Should().Equal(new WordCount("abc", count));
		}

		[Test]
		public void GetStatistics_ThrowException_AfterAddtionOfNull()
		{
			Action act = () => wordsStatistics.AddWord(null);

			act.Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void GetStatistics_IsEmpty_AfterAddtionOfEmptyWord()
		{
			wordsStatistics.AddWord("");

			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void GetStatistics_IsEmpty_AfterAddtionOfWhiteSpaceSequence()
		{
			wordsStatistics.AddWord("\t");

			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void GetStatistics_ContainsStripWord_AfterAddititionOfLongWord()
		{
			wordsStatistics.AddWord("12345678999");

			wordsStatistics.GetStatistics().Should().Equal(new WordCount("1234567899", 1));
		}

		[Test]
		public void GetStatistics_ContainsOneItem_AfterAddititionOfManyWordsWithSamePrefix()
		{
			wordsStatistics.AddWord("123456789999");
			wordsStatistics.AddWord("1234567899----");

			wordsStatistics.GetStatistics().Should().Equal(new WordCount("1234567899", 2));
		}

		[Test]
		public void GetStatistics_ContainsOneItem_AfterAddititionOfDifferentCaseWords()
		{
			wordsStatistics.AddWord("HELLO");
			wordsStatistics.AddWord("hello");
			wordsStatistics.AddWord("hELlo");

			wordsStatistics.GetStatistics().Should().Equal(new WordCount("hello", 3));
		}

		[Test]
		public void GetStatistics_ContainsOneItem_AfterAddititionOfWordWithWhitespacePrefix()
		{
			wordsStatistics.AddWord("\n\n\n\n\n\n\n\n\n\nhello");

			wordsStatistics.GetStatistics()
				.Should().Equal(new WordCount("\n\n\n\n\n\n\n\n\n\n", 1));

		}

		[Test]
		public void GetStatistics_ContainsStripWord_AfterAddititionOfLongWordWithLength10()
		{
			wordsStatistics.AddWord("1234567899");

			wordsStatistics.GetStatistics().Should().Equal(new WordCount("1234567899", 1));
		}

		// HERE

		[Test]
		public void GetStatistics_OrderedByCount_AfterAdditionOfWords()
		{
			wordsStatistics.AddWord("aaa");
			wordsStatistics.AddWord("aba");
			wordsStatistics.AddWord("aba");

			wordsStatistics.GetStatistics().Should().Equal(
				new WordCount("aba", 2), new WordCount("aaa", 1));
		}

		[Test]
		public void GetStatistics_OrderedByDescendingCount_AfterAdditionOfWords()
		{
			wordsStatistics.AddWord("aba");
			wordsStatistics.AddWord("aba");
			wordsStatistics.AddWord("aaa");

			wordsStatistics.GetStatistics().Should().Equal(
				new WordCount("aba", 2), new WordCount("aaa", 1));
		}

		[Test]
		public void GetStatistics_OrderedByDescendingWord_AfterAdditionOfWords()
		{
			wordsStatistics.AddWord("aaa");
			wordsStatistics.AddWord("aba");
			wordsStatistics.AddWord("aca");

			wordsStatistics.GetStatistics().Should().Equal(
				new WordCount("aaa", 1), new WordCount("aba", 1), new WordCount("aca", 1));
		}

		[Test]
		public void GetStatistics_ContainsDifferentItems_AcrossMultipleObjects()
		{
			wordsStatistics.AddWord("aaa");
			var newStatistics = new WordsStatistics();

			wordsStatistics.GetStatistics().Should().Equal(new WordCount("aaa", 1));
		}

		[Test]
		public void GetStatistics_ParseSpecificLetters()
		{
			wordsStatistics.AddWord("Б");

			wordsStatistics.GetStatistics().Should().Equal(new WordCount("б", 1));
		}

		[Test]
		public void GetStatistics_ContainsAllItems_AfterAddingBigNumberOfWords()
		{
			var count = 1300;
			for (var i = 0; i < count; i++)
				wordsStatistics.AddWord(i.ToString());

			wordsStatistics.GetStatistics().Should().HaveCount(count);
		}

		[Test, Timeout(500)]
		public void GetStatistics_ComputeInLittleTime_AfterAddingBigNumberOfWords()
		{
			var count = 5000;
			for (var i = 0; i < count; i++)
				wordsStatistics.AddWord(i.ToString());

			wordsStatistics.GetStatistics().Should().HaveCount(count);
		}


	}
}