using System;
using FluentAssertions;
using NUnit.Framework;
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
		public void AddWords_StatisticsSetWithSizeZero()
		{
			SetUp();
			var actual = wordsStatistics.GetStatistics().Count();
			actual.Should().Be(0);
		}

		[Test]
		public void AddWords_IncrementCountByAdding()
		{
			SetUp();
			wordsStatistics.AddWord("jad");
			var actual = wordsStatistics.GetStatistics().Count();
			actual.Should().Be(1);
		}

		[Test]
		public void AddWords_ThrowExceptionIfWordIsNull()
		{
			Action action = () => { wordsStatistics.AddWord(null); };

			action.Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void AddWords_WhiteSpaceAdditionDoesNothing()
		{
			SetUp();
			wordsStatistics.AddWord(" ");
			Assert.IsFalse(wordsStatistics.GetStatistics().Any());
		}

		[Test]
		public void AddWords_MultipleSpacesAdditionSuccessful()
		{
			SetUp();
			wordsStatistics.AddWord("          abc");
			Assert.IsTrue(wordsStatistics.GetStatistics().Any());
		}

		[Test]
		public void GetStatistics_WordsOrderedByDescending()
		{
			SetUp();
			wordsStatistics.AddWord("c");
			wordsStatistics.AddWord("a");
			wordsStatistics.AddWord("h");
			wordsStatistics.AddWord("z");
			var expected = new[] {"a", "c", "h", "z"};
			var actual = wordsStatistics.GetStatistics().Select(x => x.Word).ToArray();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetStatistics_CountOrderedByDescending()
		{
			SetUp();
			wordsStatistics.AddWord("a");
			wordsStatistics.AddWord("b");
			wordsStatistics.AddWord("b");
			wordsStatistics.AddWord("b");
			var expected = new int[] {3, 1};
			var actual = wordsStatistics.GetStatistics().Select(x => x.Count).ToArray();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetStatistics_ShouldNotReturnNewStatistics()
		{
			SetUp();
			wordsStatistics.AddWord("a");
			wordsStatistics.GetStatistics();
			wordsStatistics.AddWord("b");
			var actual = wordsStatistics.GetStatistics().Count();
			actual.Should().Be(2);
		}

		[Test]
		public void AddWords_AddingWordPutsItInStatistics()
		{
			SetUp();
			wordsStatistics.AddWord("ABC");
			Assert.IsFalse(wordsStatistics.GetStatistics().Select(x => x.Word).Contains("ABC"));
		}

		[Test]
		public void AddWords_WithSimilarStartIsSameWord()
		{
			SetUp();
			wordsStatistics.AddWord("0123456789a");
			wordsStatistics.AddWord("0123456789d");
			var actual = wordsStatistics.GetStatistics().Select(x => x.Count).First();
			2.Should().Be(actual);
		}

		[Test]
		public void AddWords_SmallWordsIsAddedCorrect()
		{
			SetUp();
			wordsStatistics.AddWord("abcdefg");
			var actual = wordsStatistics.GetStatistics().Select(x => x.Word).First().Length;
			actual.Should().Be(7);
		}

		[Test]
		public void AddWords_FailsOnAddingWordBiggerThan1000Length()
		{
			SetUp();
			for (var i = 0; i < 5000; i++) wordsStatistics.AddWord(i.ToString());

			var actual = wordsStatistics.GetStatistics().Count();
			actual.Should().Be(5000);
		}

		[Test]
		[Timeout(10)]
		public void AddWords_AddBigCountOfWords()
		{
			SetUp();
			for (var i = 0; i < 5390; i++)
				wordsStatistics.AddWord(i.ToString());
		}

		[Test]
		public void GetStatistics_StatisticsCollectionIsNotStatic()
		{
			var wordsStatistics1 = CreateStatistics();
			var wordsStatistics2 = CreateStatistics();
			wordsStatistics1.AddWord("a");
			wordsStatistics1.AddWord("b");
			wordsStatistics2.AddWord("c");
			wordsStatistics2.AddWord("d");
			var firstArray = wordsStatistics1.GetStatistics().Select(x => x.Word).ToArray();
			var secondArray = wordsStatistics2.GetStatistics().Select(x => x.Word).ToArray();
			Assert.AreNotEqual(firstArray, secondArray);
		}
	}
}