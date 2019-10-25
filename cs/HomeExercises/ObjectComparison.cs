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
			/*
			 * 1) Теперь, при возникновении ошибок, будет явно видно,
			 * в каком из полей идёт расхождение с образцом.
			 * 2) Благодаря because мы сразу же можем увидеть,
			 * почему требуется равенство свойств. В данном случае,
			 * потому что текущий царь (по версии теста) - Иван IV Грозный
			 * 3) Благодаря опции ExcludingMissingMembers, мы можем принимать
			 * различные классы, наследующиеся от Person, но, при этом,
			 * сравнивать их лишь по свойствам Person. Например, если создать класс
			 * Tsar, унаследовать его от Person и добавить ему новые свойства, например,
			 * года правления, то этот тест всё равно будет работать корректно. 
			 */
			actualTsar.ShouldBeEquivalentTo(expectedTsar, options =>
				options.ExcludingMissingMembers()
					.Excluding(o => o.Id)
					.Excluding(o => o.Parent.Id),
			"Ivan IV The Terrible is current tsar");
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
			 * 1) Невозможно определить конкретную причину ошибки теста - мы лишь знаем,
			 * что объекты не одинаковы, но такой информации не достаточно. Нужно знать конкретные
			 * отличающиеся свойства.
			 * 2) При возникновении новых свойств придётся постоянно дополнять метод AreEqual,
			 * что приводит к дополнительной трате времени. 
			 */
			Assert.True(AreEqual(actualTsar, expectedTsar));
		}

		private bool AreEqual(Person actual, Person expected)
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