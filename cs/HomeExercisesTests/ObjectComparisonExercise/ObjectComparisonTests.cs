using FluentAssertions;
using FluentAssertions.Equivalency;
using HomeExercises.ObjectComparisonExercise;
using NUnit.Framework;

namespace HomeExercisesTests.ObjectComparisonExercise
{
    public class ObjectComparisonTests
    {
        [Test]
        [Category("ToRefactor")]
        public void CheckCurrentTsar()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();

            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
                options
                    .IgnoringCyclicReferences()
                    .Excluding((IMemberInfo info) =>
                        info.SelectedMemberInfo.DeclaringType == typeof(Person)
                        && info.SelectedMemberInfo.Name == nameof(Person.Id))
            );

            expectedTsar.Should().BeEquivalentTo(actualTsar, options =>
                options
                    .IgnoringCyclicReferences()
                    .Excluding((IMemberInfo info) =>
                        info.SelectedMemberInfo.DeclaringType == typeof(Person)
                        && info.SelectedMemberInfo.Name == nameof(Person.Id))
            );
        }

        [Test]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода? 
            // Неинформативное сообщение в случае фейла: Expected: True But was: False
            // Необходимость использования дополнительного метода AreEqual
            // Необходимость менять метод AreEqual в случае добавления / удаления полей в классе Person
            // Тестирование других классов, подобных Person, потребует своих вспомогательных методов AreEqual
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
