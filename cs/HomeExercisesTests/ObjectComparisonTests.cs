using NUnit.Framework;
using FluentAssertions;
using HomeExercises;

namespace HomeExercisesTests
{
	[TestFixture]
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
			
			actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
				options.Excluding(member => 
					member.SelectedMemberInfo.DeclaringType == typeof(Person)
					&& member.SelectedMemberInfo.Name == nameof(Person.Id)));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			/* Какие недостатки у такого подхода? 
			 * 1.	Придется дополнительно изучить метод AreEqual.
			 * 2.	Код в AreEqual сложнее читать, чем если бы мы сделали через FluentAssertions.
			 * 3.	Создаем дополнительный мусорный метод, который потом будет мешаться в подсказках от IDE.
			 * 4.	Код получится длиннее, чем если бы мы использовали FluentAssertions.
			 * 5.	При добавлении новых полей в класс Person придется добавлять новые условия в метод AreEqual.
			 * 6.	Не очевидно что специально не сравнивают, а что забыли написать.
			 * 7.	Не будет показано в каком именно поле пошло что-то не так, что от него ожидалось, а что получено.
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
}