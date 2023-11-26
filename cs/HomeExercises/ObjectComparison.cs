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
			 * Чем это решение лучше решения в CheckCurrentTsar_WithCustomEquality:
			 * 1. Это решение не нужно будет переписывать при добавлении/удалении в класс/из класса Person полей.
			 * 2. В этом решении наглядно видно, какое именно поле не учитывается при сравнении объектов.
			 * 3. При падении теста мы увидим, какие именно поля объектов не прошли проверку на равенство.
			 */
			actualTsar
				.Should()
				.BeEquivalentTo(expectedTsar, options => options
					.Excluding(info => info.SelectedMemberInfo.Name.Equals(nameof(Person.Id))));
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
			 * 1. Если мы решим изменить класс Person (например, добавить или удалить какие-то поля),
			 * то тогда придется переписывать метод AreEqual
			 * 2. Если тест не пройдет, то мы не увидим, какие именно поля у двух объектов не совпали.
			 * Нам просто выдаст сообщение 'Expected: True, But was: False'
			 */
			
			Assert.True(AreEqual(actualTsar, expectedTsar));
		}

		private static bool AreEqual(Person? actual, Person? expected)
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

	public abstract class TsarRegistry
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
		private static int _idCounter;
		public readonly int Age;
		public readonly int Height;
		public readonly int Weight;
		public readonly string Name;
		public readonly Person? Parent;
		public int Id;

		public Person(string name, int age, int height, int weight, Person? parent)
		{
			Id = _idCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}