using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		private static readonly HashSet<string> exclude = new HashSet<string>()
		{
			nameof(Person.Id)
		};

		[Test]
		[Description("Проверка текущего царя")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			AssertPerson(expectedTsar, actualTsar);
		}

		// В моём решении проверяются все поля, которые существуют на данный момент,
		// которые публичные и которых нет в исключениях.
		// Также при возникновении ошибки будет видно почему тест упал
		public void AssertPerson(Person? expected, Person? actual)
		{
			if (expected == null || actual == null)
			{
				expected.Should().Be(null);
				actual.Should().Be(null);
				return;
			}
			var fields = expected.GetType()
				.GetFields(BindingFlags.Instance | BindingFlags.Public)
				.Where(field => !exclude.Contains(field.Name));
			foreach (var field in fields)
			{
				var expectedValue = field.GetValue(expected);
				var actualValue = field.GetValue(actual);

				if (field.FieldType == typeof(Person))
					AssertPerson(expectedValue as Person, actualValue as Person);
				else 
					expectedValue.Should().Be(actualValue);
			}
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода?
			// 1. Новое поле/свойство - новое изменение теста
			//    Так как все поля класса перечисляются в AreEqual
			// 2. Assert только один, и он - проверка boolean
			//    Если в чём-то произойдёт ошибка, будет непонятно из-за чего
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