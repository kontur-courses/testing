using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
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

			actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
				options.Excluding(current => current.Id)
				.Using<Person>(curTsar => curTsar.Subject.Should().BeEquivalentTo(curTsar.Expectation, personEqOptions =>
					personEqOptions
						.Using<int>(e => e.Subject.Should().Be(e.Subject))
							.When(info => info.Path.EndsWith("Id"))))
					.When(info => info.Path.EndsWith("Parent")).AllowingInfiniteRecursion()
				.AllowingInfiniteRecursion());

			//Мой метод лучше тк
			//1)Он не так чувствителен к структурным изменениям класса Person
			//2)При большом кол-ве полей выигрывает банально по количеству кода,
			//если у них не будет большое кол-во условий на поля (к примеру не учитывать Id во вложенных полях и тд)
			//3)Новым людям будет проще понять его
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

			//Лишние методы, нужные только для тестов(если сравнивать нужно только в тесте)
			//Необходимость изменять метод сравнения при структурном изменнеии класса, под который заточен метод
			//Кастомный метод сравнения может быть в разы больше чем тот который написан через FA
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