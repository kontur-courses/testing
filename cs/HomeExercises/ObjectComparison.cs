using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		Person actualTsar = TsarRegistry.GetCurrentTsar();

		Person expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			new Person("Vasili III of Russia", 28, 170, 60, null));

		[Test]
		public void CheckCurrentTsar_NameShouldBeTheSame()
		{
			actualTsar.Name.Should().Be(expectedTsar.Name);
		}

		[Test]
		public void CheckCurrentTsar_AgeShouldBeTheSame()
		{
			actualTsar.Age.Should().Be(expectedTsar.Age);
		}

		[Test]
		public void CheckCurrentTsar_HeightShouldBeTheSame()
		{
			actualTsar.Height.Should().Be(expectedTsar.Height);
		}

		[Test]
		public void CheckCurrentTsar_WeightShouldBeTheSame()
		{
			actualTsar.Weight.Should().Be(expectedTsar.Weight);
		}

		[Test]
		public void CheckCurrentTsar_ParentShouldBeTheSame()
		{
			actualTsar.Parent.Name.Should().Be(expectedTsar.Parent.Name);
			actualTsar.Parent.Age.Should().Be(expectedTsar.Parent.Age);
			actualTsar.Parent.Height.Should().Be(expectedTsar.Parent.Height);
			actualTsar.Parent.Parent.Should().Be(expectedTsar.Parent.Parent);
		}

		[Test]
		[Description("Проверка текущего царя через рефлексию типов")]
		public void CheckCurrentTsar_WithReflectionOfTypes()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			CompareTsars(actualTsar, expectedTsar);
		}

		private bool CompareTsars(Person actual, Person expected)
		{
			var personFields =
				typeof(Person).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var field in personFields)
			{
				if (field.Name.Equals("Id")) continue;
				if (field.FieldType == typeof(Person))
				{
					if (!CompareTsars(actual.Parent, expected.Parent))
						return false;
				}
				else if (field.GetValue(actual) != (field.GetValue(expected))) return false;
			}

			return true;
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		// Необходимость при каждом добавлении поля в тестируемый класс вносить изменения в переопределенный метод AreEqual
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

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