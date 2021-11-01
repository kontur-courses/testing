﻿using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class PersonComparison
    {
        [Test]
        [Description("Проверка текущего царя")]
        public void CheckCurrentTsar()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
                .Excluding(o => o.Path.EndsWith("Id")));
        }

        [Test]
        [Description("Проверка текущего царя / Нужно внести изменения в метод AreEqual!")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            /*
            НЕДОСТАТКИ:
            1. Чтобы тест продолжал быть актуальным,
            при добавлении полей или свойств в класс Person,
            нужно также не забыть внести изменения в метод AreEqual.

            2. (общий) Полезнее будет переопределить метод Equals для Person.
            И в случае, когда мы не являемся одноверенно авторами тестируемого класса,
            лучше использовать метод Equals потому, что именно он будет использоваться
            для сравнения объектов класса в коде, а не наш тестовый "AreEqual".
            Наша проверка не учитывает то, как объекты должны сравниваться по задумке автора класса.
             
            3. В методе AreEqual много условий - есть простор для ошибки.
            При добавлении полей или свойств в Person, его понтенциально будет тяжело читать  
            
            5. Малоинформативное сообщение при падении теста
            */

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
        public Person? Parent;
        public int Id;

        public Person(string name, int age, int height, int weight, Person? parent)
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