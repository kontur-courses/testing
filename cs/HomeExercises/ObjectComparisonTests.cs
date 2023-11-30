using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparisonTests
	{
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Код, который не нужно менять при добавлении новых свойств, при проверке исключаем поле Id
			actualTsar.Should().BeEquivalentTo(
				expectedTsar, config => config
					.Excluding(person => person.SelectedMemberInfo.Name.Equals("Id"))
			);

			/*
			 * Этот тест лучше CheckCurrentTsar_WithCustomEquality(), потому что код:
			 * - более читаемый
			 * - более современный и гибкий
			 * - легко расширяемый
			 * - не требует дополнительных структурных единиц (метод AreEqual)
			 */
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
			 * 1. Функция сравнения находится вне теста,
			 * это увеличивает вероятность ситуации,
			 * что при изменении класса Person мы забыли обновить AreEqual
			 *
			 * 2. Лишний структурный код - сигнатура метода, scope метода
			 *
			 * 3. При изменении Person нужно вручную обновлять метод
			 *
			 * 4. Рекурсия, возможно переполнение стека вызовов
			 */

			AreEqual(actualTsar, expectedTsar).Should().BeTrue();
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
}