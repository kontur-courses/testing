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


			CheckThatPeopleAreEqual(actualTsar, expectedTsar);

			// Преимущества перед CheckCurrentTsar_WithCustomEquality:
			// - При добавлении новых значимых для сравнения нерекурсивных полей не нужно менять тест и метод CheckThatPeopleAreEqual.
			// - Это особенно важно, когда появляются поля непримитивных типов. Не придётся писать компаратор на каждый новый тип.
			// - Не нужно задумываться о наличии у классов полей переопределения Equals. BeEquivalentTo() делает это за нас.
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
			
			// Какие недостатки у такого подхода?
			// Нужно расширять метод AreEqual на каждое новое поле. Уйдёт в бесконечную рекурсию при циклических ссылках Parent.
			Assert.True(AreEqual(actualTsar, expectedTsar));
		}

		private void CheckThatPeopleAreEqual(Person? actual, Person? expected)
		{
			actual.Should().BeEquivalentTo(expected,
				options => options.Excluding(p => p.Id)
					.Excluding(p => p.Parent));
			if (actual is null || expected is null)
				return;
			CheckThatPeopleAreEqual(actual.Parent, expected.Parent);
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