using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Types;
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

			actualTsar.ShouldBeEquivalentTo(expectedTsar,
				assertionOptions =>
					assertionOptions.Excluding(subjectInfo =>
						subjectInfo.SelectedMemberInfo.DeclaringType.Name == nameof(Person) &&
						subjectInfo.SelectedMemberInfo.Name == nameof(Person.Id)));
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
			 * 1) Необходимость писать собственную реализацию проверки на равенство объектов класса Person
			 *    (в данном случае AreEqual)
			 *
			 * 2) Тест не расширяем: в случае изменения класса Person (удаления/добавления свойств)
			 * 		придется вносить изменения в метод AreEqual
			 *
			 * 3) Assert менее читаем, чем Should. Из названия метода AreEqual не ясно,
			 * 		что он предназначен только для сравнения объектов класса Person.
			 *
			 * 4) Неинформативный вывод. Если тест упадет, то мы не сможем узнать,
			 * 		где произошла ошибка (в отличие от моего решения).
			 * 		Выведется лишь, что метод AreEqual вернул false вместо ожидаемого true.
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