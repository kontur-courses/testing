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

			var expectedTsar = new Person(
				"Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null)
			);

			actualTsar.Should().BeEquivalentTo(
				expectedTsar,
				o => o
					.AllowingInfiniteRecursion()
					.Excluding(m => m.Name == nameof(Person.Id) && m.DeclaringType == typeof(Person))
			);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person(
				"Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null)
			);

			Assert.True(AreEqual(actualTsar, expectedTsar));

			// Недостатками такого подхода является то что при удалении какого-либо из полей, тест перестанет работать,
			// и придется в ручную править его. При добавлении нового поля все получится еще хуже, так как если забыть
			// добавить эти поля в проверку, то ошибок компиляции и рантайма не будет, однако возникнут логические
			// ошибки и тест начнёт работать некорректно, опуская проверку новых полей.
			// 
			// Дополнительная проблема в том, что происходит один единственный Assert, и если тест упадёт, нам
			// не сообщат какое именно из полей не совпало с ожиданиями. И чтобы найти его, придется
			// в ручную дебагером пройтись по тесту и определить несоответствия,
			// тем самым полностью убивая смысл автоматического тестирования)
		}

		private static bool AreEqual(Person? actual, Person? expected)
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

	public static class TsarRegistry
	{
		public static Person GetCurrentTsar() =>
			new(
				"Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null)
			);
	}

	public class Person
	{
		private static int _idCounter;
		public readonly int Age, Height, Weight;
		public readonly string Name;
		public readonly Person? Parent;
		public readonly int Id;

		public Person(string name, int age, int height, int weight, Person? parent)
		{
			Id = _idCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}