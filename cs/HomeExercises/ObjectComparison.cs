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
			//actualTsar.Should().BeEquivalentTo(expectedTsar);
			actualTsar.Should().BeEquivalentTo(
				expectedTsar, 
				options => options
					.Including(t => t.Name)
					.Including(t => t.Age)
					.Including(t => t.Height)
					.Including(t => t.Weight)
				);
			actualTsar.Parent.Should().BeEquivalentTo(
				expectedTsar.Parent,
				options => options
					.Including(p => p!.Name)
					.Including(p => p.Age)
					.Including(p => p.Height)
					.Including(p => p.Parent)
				);
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
			/*
			 В данном решении присутствует дополнительная проверка на то,
			 что actual.Parent.Weight == expected.Parent.Weight, это в принципе делает проверки не эквивалетными,
			 несмотря на совпадение результатов проверки: если expectedTsar.Parent.Weight = 61, то первый тест зайдёт, а 
			 этот нет.
			 В данном решении нет возможности поменять условия для детей и родителей индивидуально, то есть
			 условия в строчках 60,61,63-67 должны выполняться как для child, так и для parent, в моём же решении 
			 условия для детей и родителей не совпадают. (+Гибкость)
			 Также моё решение лучше читается, в этом же подходе не очень очевидно, как работает сравнение царей.
			 (+Читаемость)
			 */
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