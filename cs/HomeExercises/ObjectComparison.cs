using System.Configuration;
using FluentAssertions;
using FluentAssertions.Equivalency;
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
			actualTsar.Should().BeEquivalentTo(expectedTsar, config =>
				config.AllowingInfiniteRecursion().Excluding(memberInfo => memberInfo.SelectedMemberInfo.Name.Contains("Id")));
			// SelectedMemberInfo.Name == "Id", но пользователь при расширении может добавить другое поле id
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			//Какие недостатки у такого подхода? 
			//1) В методе AreEqual подробно сравнивается каждое поле класса Parent, что запрещает
			//   менять поля класса без вмешательства в код(т.к. эти поля не будут учитываться в сравнении)
			//
			//2) Сам по себе код Assert.True(AreEqual()) менее читаем чем в FluentAssertions ShouldBeEquivalentTo()-должно быть эквивалентно
			//
			//3) Вывод ошибки в тестах будет более понятен, т.к вывод AreEqual: "Expected: True But was:  False"
			//   FluentAssertions более понятный вывод: "Expected member Parent 'a',but found 'e'",что ускоряет поиск проблемного участка кода
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
		private static int idCounter;
		public int Age, Height, Weight;
		public string Name;
		public Person? Parent;
		public int Id;

		public Person(string name, int age, int height, int weight, Person? parent)
		{
			Id = idCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}