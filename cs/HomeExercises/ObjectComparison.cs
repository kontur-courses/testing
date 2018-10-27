using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
        private Person actualTsar;
		private const string CharacteristicsCategory = "TsarCharacteristics";
		private const string ParentCharacteristicsCategory = "TsarParentCharacteristics";

        [SetUp]
		public void SetUp()
		{
			actualTsar = TsarRegistry.GetCurrentTsar();
		}

		[Test]
		[Category(CharacteristicsCategory)]
        public void CheckCurrentTsarName()
		{
			const string expectedName = "Ivan IV The Terrible";
			actualTsar.Name.Should().Be(expectedName);
		}

		[Test]
		[Category(CharacteristicsCategory)]
        public void CheckCurrentTsarAge()
		{
			const int expectedAge = 54;
			actualTsar.Age.Should().Be(expectedAge);
		}

		[Test]
		[Category(CharacteristicsCategory)]
        public void CheckCurrentTsarHeight()
		{
			const int expectedHeight = 170;
			actualTsar.Height.Should().Be(expectedHeight);
		}

		[Test]
		[Category(CharacteristicsCategory)]
        public void CheckCurrentTsarWeight()
		{
			const int expectedWeight = 70;
			actualTsar.Weight.Should().Be(expectedWeight);
		}

		[Test]
		[Category(ParentCharacteristicsCategory)]
		public void CheckCurrentTsarParentName()
		{
			const string expectedName = "Vasili III of Russia";
			actualTsar.Parent.Name.Should().Be(expectedName);
		}

		[Test]
		[Category(ParentCharacteristicsCategory)]
		public void CheckCurrentTsarParentAge()
		{
			const int expectedAge = 28;
			actualTsar.Parent.Age.Should().Be(expectedAge);
		}

		[Test]
		[Category(ParentCharacteristicsCategory)]
        public void CheckCurrentTsarParentHeight()
		{
			const int expectedHeight = 170;
			actualTsar.Parent.Height.Should().Be(expectedHeight);
		}

		[Test]
		[Category(ParentCharacteristicsCategory)]
        public void CheckCurrentTsarParentWeight()
		{
			const int expectedWeight = 60;
			actualTsar.Parent.Weight.Should().Be(expectedWeight);
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
		}
		// Недостаток такого подхода заключается в том, что мы не узнаем на каком из полей не прошло сравнение,
        // при ошибке в одном поле, не проверим остальные, и при каждом тесте должны проверять абсолютно все поля,
		// создавать нового Царя, следовательно при расширении класса Царя придется изменять все тесты

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