using FluentAssertions;
using NUnit.Framework;
using System.Text.RegularExpressions;

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

			actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
				.Excluding(p => p.Id)
				.Excluding(p => p.Parent!.Id)
				.Excluding(p => p.Parent!.Weight) // в оригинале нет теста для weight
				.Excluding(p => p.Parent!.Parent));
			//Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent); - сравнивает ссылки
			actualTsar.Parent!.Parent.Should().BeSameAs(expectedTsar.Parent!.Parent);
		}

		[Test]
		[Description("Вариация альтернативного решения")]
		[Category("ToRefactorAlternative")]
		public void CheckCurrentTsarAlternative()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
				.Excluding(p => Regex.IsMatch(p.SelectedMemberPath, @"(^|(\.|^)Parent\.)Id$")));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			Assert.True(AreEqual(actualTsar, expectedTsar));
			// 1 Не показательно - судя по названию методов тестирования, интересует не результат сравнения:
			// равны объекты или нет, а проверить соответствует ли объект заданным параметрам и => всего 1 assert
			// не будет показателен, в плане где именно произошло несоответствие
			// 2 может уйти в цикл parent1.parent2.parent1...
			// 3 проверка сильно привязана к структуре объекта
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