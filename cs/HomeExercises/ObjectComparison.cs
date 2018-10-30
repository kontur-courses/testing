using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
										  new Person("Vasili III of Russia", 28, 170, 60, null));
			actualTsar.Should()
					  .BeEquivalentTo(expectedTsar, options => options.ExcludeMember(nameof(Person.Id)));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
										  new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода?
			// ##### Cмотри приложенный файл Disadvantages.md #####
			Assert.True(AreEqual(actualTsar, expectedTsar));
		}

		private bool AreEqual(Person actual, Person expected)
		{
			if (actual == expected) return true;
			if (actual == null || expected == null) return false;
			return actual.Name == expected.Name &&
				   actual.Age == expected.Age &&
				   actual.Height == expected.Height &&
				   actual.Weight == expected.Weight &&
				   AreEqual(actual.Parent, expected.Parent);
		}
	}

	public static class FluentAssertionsOptionsExtensions
	{
		public static EquivalencyAssertionOptions<TDeclaring> ExcludeMember<TDeclaring>(
			this EquivalencyAssertionOptions<TDeclaring> options,
			string fieldName)
		{
			return options.Excluding(info => info.SelectedMemberInfo.Name.Equals(fieldName) &&
											 info.SelectedMemberInfo.DeclaringType == typeof(TDeclaring));
		}
	}

	public class TsarRegistry
	{
		public static Person GetCurrentTsar()
		{
			return new Person("Ivan IV The Terrible", 54, 170, 70,
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

		public Person(
			string name,
			int age,
			int height,
			int weight,
			Person parent)
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
