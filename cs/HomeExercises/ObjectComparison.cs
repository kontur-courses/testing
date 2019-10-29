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
		public void GetCurrentTsar_AlwaysShouldReturn_IvanIVTheTerrible()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			                              new Person("Vasili III of Russia", 28, 170, 60, null));

			const string ignoredMemberName = "Id";

			actualTsar.Should().BeEquivalentTo(expectedTsar, options => options.Excluding(
				                                   person => person.SelectedMemberInfo.Name == ignoredMemberName),
			                                   "TsarRegistry contains only one person - Ivan IV The Terrible");
		}
		
		/*
		 * Недостатки:
		 * 1. Информативность - имя теста не соответствует популярным конвенциям и не даёт информации о тестируемом
		 * 	  модуле и ожидаемом поведении.
		 *    Пример хорошей конвенции: [UnitOfWork_StateUnderTest_ExpectedBehavior].
		 * 2. Расширяемость - при добавлении новых членов в Person, нам придётся переписывать тесты.
		 * 3. Читаемость - "Assert.True(AreEqual(actualTsar, expectedTsar));" - читается хуже, чем Fluent-style.
		 * 	  Так же хорошим тоном считается указание причины ожидаемого поведения.
		 * 4. Много кода - обычно, чем больше кода, тем больше вероятность сделать в нём ошибку.
		 *    Плюс опять же, много кода хуже читается.
		 * 5. Производительность - скорее всего этот рекурсивный велосипед много хуже оптимизирован, нежели специальный
		 * 	  метод из специальной библиотеки.
		 */

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