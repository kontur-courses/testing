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

			actualTsar
				.Should()
				.BeEquivalentTo(expectedTsar, options =>
					options
						.AllowingInfiniteRecursion()
						.Excluding(person => person.SelectedMemberInfo.Name.Equals(nameof(Person.Id))));
		}

		/*
		* Достоинства CheckCurrentTsar:
		* 1) При изменении полей класса Person не придется переписывать много кода
		* 2) По результату теста можно сказать где он упал
		* (мы сможем сказать, что у царей были разные имена или возраст или ...)
		*
		* Недостатки CheckCurrentTsar_WithCustomEquality вытекают из достоинств CheckCurrentTsar,
		* которые CheckCurrentTsar_WithCustomEquality не имеет.
		 * Также на метод AreEqual не написаны тесты.
		   */

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

		private bool AreEqual(Person actual, Person expected)
		{
			if (actual == expected)
			{
				return true;
			}

			if (actual == null || expected == null)
			{
				return false;
			}

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
		public static int IdCounter;
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