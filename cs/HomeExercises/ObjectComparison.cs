using FluentAssertions;
using NUnit.Framework;
using System.Linq;

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
			PersonEquals(actualTsar, expectedTsar);
			PersonEquals(actualTsar.Parent, expectedTsar.Parent);
			// Почему лучше? 
			// Первое - граммотное сообщение об ошибке
			// Второе - если появится новое поле, то его можно легко добавить в проверку в PersonEquals
			// actualTsar.NewField.Should().Be(expectedTsar.NewField, "becouse");
			// если же появилось поле типа Person (например второй родитель), 
			// то в CheckCurrentTsar легко добавить проверку PersonEquals(actualPerson.NewParent, expectedPerson.NewParent)
			// тем самым обходя рекурсивное рассмотрение (если это нужно), 
			// если нужно рекурсивно смотреть то проверку можно осуществить в PersonEquals,
			// однако, если у нас есть поле типа "брат"/"сестра" то рекурсивный подход зациклится и заполнит стек
			// поэтому есть смысл дать проверяющему самостоятельно разобраться как именно и до какого момента он желает проверять Person.
		}

		private void PersonEquals(Person actualPerson, Person expectedPerson, bool isRecurs = false)
		{
			if(ReferenceEquals(actualPerson, expectedPerson))
				return;
			if(actualPerson == null && expectedPerson == null)
				return;
			new[] { actualPerson, expectedPerson }.Should().NotContain(p => p == null);
			actualPerson.Name.Should().Be(expectedPerson.Name, "actualName = expectedName");
			actualPerson.Age.Should().Be(expectedPerson.Age, "actualAge = expectedAge");
			actualPerson.Height.Should().Be(expectedPerson.Height, "actualHeight = expectedHeight");
			actualPerson.Weight.Should().Be(expectedPerson.Weight, "actualWeight = expectedWeight");
			if (isRecurs)
				PersonEquals(actualPerson.Parent, expectedPerson.Parent, true);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// Если проверка не прошла, то выведет сообщение вида "ожидалась истина, а была ложь"
			// Что затрудняет понимание теста и сравнения объектов
			// Кроме того метод рекурсивно проверяет все гениалогическое древо 
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