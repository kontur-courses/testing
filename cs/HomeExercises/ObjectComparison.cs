using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		public void GetCurrentTsar_CurrentTsarEqualsHistoricalTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar
				.Should()
				.BeEquivalentTo(expectedTsar, options =>
					options.Excluding(property =>
						property.SelectedMemberInfo.DeclaringType == typeof(Person) &&
						property.SelectedMemberInfo.Name.Equals(nameof(Person.Id))));
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
			 * 1) Если тест не прошел, не будет возможности понять, какие именно поля не равны - тест просто вернет false
			 * 2) При изменении класса Person необходимо будет изменять тест
			 * 3) Название теста не раскрывает смысла теста, не работает как спецификация
			 * 4) Проверку объектов на равенство лучше внести в класс
			 * 5) При наличии кольцевой ссылки кастомный тест уйдет в бесконечную рекурсию,
			 *		а тест на FluentAssertions вернет false и укажет на наличие цикла
			 */
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