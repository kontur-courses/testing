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

            /* Если нужно увеличить глубину проверки, можно создать цикл,
             * но в данном случае код лишь разрастётся и станет менее понятным
             */
            actualTsar.ShouldBeEquivalentTo(expectedTsar, options => options
                .Excluding(ts => ts.Id)
                .Excluding(ts => ts.Parent));

            actualTsar.Parent.ShouldBeEquivalentTo(expectedTsar.Parent, options => options
                .Excluding(ts => ts.Id));

        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода?

            /* 1. Если Assert завершился неудачей, непонятно, из-за какого именно из полей
             * провалилось условие: само сообщение о неудаче не сообщает никакой
             * полезной информации, а только то, что тест "зафэйлился".
             * 2. При добавлении новых полей в Person можно забыть поменять метод AreEqual.
             */
            Assert.True(AreEqual(actualTsar, expectedTsar));
        }

        private bool AreEqual(Person actual, Person expected)
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

    public class TsarRegistry
    {
        public static Person GetCurrentTsar()
        {
            return new Person(
                "Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));
        }
    }

    public class Person
    {
        public static int IdCounter = 0;
        public int Age, Height, Weight;
        public string Name;
        public Person Parent;
        public int Id;

        public Person(string name, int age, int height, int weight, Person parent)
        {
            Id = IdCounter++;
            Name = name;
            Age = age;
            Height = height;
            Weight = weight;
            Parent = parent;
        }
    }
}