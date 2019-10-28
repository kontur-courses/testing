using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using static System.Reflection.BindingFlags;

namespace HomeExercises
{
	public class ObjectComparison
	{
		private static readonly Person parentTsar = new Person("Vasili III of Russia", 28, 170, 60, null);

		private readonly Person actualTsar = TsarRegistry.GetCurrentTsar();

		private readonly Person expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			parentTsar);

		[Test]
		public void CheckCurrentTsar_WithTheSameTsars_GoesOK()
		{
			actualTsar.Should().BeEquivalentTo(expectedTsar,
				opts => opts.Excluding(field => field.SelectedMemberInfo.Name.Equals("Id")));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		// Необходимость при каждом добавлении поля в тестируемый класс вносить изменения в метод AreEqual
		// При упавшем тесте нам самим придется искать причину, нет ни стак трейса, ни конкретной инфы в самом названии
		public void CheckCurrentTsar_WithCustomEquality()
		{
			// Какие недостатки у такого подхода?
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