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

			// Во время чтения названия методов сразу понятно, что здесь проверяется
			// и каким образом. Помимо этого, при таком подходе не придётся
			// делать никаких изменений в тесте, если мы вводим поле, которое нужно проверять,
			// так как такая проверка по умолчанию сравнивает все поля, кроме тех, которые мы
			// указали отдельно. Также нам не нужно смотреть никакие доп методы, вся информация
			// указана прямо здесь, в тесте. Мы сразу узнаем точное место, на котором тест упал.
			actualTsar.Should().BeEquivalentTo(expectedTsar,
				options => options
					.Excluding(person => person.SelectedMemberInfo.Name == nameof(Person.Id)));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// Из названия метода не ясно, по какому именно принципу работает
			// AreEqual. Какие поля он учитывает, как именно сравнивает.
			// Помимо этого, название метода похоже на название методов из классов тестирования,
			// что само по себе уже вводит в недоумение.
			// Ещё важно, что при падении теста не ясно, что именно не совпало, и где есть "неравенство".
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