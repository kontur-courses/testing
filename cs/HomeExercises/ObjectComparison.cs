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
			Person actualTsar = TsarRegistry.GetCurrentTsar();
			Person expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Перепишите код на использование Fluent Assertions.
			
			actualTsar.ShouldBeEquivalentTo(expectedTsar, options => 
				options.Excluding(o => o.SelectedMemberPath.Equals("Id"))
					.Excluding(o => o.SelectedMemberPath.Equals("Parent")));
			
			expectedTsar.Parent.ShouldBeEquivalentTo(expectedTsar.Parent, options => 
				options.Excluding(o => o.SelectedMemberPath.Equals("Id"))
					.Excluding(o => o.SelectedMemberPath.Equals("Parent")));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// Недостатки у подобного подхода на мой взгляд заключаются в том, что:
			// Название теста не подходит т.к. в нем происходит проверка не всего объекта целиком, а только тех полей, которые указаны в условии
			// При добавлении новых свойств легко можно забыть внести изменения в условие
			// Тест сломается, если будет несколько вложенных объектов в исходных данных из-за рекурсии
			// По сообщению в случае если тест не пройдет будет не понятно что и где сломалось
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