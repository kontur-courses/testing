using FluentAssertions;
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
			
			actualTsar
				.Should()
				.BeEquivalentTo(expectedTsar, options => options
					.Excluding(memberInfo => memberInfo.SelectedMemberInfo.DeclaringType == typeof(Person) 
					                     && memberInfo.SelectedMemberInfo.Name == nameof(Person.Id)));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		/* 1. Непонятно из названия, что тест проверяет и какого результата ожидает.
		 * 2. При использовании такого Assert, при падении теста мы узнаем лишь, что мы получили False,
		 *	  без дополнительной информации об отличающихся полях и т.д.
		 * 3. При изменении класса Person нам всегда придётся соответственно менять и локальный метод
		 *    AreEqual для класса Person.
		 * 4. Если каким-то образом произойдёт бесконечная рекурсия внутри класса Person, то данный способ привдёт к
		 *	  бесконечному проходу по нему. Проверка через FluentAssertions ограничена глубиной
		 *    в 10 вхождений и это значение кастомизируемо.
		 * 5. При добавлении новых полей в класс Person, которые мы хотим включать в этот Equals - метод может
		 *    сильно разрастись и его читаемость сильно снизится.
		 */
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
			return new Person("Ivan IV The Terrible", 54, 170, 70,
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