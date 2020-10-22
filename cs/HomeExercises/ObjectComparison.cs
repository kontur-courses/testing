using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		/// <summary>
		/// Альтернативное решение не даёт информации о том, какие данные были неверными.
		/// Может вызвать переполнение стека вызовов.
		/// При добавлении нового поля в классе Person нужно добавлять проверку в методе AreEqual.
		///
		/// FluentAssertions позволяет избавиться от вложенных вызовов методов.
		/// Например: Assert.True(AreEqual(actualTsar, expectedTsar)).
		/// Should() явно указывает на актуальное и ожидаемое значения.
		/// Сообщение о неверном результате предоставляет полную информацию.
		/// При добавлении нового поля типа Person его нужно сравнивать аналогично полю Parent.
		/// Например: при добавлении Person Child.
		/// В остальных случаях при добавлении нового поля изменения в тесте не требуются.
		/// </summary>
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar, ExcludeEquivalencyOptions);

			CheckTsarParents(actualTsar, expectedTsar);
		}

		private static void CheckTsarParents(Person actualTsar, Person expectedTsar)
		{
			var actualTsarParent = actualTsar.Parent;
			var expectedTsarParent = expectedTsar.Parent;
			while (actualTsarParent != null || expectedTsarParent != null)
			{
				actualTsarParent.Should().BeEquivalentTo(expectedTsarParent, ExcludeEquivalencyOptions!);
				actualTsarParent = actualTsarParent?.Parent;
				expectedTsarParent = expectedTsarParent?.Parent;
			}
		}

		private static EquivalencyAssertionOptions<Person> ExcludeEquivalencyOptions(EquivalencyAssertionOptions<Person> options)
		{
			return options
				.Excluding(o => o.Id)
				.Excluding(o => o.Parent);
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