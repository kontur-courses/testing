using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using HomeExercisesExtensions;
using HomeExercises;


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

			actualTsar.ShouldBe(expectedTsar);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// 1) Он не расширяем, при добавлении/удаленни полей придется изменять вручную метод AreEqual
			// 2) Вместо этого логичне было бы сделать метод Equals в класее Person
			// 3) Рекурсия ничем не ограничена, 
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

namespace HomeExercisesExtensions
{
	public static class PersonExtensions
	{
		public static void ShouldBe(this Person actual, Person expected, int depthLevel = 0)
		{
			if (depthLevel == 2)
				return;
			
			foreach (var field in typeof(Person).GetFields()
				                                .Where(f => f.Name != "Id"))
			{
				var actualFieldValue = field.GetValue(actual);
				var expectedFieldValue = field.GetValue(expected);
				if (field.FieldType == typeof(Person))
				{
					var actualPersonField = (Person) field.GetValue(actual);
					var expectedPersonField = (Person) field.GetValue(expected);
					actualPersonField.ShouldBe(expectedPersonField, ++depthLevel);
				}
				else
					actualFieldValue.Should().Be(expectedFieldValue);
			}
		}
	}
}