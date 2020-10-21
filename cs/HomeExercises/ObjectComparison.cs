using System.Linq;
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

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Перепишите код на использование Fluent Assertions.
			
			actualTsar.Name.Should().Be(expectedTsar.Name);
			actualTsar.Age.Should().Be(expectedTsar.Age);
			actualTsar.Height.Should().Be(expectedTsar.Height);
			actualTsar.Weight.Should().Be(expectedTsar.Weight);
			
			expectedTsar.Parent!.Name.Should().Be(actualTsar.Parent!.Name);
			expectedTsar.Parent.Age.Should().Be(actualTsar.Parent.Age);
			expectedTsar.Parent.Height.Should().Be(actualTsar.Parent.Height);
			expectedTsar.Parent.Parent.Should().Be(expectedTsar.Parent.Parent);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// Недостаток такой, что при добавлении нового поля в классе Person, придётся добавлять корректировать метод AreEquals. 
			AreEqual(actualTsar, expectedTsar).Should().BeTrue();
		}
		
		// Моё решение отличается от предыдущего тем, что в нём использована рефлексия, которая позволяет сравнивать поля классов.
		// То есть даже при добавлении нового поля, тест переписывать не придётся
		private bool AreEqual(Person? actual, Person? expected)
		{
			if (actual == expected) return true;
			if (actual == null || expected == null) return false;
			var fields = typeof(Person).GetFields();
			foreach (var fieldInfo in fields.Where((field => field.Name != "Id")))
			{
				if (fieldInfo.FieldType == typeof(Person))
				{
					if (!AreEqual((Person?)fieldInfo.GetValue(actual), (Person?)fieldInfo.GetValue(expected)))
						return false;
				}
				else
				{
					if (!fieldInfo.GetValue(actual)!.Equals(fieldInfo.GetValue(expected)))
					{
						return false;
					}
				}
			}

			return true;
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