using FluentAssertions;
using FluentAssertions.Execution;
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

            // Вариант теста с возможностью изменения структуры класса Person
            // реализован в тесте CurrentTsar_ShouldBeEquivalent_WhenExpectedTsarTheSame
            #region Вариант N1. Через AssertionScope, но без возможности расширения класса Person
            using (new AssertionScope())
            {
                actualTsar.Name.Should().BeEquivalentTo(expectedTsar.Name);
                actualTsar.Age.Should().Be(expectedTsar.Age);
                actualTsar.Height.Should().Be(expectedTsar.Height);
                actualTsar.Weight.Should().Be(expectedTsar.Weight);

                actualTsar.Parent!.Name.Should().BeEquivalentTo(expectedTsar.Parent!.Name);
                actualTsar.Parent.Age.Should().Be(expectedTsar.Parent!.Age);
                actualTsar.Parent.Height.Should().Be(expectedTsar.Parent.Height);
                actualTsar.Parent.Weight.Should().Be(expectedTsar.Parent.Weight);
            }
            #endregion
        }

        [Test]
        [Description("Проверка текущего царя с возможностью изменения структуры класса Person")]
        public void ActualTsar_ShouldBeEquivalent_WhenExpectedTsarTheSame()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
                options.Excluding(x => x.SelectedMemberInfo.Name == nameof(Person.Id)));

        }

        // Основной недостаток, что мы не видим, на каком значении поля падает тест, возвращается только False;
        // Также есть жесткая связь между структурой класса Person и данным тестом.
        // Метод AreEqual yt выполняет глубокое сравнение классов и соответственно при каждом изменении класса Person
        // необходимо модифицировать этот метод в отличие от метода BeEquivalentTo
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