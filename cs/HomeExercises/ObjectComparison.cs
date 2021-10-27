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

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar,
				options => options.Excluding(x => x.SelectedMemberPath.EndsWith("Id")));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		/*
		 * В решении ниже используется дополнительный метод AreEqual, в котром явно указаны поля для сравнения.
		 * Если в класс Person будет добавлено новое поле, то придется дописывать новые поля в метод AreEqual.
		 * В это же время тест CheckCurrentTsar() не потребует изменений, чтобы при сравнении учитывались новые поля.
		 * Так же тест CheckCurrentTsar_WithCustomEquality() при падении не указывает, какие поля отличались от ожидаемых.
		 * Указывается лишь то, что ожидалось от AreEqual True, но вернулось False.
		 */
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
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
}