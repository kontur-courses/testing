using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		public void TsarRegistryGetCurrentTsar_Fields_AreCorrect()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar, 
				option => option.
					AllowingInfiniteRecursion().
					IgnoringCyclicReferences().
					Excluding(member=> member.SelectedMemberInfo.Name == nameof(Person.Id)));
		}

		[Test]
		[Category(nameof(Person.IdCounter))]
		[Description("Проверка инкрементирования счетчика персон")]
		public void PersonField_IdCounter_IsIncrementable()
		{
			Person.IdCounter = 5;

			new Person("", 0, 0, 0, null);

			Person.IdCounter.Should().Be(6);
		}

		[Test]
		[Category(nameof(Person.IdCounter))]
		[Description("Проверка инкрементирования счетчика персон")]
		public void PersonField_IdCounter_AreIncrementedAfterNewPerson()
		{
			Person.IdCounter = 7;

			var person = new Person("", 0, 0, 0, null);

			person.Id.Should().Be(7);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			/* 1.Сложно поддерживать тест. Добавляем поле в Person => правим AreEqual (
			 * можно забыть это сделать, например, забыли включить Id или специально не
			 * включили, но тесты для Id и счетчика не написали)
			 * 2. По результатам теста невозможно понять, в чем именно ошибка
			 * 3. Нет защиты от зацикливания (хотя в логике рассматриваемого случая вроде она не нужна)
			 * 4. Метод AreEqual не тривиален, желательно написать тесты для его проверки
			 * 5. Невозможно быстро убедиться в корректности теста
			 * 6. Невозможно быстро понять, что тест проверяет, нужны дополнительные усилия
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
}