using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	[Description("Проверка текущего царя")]
	public class ObjectComparison_Should
	{
		private Person actualTsar;

		[SetUp]
		public void SetUp()
		{
			actualTsar = TsarRegistry.GetCurrentTsar();
		}

		[Test]
		public void GetCurrentTcar_NameShouldBeRight_AfterCreation()
		{
			actualTsar.Name.Should().Be("Ivan IV The Terrible");
		}

		[Test]
		public void GetCurrentTcar_AgeShouldBeRight_AfterCreation()
		{
			actualTsar.Age.Should().Be(54);
		}

		[Test]
		public void GetCurrentTcar_HeightShouldBeRight_AfterCreation()
		{
			actualTsar.Height.Should().Be(170);
		}

		[Test]
		public void GetCurrentTcar_WeightShouldBeRight_AfterCreation()
		{
			actualTsar.Weight.Should().Be(70);
		}

		[Test]
		public void GetCurrentTcar_ParentNameShouldBeRight_AfterCreation()
		{
			actualTsar.Parent.Name.Should().Be("Vasili III of Russia");
		}

		[Test]
		public void GetCurrentTcar_ParentAgeShouldBeRight_AfterCreation()
		{
			actualTsar.Parent.Age.Should().Be(28);
		}

		[Test]
		public void GetCurrentTcar_ParentHeightShouldBeRight_AfterCreation()
		{
			actualTsar.Parent.Height.Should().Be(170);
		}

		[Test]
		public void GetCurrentTcar_ParentWeightShouldBeRight_AfterCreation()
		{
			actualTsar.Parent.Weight.Should().Be(60);
		}

		[Test]
		public void GetCurrentTcar_ParentOfParentShouldBeNull_AfterCreation()
		{
			actualTsar.Parent.Parent.Should().BeNull();
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

		// Если изменится только одно поле, то будет сложнее понять из-за чего тест завалился т.к. все поля проверяются в одном тесте вместе

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