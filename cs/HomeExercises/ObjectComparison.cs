using System.Data;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class TsarInitializingTester
    {
        private Person actualTsar;
        private Person expectedTsar;

        [OneTimeSetUp]
        public void Init_Tsars()
        {
            actualTsar = TsarRegistry.GetCurrentTsar();

            expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));
        }

        [Test]
        public void GetCurrentTsar_NamesShouldBeEqual()
        {
            actualTsar.Name.Should().Be(expectedTsar.Name);
        }

        [Test]
        public void GetCurrentTsar_AgesShouldBeEqual()
        {
            actualTsar.Age.Should().Be(expectedTsar.Age);
        }

        [Test]
        public void GetCurrentTsar_HeightsShouldBeEqual()
        {
            actualTsar.Height.Should().Be(expectedTsar.Height);
        }

        [Test]
        public void GetCurrentTsar_WeightsShouldBeEqual()
        {
            actualTsar.Weight.Should().Be(expectedTsar.Weight);
        }

        [Test]
        public void GetCurrentTsar_ParentsNamesShouldBeEqual()
        {
            actualTsar.Parent.Name.Should().Be(expectedTsar.Parent.Name);
        }



        [Test]
        public void GetCurrentTsar_ParentsHeightsShouldBeEqual()
        {
            actualTsar.Parent.Height.Should().Be(expectedTsar.Parent.Height);
        }

        [Test]
        public void GetCurrentTsar_ParentsWeightsShouldBeEqual()
        {
            actualTsar.Parent.Weight.Should().Be(expectedTsar.Parent.Weight);
        }

        // Но как быть с предком?

        [OneTimeTearDown]
        public void Clear()
        {
            actualTsar = null;
            expectedTsar = null;
        }

    }
    public class ObjectComparison
    {

	    [Test]
	    public void CheckCurrentTsar_Fixed()
	    {
		    var actualTsar = TsarRegistry.GetCurrentTsar();
		    var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			    new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.ShouldBeEquivalentTo(expectedTsar, options => 
				options.ExcludingNestedObjects());
	    }
        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Анти-паттерн The-One.
            // Единовременно тестируется множество различных характеристик
            // Плохая читаемость кода, неочевидность тестируемых свойств объекта
            // При изменении объекта Person неудобно вносить изменения в тест
            // Одна упавшая проверка в начале блокирует проверку остальных свойств
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