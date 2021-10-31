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
			
			/*
			 * Такой подход гораздо лучше, чем AreEqual, так как при добавлении новый полей такое
			 * выражение сразу сравнит новые поля на эквивалентность. А AreEqual требует
			 * добавления новых полей в сам метод, что можно забыть, так как класс и метод находятся
			 * в разных местах.
			 * Единственное, что нужно сюда добавлять, дак это то, что не надо сравнивать. Но если забыть это сделать,
			 * то тест не пройдёт.
			 */
			expectedTsar.Should().BeEquivalentTo(actualTsar, opts =>
				opts.Excluding(tsar => tsar.Id).Excluding(tsar => tsar.Parent!.Id));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода?
			/*
			 * AreEqual плохо расширяемый, потому что для каждого нового поля
			 * нужно добавлять сравнение, а это можно легко забыть, приэтом тест будет
			 * проходить всё равно. Это может создать проблемы в будущем.
			 */
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