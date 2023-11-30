using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		private const int MaxTreeHeight = 75;

		[SetUp]
		public void SetUp()
		{
			AssertionOptions.FormattingOptions.MaxDepth = MaxTreeHeight;
		}

		[Test]
		[Description("Проверка текущего царя")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
			actualTsar
				.Should()
				.BeEquivalentTo(expectedTsar, options =>
					options.Excluding(tsar =>
						tsar.DeclaringType == typeof(Person) && tsar.Name.Equals(nameof(Person.Id))));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		// размыта зона ответственности этого теста(если тест упадет из-за неверного поля,то не понятно из-за какого конкретно) 
		//Очень негибкий тест:
		//  Любое изменение существующих полей класса Person приводит к ошибке
		// При любом добавлении новых полей нужно снова изменять этот тест 
		// 
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

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