using System;
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
			//Arrange
			var expectedTsar = new Person(
				"Ivan IV The Terrible", 54, 170, 70, new DateTime(1530, 08, 25),
				new Person("Vasili III of Russia", 28, 170, 60, new DateTime(1479, 03, 25), null));

			//Act
			var actualTsar = TsarRegistry.GetCurrentTsar();

			//Assert
			actualTsar.Should().BeEquivalentTo(expectedTsar,
				options => options.Excluding(memberInfo =>
						memberInfo.SelectedMemberInfo.DeclaringType == typeof(Person)
						&& memberInfo.SelectedMemberInfo.Name == nameof(Person.Id)
						&& memberInfo.SelectedMemberInfo.MemberType == typeof(int))
					.Excluding(memberInfo =>
						memberInfo.SelectedMemberPath.StartsWith("Parent.Parent")));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person(
				"Ivan IV The Terrible", 54, 170, 70, new DateTime(1530, 08, 25),
				new Person("Vasili III of Russia", 28, 170, 60, new DateTime(1479, 03, 25),
					new Person("Ivan the Great", 65, 180, 75, new DateTime(1440, 01, 22), null)));

			// Какие недостатки у такого подхода?
			// В случае ошибки мы получим малоинформативное сообщение и не узнаем какие конкретно члены у полученного экземпляра и ожидаемого экземпляры различаются
			// Если добавим новое поле в класс Person, нужно будет отредактировать AreEqual
			// Для каждой сущности, на подобии класса Person потребуется собственный метод AreEqual 
			// Можно допустить ошибку при написании метода AreEqual
			// В реализации с FluentAssertion я настроил игнорирование поля Id, но в большинстве случаев дополнительная настройка не требуется 
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
				&& actual.DayOfBirth == expected.DayOfBirth
				&& AreEqual(actual.Parent, expected.Parent);
		}
	}

	public class TsarRegistry
	{
		public static Person GetCurrentTsar()
		{
			return new Person(
				"Ivan IV The Terrible", 54, 170, 70, new DateTime(1530, 08, 25),
				new Person("Vasili III of Russia", 28, 170, 60, new DateTime(1479, 03, 25), null));
		}
	}

	public class Person
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person? Parent;
		public int Id;
		public DateTime DayOfBirth;

		public Person(string name, int age, int height, int weight, DateTime dayOfBirth, Person? parent)
		{
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			DayOfBirth = dayOfBirth;
			Parent = parent;
		}
	}
}