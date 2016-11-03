using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Challenge.Solved
{
    [TestFixture]
    public class WordsStatistics_Tests
    {
        public virtual IWordsStatistics CreateStatistics()
        {
            // меняется на разные реализации при запуске exe
            return new Challenge.WordsStatistics();
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
        public void AddWord_AllowsShortWords()
        {
            statistics.AddWord("aaa");
        }

        [Test]
        public void AddWord_CountsOnce_WhenSameWord()
        {
            statistics.AddWord("aaaaaaaaaa");
            statistics.AddWord("aaaaaaaaaa");
            statistics.GetStatistics().Should().HaveCount(1);
        }

        [Test]
        public void AddWord_IncrementsCounter_WhenSameWord()
        {
            statistics.AddWord("aaaaaaaaaa");
            statistics.AddWord("aaaaaaaaaa");
            statistics.GetStatistics().Select(t => t.Item1)
                .Should().BeEquivalentTo(2);
        }

        [Test]
        public void GetStatistics_SortsWordsByFrequency()
        {
            statistics.AddWord("aaaaaaaaaa");
            statistics.AddWord("bbbbbbbbbb");
            statistics.AddWord("bbbbbbbbbb");
            statistics.GetStatistics().Select(t => t.Item2)
                .ShouldBeEquivalentTo(new[] { "bbbbbbbbbb", "aaaaaaaaaa" },
                    options => options.WithStrictOrdering());
        }

        [Test]
        public void GetStatistics_SortsWordsByAbc_WhenFrequenciesAreSame()
        {
            statistics.AddWord("cccccccccc");
            statistics.AddWord("aaaaaaaaaa");
            statistics.AddWord("bbbbbbbbbb");
            statistics.GetStatistics().Select(t => t.Item2)
                .Should().ContainInOrder("aaaaaaaaaa", "bbbbbbbbbb", "cccccccccc");
        }

        [Test]
        public void AddWordThrows_WhenWordIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => statistics.AddWord(null));
        }

        [Test]
        public void AddWord_Ignores_EmptyWord()
        {
            statistics.AddWord("");
            statistics.GetStatistics().Should().BeEmpty();
        }

        [Test]
        public void AddWord_Ignores_WhitespaceWord()
        {
            statistics.AddWord(" ");
            statistics.GetStatistics().Should().BeEmpty();
        }

        [Test]
        public void AddWord_CutsWords_LongerThan10()
        {
            statistics.AddWord("12345678901");
            statistics.GetStatistics().Select(t => t.Item2)
                .Should().BeEquivalentTo("1234567890");
        }

        [Test]
        public void AddWord_CutWordsJoined()
        {
            statistics.AddWord("12345678901");
            statistics.AddWord("1234567890");
            statistics.GetStatistics().Select(t => t.Item1)
                .Should().BeEquivalentTo(2);
        }

        [Test]
        public void AddWord_AllowWordAndCutItToWhitespaces_WhenWordPrecededByWhitespaces()
        {
            statistics.AddWord("          a");
            statistics.GetStatistics().Select(t => t.Item2)
                .Should().BeEquivalentTo("          ");
        }

        [Test]
        public void AddWord_IsCaseInsensitive()
        {
            var counter = 0;
            for (char c = 'a'; c <= 'z'; c++)
            {
                statistics.AddWord("".PadRight(10, c));
                statistics.AddWord("".PadRight(10, c).ToUpper());
                counter++;
            }
            for (char c = 'а'; c <= 'я' || c <= 'ё'; c++)
            {
                statistics.AddWord("".PadRight(10, c));
                statistics.AddWord("".PadRight(10, c).ToUpper());
                counter++;
            }
            statistics.GetStatistics().Should().HaveCount(counter);
        }

        [Test]
        public void AddWord_HaveNoCollisions()
        {
            const int wordCount = 1000;
            for (int i = 0; i < wordCount; i++)
            {
                statistics.AddWord(i.ToString().PadRight(10));
            }
            statistics.GetStatistics().Should().HaveCount(wordCount);
        }

        [Test, Timeout(100)]
        public void AddWord_HaveSufficientPerformance_OnAddingDifferentWords()
        {
            for (int i = 0; i < 1000; i++)
            {
                statistics.AddWord(i.ToString().PadRight(10));
            }
            statistics.GetStatistics();
        }

        [Test, Timeout(100)]
        public void AddWord_HaveSufficientPerformance_OnAddingSameWord()
        {
            for (int i = 0; i < 1000; i++)
            {
                statistics.AddWord(i.ToString().PadRight(10));
            }
            var sameWord = 9.ToString().PadRight(10);
            for (int i = 0; i < 1000; i++)
            {
                statistics.AddWord(sameWord);
            }
            statistics.GetStatistics();
        }

        [Test]
        public void SeveralInstansesAreSupported()
        {
            var anotherStatistics = CreateStatistics();
            statistics.AddWord("aaaaaaaaaa");
            anotherStatistics.AddWord("bbbbbbbbbb");
            statistics.GetStatistics().Should().HaveCount(1);
        }
		
        [Test]
        public void GetStatistics_ShouldSupportMultipleEnumeration()
        {
            statistics.AddWord("aaaaaaaaaa");
            var stat = statistics.GetStatistics().ToList();
            statistics.GetStatistics().Should().NotBeEmpty();
        }

        [Test]
        public void Statistics_ShouldBeModifiable_AfterEnumeration()
        {
            var stat = statistics.GetStatistics().ToList();
            statistics.AddWord("aaaaaaaaaa");
            statistics.GetStatistics().Should().NotBeEmpty();
        }
    }
}