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

			/*
			 *  Данный тест рекурсивно пройдется по всем полям объекта и сравнит их по значению рекурсивно
			 *  По умолчанию рекурсия проходит на глубину 10, для того чтобы снять ограничение
			 *  используется опция AllowingInfiniteRecursion, но она может стать бесконечной, если цепь проверки замкнется.
			 *  Мой способ лучше тем, что в случае добавления новых полей в объект Person, будут проверяться все поля без изменения теста.
			 *  Изменения нужно будет вносить только в случае если мы хотим убрать какие-то поля из проверки.
			 */

			actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
				options.Excluding(o => o.SelectedMemberInfo.Name == nameof(Person.Id) &&
				                       o.SelectedMemberInfo.DeclaringType == typeof(Person))
					.IgnoringCyclicReferences());
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
			/*
			 * Какие недостатки у такого подхода?
			 * 1. Придется менять метод AreEqual, если в объект добавятся еще поля.
			 * 2. Рекурсия может стать бесконечной, если в каком-то Person Parent будет равен Person, который уже был в цепи проверки
			 * 3. Имя теста никак не сигнализирует о том какой особый случай оно проверяет
			 * 4. Тест никак не показывает в чем проблема, тест просто становится красным
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
		public static int IdCounter;
		public int Age, Height, Weight;
		public int Id;
		public string Name;
		public Person? Parent;

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