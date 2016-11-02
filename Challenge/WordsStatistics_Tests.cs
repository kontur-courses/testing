using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Challenge
{
    [TestFixture]
    public class WordsStatistics_Tests
    {
        public static string Authors = "<ВАШИ ФАМИЛИИ>"; // "Egorov Shagalina"

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
        public void AddWord_CountsOnce_WhenSameWord()
        {
            statistics.AddWord("aaaaaaaaaa");
            statistics.AddWord("aaaaaaaaaa");
            statistics.GetStatistics().Should().HaveCount(1);
        }

        /* 
            Коллекции без учёта порядка
                new[] {1,2,3}.ShouldAllBeEquivalentTo(new [] {3,2,1});
            Коллекции с учётом порядка
                new[] {1,2,3}.ShouldAllBeEquivalentTo(new [] {1,2,3}, o => o.WithStrictOrdering());
            Больше примеров: http://www.fluentassertions.com/

            Исключения:
                Assert.Throws<ArgumentNullException>(() => SomeOperation());
            
        */
    }
}