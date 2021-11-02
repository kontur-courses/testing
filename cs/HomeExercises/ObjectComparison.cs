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

			//Правильно ли я понял, что мы сравниваем только по сыну и отцу?...

			actualTsar.Should().BeEquivalentTo(expectedTsar, cfg => cfg
			.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
			.ExcludingNestedObjects()
			.Excluding(x => x.SelectedMemberPath.EndsWith("Parent")));

			actualTsar.Parent.Should().BeEquivalentTo(expectedTsar.Parent, cfg => cfg
			.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
			.ExcludingNestedObjects()
			.Excluding(x => x.SelectedMemberPath.EndsWith("Parent")));

			//Данный код лучше альтернативного решения тем, что нам:
			//1. Не нужно каждый раз переписывать метод AreEqual при изменении Person
			//2. Если альтернативный тест не пройдёт, мы не узнаем, какое именно поле Person это вызвало,
			//т.е. не узнаем, почему этот тест не проходит. Всё, что мы получим:
			//Expected: True
			//But was:  False
			//Что не даёт практически никакой информации.
			//Такой же вариант достаточно легко читается, мы сразу понимаем, что не так, если что-то не так
			//и расширение происходит автоматически при добавлении новых полей


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