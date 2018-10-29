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
		public void DoSomething_WhenSomething()
		{
			wordsStatistics.AddWord("asd");
			wordsStatistics.AddWord("asd");
			wordsStatistics.AddWord("qwe");
			wordsStatistics.GetStatistics().Should().BeInDescendingOrder(x => x.Count);
		}
		[Test]
		public void DoSomething_WhenSomething_1()
		{
			wordsStatistics.AddWord("asd");
			wordsStatistics.AddWord("qwe");
			wordsStatistics.GetStatistics().Should().BeInAscendingOrder(x => x.Word);
		}

		[Test]
		public void GetStatistcic_CheckLongWord()
		{
			var word = "asdasdaadsadasdasdasdasdasdasdasfsdagadfs";
			wordsStatistics.AddWord(word);
			foreach(var c in wordsStatistics.GetStatistics())
				c.Word.Should().Be(word.Substring(0,10));
		}

		[Test]
		public void CheckTrow()
		{
			Assert.Throws<ArgumentNullException>(()=>wordsStatistics.AddWord(null));
		}

		[Test]
		public void GetStatistic_DOnotCut()
		{

			var word = "asdasd";
			wordsStatistics.AddWord(word);
			foreach (var c in wordsStatistics.GetStatistics())
				c.Word.Should().Be(word);
		}

		[Test]
		public void checkAddEmpty()
		{
			wordsStatistics.AddWord("");
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void checkCut()
		{
			wordsStatistics.AddWord("	");
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void checkLower()
		{
			var w = "QWE";
			wordsStatistics.AddWord(w);
			wordsStatistics.GetStatistics().First().Word.Should().Be(w.ToLower());
		}

		[Test]
		public void ckeckEmpty()
		{
			wordsStatistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void ckecknocut()
		{
			var w = "asdasd";
			wordsStatistics.AddWord(w);
			wordsStatistics.GetStatistics().First().Word.Should().Be(w);
		}

		[Test]
		public void checkCaseSame()
		{
			wordsStatistics.AddWord("qwe");
			wordsStatistics.AddWord("QWE");
			wordsStatistics.GetStatistics().First().Count.Should().Be(2);
		}


		[Test]
		public void checkSame10()
		{
			wordsStatistics.AddWord("qweqweqweqwe");
			wordsStatistics.AddWord("qweqweqweq");
			wordsStatistics.GetStatistics().First().Count.Should().Be(2);
		}


		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	}
}