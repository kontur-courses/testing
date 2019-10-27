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
            // Перепишите код на использование Fluent Assertio
            actualTsar.Name.Should().Be(expectedTsar.Name);
            actualTsar.Age.Should().Be(expectedTsar.Age);
            actualTsar.Height.Should().Be(expectedTsar.Height);
            actualTsar.Weight.Should().Be(expectedTsar.Weight);
            expectedTsar.Parent.Name.Should().Be(actualTsar.Parent.Name);
            expectedTsar.Parent.Age.Should().Be(actualTsar.Parent.Age);
            expectedTsar.Parent.Height.Should().Be(actualTsar.Parent.Height);
            expectedTsar.Parent.Parent.Should().Be(actualTsar.Parent.Parent);
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));


            //В таком подходе в независимости от количества полей класса person можно сравнивать объекты этого класса по всем возможным полям
            //Так же можно указать те поля, которые сравнивать не нужно
            //Пишется намного меньше кода, однако читаемость у него такая себе
            actualTsar.ShouldBeEquivalentTo(expectedTsar, opitons => opitons.Excluding(x => x.SelectedMemberPath.EndsWith("Id")));
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