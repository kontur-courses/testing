using System;
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
			var expectedTsar = new Person(
				"Ivan IV The Terrible", 54, 170, 70, new DateTime(1530, 08, 25),
				new Person("Vasili III of Russia", 28, 170, 60, new DateTime(1479, 03, 25), null));

			actualTsar.Should().BeEquivalentToPersonIgnoringIdentifiers(expectedTsar);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person(
				"Ivan IV The Terrible", 54, 170, 70, new DateTime(1530, 08, 25),
				new Person("Vasili III of Russia", 28, 170, 60, new DateTime(1479, 03, 25), null));

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
				"Ivan IV The Terrible", 54, 170, 70, new DateTime(1530, 08, 25),
				new Person("Vasili III of Russia", 28, 170, 60, new DateTime(1533, 12, 3), null));
		}
	}

	public class Person
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person? Parent;
		public int Id;
		public DateTime DayOfBirth;

		public Person(string name, int age, int height, int weight, DateTime dayOfBirth, Person? parent)
		{
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			DayOfBirth = dayOfBirth;
			Parent = parent;
		}
	}
}