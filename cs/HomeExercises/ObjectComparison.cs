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

            actualTsar.Should().BeEquivalentTo(
                expectedTsar,
                options => options.Excluding(member => member.SelectedMemberInfo.Name == nameof(actualTsar.Id)));
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода? 
            // Главный недостаток этого подхода: если тест упадет, то БЕЗ ДЕБАГА не понятно
            // на чем он упал - может быть на сравнении имен, а может на сравнении возрастов и т.д.
            // Еще один недостаток этого подхода: при добавлении новых полей/свойств к объектам
            // сравниваемого класса (класс Person), в тест также придется добавить проверку на равенство
            // новых полей/свойств
            // Мой подход лучше, потому что если тест упадет, то НЕ ЗАГЛЯДЫВАЯ в код теста, можно
            // понять при сравнении каких полей тест упал и посмотреть значения этих полей,
            // возможно этой информации хватит, чтобы исправить ошибку в коде
            // Также при добавлении новых полей/свойств в моем подходе придется менять код теста,
            // только если добавленные новые поля/свойства не должны сравниваться. Эта ситуация
            // происходит как правило реже, чем добавление сравниваемых полей/свойств
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
