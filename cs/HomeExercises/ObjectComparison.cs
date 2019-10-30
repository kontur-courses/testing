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
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            var actualTsar = TsarRegistry.GetCurrentTsar();

            // Перепишите код на использование Fluent Assertions.
            actualTsar
                .Should()
                .BeEquivalentTo(expectedTsar, options => options
                    .Excluding(p => p.SelectedMemberInfo.Name == nameof(Person.Id)));
        }

        /* Недостатки:
         * 1. При добавлении новых свойств в класс Person придется каждый раз дописывать
         *    их в метод AreEqual -- лишняя работа, плюс он может вырасти до невероятных размеров
         *    и стать трудно читаемым. Например, даже сейчас в этом методе, в отличии от предыдущего,
         *    написанного с использованием Fluent Assertions, не заметно, что вообще-то у класса
         *    Person есть поле Id, которое в сравнении не участвует.
         * 2. Нет понятного сообщения об ошибке
         * 3. Можно запутаться, где actual, а где expected
         */

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода? 
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