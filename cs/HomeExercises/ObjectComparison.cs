using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	[Description("Проверка текущего царя")]
	public class ObjectComparison_Tests
	{
		private Person actualTsar;
		private Person expectedTsar;

		[SetUp]
		public void SetUp()
		{
			actualTsar = TsarRegistry.GetCurrentTsar();
			expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
		}

		[Test]
		public void GetCurrentTsar_ShouldBeRight_AfterCreation()
		{
			actualTsar.Should().BeEquivalentTo(expectedTsar, config => config.Excluding(info => 
				info.SelectedMemberInfo.Name == nameof(Person.Id)));
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

		// Если изменится только одно поле, то будет сложнее понять из-за чего тест завалился т.к. все поля проверяются в одном тесте вместе.
		// Также, если тест завалится, то невозможно понять, на каком поколении это произошло

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