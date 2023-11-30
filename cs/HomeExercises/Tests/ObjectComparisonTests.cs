using FluentAssertions;
using NUnit.Framework;
using System;

namespace HomeExercises.Tests
{
    [TestFixture(TestOf = typeof(TsarRegistry))]
    public class ObjectComparisonTests
    {
        [Test]
        [Description("Проверка текущего царя")]
        [Category("ToRefactor")]
        public void CheckCurrentTsar()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();

            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            actualTsar.Should()
                .BeEquivalentTo(expectedTsar, x => x
                    .Excluding(x => x.Id)
                    .Excluding(x => x.Parent!.Id));
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода? 
            // Данный подход делает класс труднорасширяемым, ведь при добавлении новых полей в класс Person
            // нужно переписывать и метод сравнения для всех этих полей, так же новые поля cможет добавить
            // другой разработчик, который не знает о таком методе сравнения, что приведет к новым, неотловоенным ошибкам - 
            // новые поля не будут сравниваться.
            // В моем решении (CheckCurrentTsar), такой ошибки не возникнет и класс Person сможет без ошибок расширяться
            // при условии, что сравниваться будут все поля, кроме Id (так же и их Parent), так как код написан не перебиранием
            // всех полей для сравнения, а сравнением объекта в целом с исключением его Id.
            Assert.True(AreEqual(actualTsar, expectedTsar));
        }

        private bool AreEqual(Person? actual, Person? expected)
        {
            if (actual == expected) return true;
            if (actual == null || expected == null) return false;
            return
                actual.Name == expected.Name
                && actual.Age == expected.Age
                && actual.Height == expected.Height
                && actual.Weight == expected.Weight
                && AreEqual(actual.Parent, expected.Parent);
        }
    }
}
