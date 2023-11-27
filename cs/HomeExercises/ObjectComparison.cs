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

            actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
                options
                .Excluding(tsar => tsar.SelectedMemberInfo.Name == nameof(Person.Id))
                .AllowingInfiniteRecursion());
        }

        // Недостаток в том, что нужно будет переписывать тест при любом исправлении в классе Person.
        // Возможно нужно добавить какое-то поле, или изменить название уже существующего,
        // придется изменять каждый раз метод AreEqual, так же снижается читаемость теста,
        // так как приходится разбираться еще в методе AreEqual
        // Мое решение же лучше, потому что при добавлении новых полей нужно добавлять только те,
        // которые необходимо игнорировать при сравнении классов, плюс гораздо локаничнее и читаемее
        // Если передать в тест царя, который ссылается сам на себя, то его выполнение будет прервано из-за 
        // Stack overflow, то есть переполнения стека рекурсии. 
        // FluentAssertions видит возможную циклическую ссылку объекта на самого себя и выдает ошибку
        // Cyclic reference to type HomeExercises.Person detected
        // При разных царях тест не показывает, в чем отличие и поэтому тяжело отловить ошибку, если же упадет тест 
        // с FluentAssertions, он показывает, где именно была ошибка и в чем она заключается
        // например Expected member Age to be 53, but found 54.
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
            return new Person("Ivan IV The Terrible", 54, 170, 70,
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