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

			actualTsar.Should().BeEquivalentTo(
				expectedTsar, 
				options => options
					.Excluding(t => t.Id)
					.Excluding(t => t!.Parent!.Id)
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
			 - читаемость: не очевидно, как работает AreEqual, 
			 также можно подумать, что тест проверяет метод AreEqual;
			 (в моём решении строк меньше, читаемость лучше засчёт FluentAssertions)
			 
			 - гибкость: после добавления новых полей в Person 
			 надо будет дополнить условие в методе AreEqual;
			 (в моём решении, если поле надо учитывать при сравнении, то ничего дописывать не надо)
			 
			 - возможно исключение StackOverflowException
			 (в моём решении есть даже настройка рекурсии при сравнении)

			 - мне кажется, что далеко не лучший вариант писать,
			 а затем использовать методы с непростой логикой, как в данном случае, возможно, стоит этот метод потестить,
			 чтобы корректность метода AreEqual была актульна при смене контекста
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