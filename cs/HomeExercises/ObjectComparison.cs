using FluentAssertions;
using NUnit.Framework;
using System.Reflection;
using System.Linq;

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

			CheckPerson(actualTsar, expectedTsar);
		}

		private void CheckPerson(Person? actual, Person? expected)
        {
			if (actual == expected)
				return;
			actual.Should().NotBeNull();
			expected.Should().NotBeNull();

			var ignoredFields = new string[] { "Id", "IdCounter" };
			typeof(Person).GetFields()
				.Where(f => !ignoredFields.Contains(f.Name))
				.ToList()
				.ForEach(f => CompareValues(f, actual, expected));
        }

		private void CompareValues(FieldInfo field, Person actual, Person expected)
        {
			var expectedValue = typeof(Person).GetField(field.Name)
				.GetValue(expected);
			var actualValue = field.GetValue(actual);
			if (field.Name == "Parent") 
				CheckPerson((Person?)actualValue, (Person?)expectedValue);
			else
				actualValue.Should().Be(expectedValue);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?" +
			"Недостаток по сравнению с моим тестом - его сложнее поддерживать" +
			"При изменениях в классе Person он будет ломаться" +
			"либо становиться не полным." +
			"Уверен у этого подхода есть еще подводные камни" +
			"но я их не увидел.")]
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