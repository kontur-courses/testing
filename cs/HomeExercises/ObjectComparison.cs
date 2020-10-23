using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class ObjectComparison
    {
        private int ancestorLevel;

        [Test]
        [Description("Проверка текущего царя")]
        [Category("ToRefactor")]
        public void CheckCurrentTsar()
        {
            ancestorLevel = 0;
            var actualTsar = TsarRegistry.GetCurrentTsar();

            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            CheckPerson(actualTsar, expectedTsar);
        }

        private void CheckPerson(Person? actualPerson, Person? expectedPerson)
        {
            if (actualPerson == expectedPerson)
                return;
            var stringParentsForFault = string.Concat(Enumerable.Repeat(".Parent", ancestorLevel));
            if (actualPerson == null || expectedPerson == null)
            {
                actualPerson.Should().NotBeNull($"expectedPerson{stringParentsForFault} is not null");
                actualPerson.Should().BeNull($"expectedPerson{stringParentsForFault} is null");
            }

            var fields = typeof(Person).GetFields();
            foreach (var field in fields.Where(x => x.Name != "Id"))
                if (field.FieldType == typeof(Person))
                {
                    ancestorLevel++;
                    CheckPerson((Person?) field.GetValue(actualPerson),
                        (Person?) field.GetValue(expectedPerson));
                }
                else
                {
                    field.GetValue(actualPerson).Should().Be(field.GetValue(expectedPerson),
                        $"actualPerson{stringParentsForFault}.{field.Name}" +
                        $" should be equals to expectedPerson{stringParentsForFault}.{field.Name}");
                }
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода? 
            Assert.True(AreEqual(actualTsar, expectedTsar));
            /*1. Плохо расширяем, при добавлении свойств в Person, придется добавлять проверки в этот тест
              2. Если 2 объекта не равны, то непонятно будет, в чем именно они не равны
              У моего теста этих недостатков нет*/
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
        public static int IdCounter;
        public int Age, Height, Weight;
        public int Id;
        public string Name;
        public Person? Parent;

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