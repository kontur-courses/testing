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
			AssertionOptions.AssertEquivalencyUsing(options =>
				options.Excluding(ctx => 
						ctx.SelectedMemberInfo.Name.Equals("Id"))
					.AllowingInfiniteRecursion());
			
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar);
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
			/* Самый главный недостаток - мы получим максимально неинформативное сообщение:
			 *
			 * Expected: True
			 * But was: False
			 *
			 * Из данного сообщения ничего непонятно: ни что сломалось,
			 * ни как именно сломалось, непонятно даже причем тут True и False,
			 * мы же должны два экземпляра класса Person сравнивать.
			 *
			 * Это можно избежать, если в методе AreEqual вместо возвращения
			 * результата сравнений всех требуемых полей сделать проверку на каждое поле.
			 * (Пример - метод AreEqualWithAsserts)
			 * В таком случае мы узнаем, в каком именно поле различия между экземплярами классов
			 *
			 * Второй недостаток - нет расширяемости кода. Если класс
			 * Person изменится и в нем появятся новые поля, придется вручную писать
			 * правило сравнения этих полей.
			 *
			 * Трейти недостаток - ухудшается читаемость кода. Он читается топорнее,
			 * чем при использовании Fluent, а так же содержит много повторов
			 * (сравнения в блоке return)
			 */
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