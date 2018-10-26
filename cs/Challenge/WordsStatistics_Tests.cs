using System;
using System.Linq;
using Challenge.IncorrectImplementations;
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
        public void GetStatistics_ContainsOneElement_AfterAdditionEqualElement()
        {
            wordsStatistics.AddWord("abc");
            wordsStatistics.AddWord("abc");
            wordsStatistics.GetStatistics().Should().HaveCount(1);
        }

        [Test]
        public void GetStatistics_CorrectOrder_AfterAdditionTwoItemsWithDifferentFrequencies()
        {
            wordsStatistics.AddWord("abc");
            wordsStatistics.AddWord("abc");
            wordsStatistics.AddWord("b");

            wordsStatistics
                .GetStatistics()
                .Select(x => x.Word)
                .ToArray()
                .ShouldBeEquivalentTo(new[] { "abc", "b" }, x => x.WithStrictOrdering());
        }

        [Test]
        public void GetStatistics_CorrectOrder_AfterAdditionTwoDifferentItemsOneTime()
        {
            wordsStatistics.AddWord("abc");
            wordsStatistics.AddWord("b");

            wordsStatistics
                .GetStatistics()
                .Select(x => x.Word)
                .ToArray()
                .ShouldBeEquivalentTo(new[] { "abc", "b" }, x => x.WithStrictOrdering());
        }

        [Test]
        public void GetStatistics_Fails_AfterAdditionNullItem()
        {

            Action action = () => { wordsStatistics.AddWord(null); };
            action.ShouldThrow<ArgumentNullException>();
        }


        [Test]
        public void GetStatistics_Empty_AfterAdditionEmptyString()
        {
            wordsStatistics.AddWord("");

            wordsStatistics
                .GetStatistics()
                .ToArray()
                .ShouldBeEquivalentTo(new WordCount[0]);
        }

        [Test]
        public void GetStatistics_ContainOneItem_AfterAdditionTwoDifferentStringWithEqualsBeginning()
        {
            wordsStatistics.AddWord("aaaaaaaaaabbb");
            wordsStatistics.AddWord("aaaaaaaaaaccc");

            wordsStatistics
                .GetStatistics()
                .Count()
                .ShouldBeEquivalentTo(1);
        }
        [Test]
        public void GetStatistics_ContainOneItemWithTenLengths_AfterAdditionStringWithLengthMoreThenTen()
        {
            wordsStatistics.AddWord("aaaaaaaaaabbb");

            wordsStatistics
                .GetStatistics()
                .First()
                .Word
                .Length
                .ShouldBeEquivalentTo(10);
        }

        [Test]
        public void GetStatistics_ShouldContainOneStringWithLengthSix_AfterAdditionOneStringWithLengthSix()
        {
            wordsStatistics.AddWord("aaaaaa");

            wordsStatistics
                .GetStatistics()
                .First()
                .Word
                .Length
                .ShouldBeEquivalentTo(6);
        }

        [Test]
        public void GetStatistics_ContainOneItemWithTenLengths_AfterAdditionStringWithLengthEleven()
        {
            wordsStatistics.AddWord("aaaaaaaaaab");

            wordsStatistics
                .GetStatistics()
                .First()
                .Word
                .Length
                .ShouldBeEquivalentTo(10);
        }

        [Test]
        public void GetStatistics_ContainLowerCaseString_AfterAdditionUpperCaseString()
        {

            wordsStatistics.AddWord("A");

            wordsStatistics
                .GetStatistics()
                .First()
                .Word
                .ShouldBeEquivalentTo("a");
        }

        [Test]
        public void GetStatistics_ContainOneItem_AfterAdditionStringWithTenWhiteSpacesInTheBeginningAndThenLetters()
        {

            wordsStatistics.AddWord("            aaa");

            wordsStatistics
                .GetStatistics()
                .Count()
                .ShouldBeEquivalentTo(1);
        }

        [Test]
        public void GetStatistics_CorrectOrder_AfterAdditionTwoStringsWithDifferentFrequencies()
        {
            wordsStatistics.AddWord("abc");
            wordsStatistics.AddWord("b");
            wordsStatistics.AddWord("b");

            wordsStatistics
                .GetStatistics()
                .Select(x => x.Word)
                .ToArray()
                .ShouldBeEquivalentTo(new[] { "b", "abc" }, x => x.WithStrictOrdering());
        }

        [Test]
        public void GetStatistics_IsEmpty_AfterAdditionWhiteSpacesString()
        {

            wordsStatistics.AddWord("  ");

            wordsStatistics
                .GetStatistics()
                .Count()
                .ShouldBeEquivalentTo(0);
        }

        //[Test]
        //public void GetStatistics_ShouldCintains
        // Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
    }
}