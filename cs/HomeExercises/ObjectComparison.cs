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
            Person? actualTsar = TsarRegistry.GetCurrentTsar();

            Person? expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Перепишите код на использование Fluent Assertions.
            Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
            Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
            Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
            Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);

            Assert.AreEqual(expectedTsar.Parent!.Name, actualTsar.Parent!.Name);
            Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
            Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
            Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            Person? actualTsar = TsarRegistry.GetCurrentTsar();
            Person? expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода? 
            Assert.True(AreEqual(actualTsar, expectedTsar));
            //Недостатки:
            //Использование внешнего метода, при изменении Person его скорее всего потребуется отредактировать
            //Отстутвие пояснений при провале: в прошлом тесте хотя-бы понятно где он провалился и из за чего, 
            //здесь тест просто провалится, без пояснений
        }

        [Test]
        public void CheckCurrentTsar_WithFluentAssertions()
        {
            Person? actualTsar = TsarRegistry.GetCurrentTsar();
            Person? expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            actualTsar.Should()
                .BeEquivalentTo(expectedTsar, config => config
                    .Excluding(tsar => tsar.Id)
                    .Excluding(tsar => tsar.Parent!.Id));
            //Особенности:
            //BeEquivalentTo сравнивает объекты структурно, в нашем случае это значит что при изменении Person изменений в тесте почти не потребуется
            //(кроме полей и свойств не участвующих в сравнении). В данном случае исключены поля Id и Parent.Id
            //Работает сильно медленнее чем предыдущий вариант, но выигрывает в удобстве пользования (расширении, анализе результатов)
            //Так же этот вариант гораздо более компактный и читаемый нежели 1 вариант
        }

        private bool AreEqual(Person? actual, Person? expected)
        {
            if (actual == expected)
            {
                return true;
            }

            if (actual == null || expected == null)
            {
                return false;
            }

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