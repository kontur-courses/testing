using System;
using FluentAssertions;
using NUnit.Framework;

namespace Challenge
{
    [TestFixture]
    public class WordsStatistics_Tests_Should
    {
        private IWordsStatistics statistics { get; set; }

        [SetUp]
        public void SetUp()
        {
            statistics = new WordsStatistics();
        }


        [Test]
        public void DoSomething_WhenSomething()
        {

        }
    }

	[TestFixture]
	public class WordsStatistics_Tests
	{
		public static string Authors = "Макаров Петряшов"; // "Egorov Shagalina"

		public virtual IWordsStatistics CreateStatistics()
		{
			// меняется на разные реализации при запуске exe
			return new WordsStatistics();
		}

		private IWordsStatistics statistics;

		[SetUp]
		public void SetUp()
		{
			statistics = CreateStatistics();
		}

		[Test]
		public void GetStatistics_IsEmpty_AfterCreation()
		{
			statistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void GetStatistics_ContainsItem_AfterAddition()
		{
			statistics.AddWord("abc");
			statistics.GetStatistics().Should().Equal(Tuple.Create(1, "abc"));
		}

		[Test]
		public void GetStatistics_ContainsManyItems_AfterAdditionOfDifferentWords()
		{
			statistics.AddWord("abc");
			statistics.AddWord("def");
			statistics.GetStatistics().Should().HaveCount(2);
		}


	    [Test]
	    public void AddWord_ThrowArgumentNullException_OnNull()
	    {
            Action act = () => statistics.AddWord(null);
            act.ShouldThrow<ArgumentNullException>();
	    }

	    

        [Test]
	    public void AddWord_ShouldNotAdd_WordWithSpaces()
	    {
	        statistics.AddWord("      ");
	        statistics.GetStatistics().Should().BeEmpty();
	    }

	  

	    [Test]
	    public void AddWord_ShouldLowerCaseWords()
	    {
	        statistics.AddWord("ABC");
            statistics.GetStatistics().Should().Equal(Tuple.Create(1, "abc"));
	    }

	    [Test]
	    public void GetStatistics_ShouldSortElements_ByDescending()
	    {
	        statistics.AddWord("abc");
	        statistics.AddWord("abc");
	        statistics.AddWord("cde");
            statistics.GetStatistics().Should().BeEquivalentTo(Tuple.Create(2, "abc"), Tuple.Create(1, "cde"));
        }

	    [Test]
	    public void AddWordShould_CountWord_ThatEqualBefore10Symbols()
	    {
	        statistics.AddWord(new String('a', 10));
	        statistics.AddWord(new String('a', 20));
	        statistics.GetStatistics().Should().HaveCount(1);
	    }

	    [Test]
	    public void BB()
	    {
	        statistics.AddWord("aa");
	        statistics.AddWord("aa");
	        statistics.AddWord("bb");
	        statistics.AddWord("bb");
	        statistics.GetStatistics().ShouldAllBeEquivalentTo(new[] {Tuple.Create(2, "aa"), Tuple.Create(2, "bb")}, o => o.WithStrictOrdering());
	    }


	    [Test]
	    public void AddWord_ShouldNotIgnoreEmptyWord_AfterCutto10Symbols()
	    {
            statistics.AddWord("           a");
	        statistics.GetStatistics().Should().HaveCount(1);
	    }

	    [Test]
	    public void AddWord_ShouldCutWords_To10Symbols()
	    {
	        statistics.AddWord(new String('a', 11));
	        statistics.GetStatistics().Should().BeEquivalentTo(Tuple.Create(1, "aaaaaaaaaa"));
	    }

	    [Test]
	    public void AA()
	    {
            statistics.AddWord("a");
	        statistics.GetStatistics();

            statistics.AddWord("b");
	        statistics.GetStatistics().Should().HaveCount(2);
	    }

        // Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
    }
}