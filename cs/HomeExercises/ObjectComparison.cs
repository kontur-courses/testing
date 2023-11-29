using FluentAssertions;
using NUnit.Framework;
using System.Net;

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

            // сравнение родителей такое же как и в коде, который нужно было отрефакторить - сравнивает по ссылке,
            // а значит при создании экземпляра для сравнения, если родитель родителя будет не null,
            // то будет ошибка, если это не ошибка в тесте, а так и должно быть,
            // то реализация CheckCurrentTsar_WithCustomEquality работает неправильно,
            // тк в таком случае она засчитает тест
            // также данная реализация CheckCurrentTsar не сравнивает поле Weight для родителя,
            // также как и оригинальный код, из-за чего данная реализация
            // будет показывать результат отличный от CheckCurrentTsar_WithCustomEquality при
            // небольших изменениях в TsarRegistry или в экземпляре expectedTsar

            actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
            .Excluding(tsar => tsar.Id)
            .Excluding(tsar => tsar.Parent));
            expectedTsar.Parent.Should().BeEquivalentTo(actualTsar.Parent!, options => options
            .Excluding(parent => parent.Id)
            .Excluding(parent => parent.Weight));

            // данная реализация лучше CheckCurrentTsar_WithCustomEquality тем, что:
            // 1) при ошибке в тесте, выводится объяснение того, что пошло не так
            // 2) код теста легко читается - методы записаны последовательно, и что они делают понятно из названия
            // 3) название теста на прямую отражает происходящее в коде и соответствует выводимому результату
            // 4) более удобная в плане расширяемости

        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода? 
            // 1) функциональность теста разбита на 2 метода, усложняя его читаемость
            // 2) тест уже посути не сравнивает царей, а проверяет работу метода AreEqual
            // 3) отсутствие пояснения - только выкинет "ожидается true, а было false" при ошибке
            // 4) такая реализация AreEqual может вызвать переполнение стэка, тк сравнение родителей рекурсивно
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