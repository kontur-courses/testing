using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class ObjectComparison
    {
        [Test]
        [Description("Проверка текущего царя")]
        [Category("ToRefactor")]
        public void CheckCurrentTsar()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();

            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Перепишите код на использование Fluent Assertions.

            actualTsar.Name.Should().Be(expectedTsar.Name);
            actualTsar.Age.Should().Be(expectedTsar.Age);
            actualTsar.Height.Should().Be(expectedTsar.Height);
            actualTsar.Weight.Should().Be(expectedTsar.Weight);

            expectedTsar.Parent!.Name.Should().Be(actualTsar.Parent!.Name);
            expectedTsar.Parent.Age.Should().Be(actualTsar.Parent.Age);
            expectedTsar.Parent.Height.Should().Be(actualTsar.Parent.Height);
            expectedTsar.Parent.Parent.Should().Be(expectedTsar.Parent.Parent);
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода? 
            // Недостаток такой, что при добавлении нового поля в классе Person, придётся добавлять корректировать метод AreEquals. 

            // Моё решение отличается от предыдущего тем, что в нём использована рефлексия, которая позволяет сравнивать поля классов.
            // То есть даже при добавлении нового поля, тест переписывать не придётся
            // К тому же до проверки равности полей мы проверяем находятся ли цари и их родители в рекурсивном отношении.
            var tsarIdSet = new HashSet<int>(actualTsar.Id);
            var firstTsar = TsarRegistry.GetCurrentTsar();
            while (firstTsar.Parent != null)
            {
                Assert.DoesNotThrow(() => tsarIdSet.Add(firstTsar.Parent.Id));
                firstTsar = firstTsar.Parent;
            }

            actualTsar.Should().BeEquivalentTo(expectedTsar,
                option => option.Excluding(person => person.Id)
                    .Excluding(info =>
                        info.SelectedMemberPath.EndsWith($"{nameof(Person.Parent)}.{nameof(Person.Id)}")));
        }
    }
}