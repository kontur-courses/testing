using System;
using FluentAssertions;
using FluentAssertions.Equivalency;
using JetBrains.Annotations;
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

            actualTsar.Should()
                .BeEquivalentTo(expectedTsar, options => options.Excluding(info =>
                    info.SelectedMemberInfo.DeclaringType == typeof(Person) &&
                    info.SelectedMemberInfo.Name == nameof(Person.Id)));
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Данный подход менее читаемый, т.к. дописан дополнительный класс AreEqual.
            // Такой подход менее расширяемый, т.е. если добавить новые поля в класс Person придётся
            // менять AreEqual,
            // а в моём решении придётся подправить только инициализацию в случае,
            // если значение поля будет задваться через конструктуор.
            // Но если добавить поле, которое мы не хотим учитывать при сравнении объектов,
            // такое как Id, то в моё решение придётся добавлять ещё один Excluding, 
            // Эту проблему можно решить вот так:  
            //actualTsar.Should().BeEquivalentTo(expectedTsar, MakeOptions(nameof(Person.Id)));

            //public Func<EquivalencyAssertionOptions<Person>, EquivalencyAssertionOptions<Person>> MakeOptions(
            // params string[] fields)
            //{
            // Func<EquivalencyAssertionOptions<Person>, EquivalencyAssertionOptions<Person>> predFunc = options =>
            // {
            //  foreach (var field in fields)
            //  {
            //   options.Excluding(info =>
            //    info.SelectedMemberInfo.DeclaringType == typeof(Person) &&
            //    info.SelectedMemberInfo.Name == field);
            //  }
            //  return options;
            // };
            // return predFunc;
            //}
            // Но такой код не становится более читаемым, но зато расширяемость возрастает.
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