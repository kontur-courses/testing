using System;
using FluentAssertions;
using NUnit.Framework;

namespace Challenge
{
	[TestFixture]
	public class WordsStatistics_Tests
	{
		public static string Authors = "ВАШИ ФАМИЛИИ ЧЕРЕЗ ПРОБЕЛ"; // "Egorov Shagalina"

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


		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	}
}