using NUnit.Framework;
using FluentAssertions;
using HomeExercises.Entities;

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

			actualTsar.Should()
				.BeEquivalentTo(expectedTsar, opt => 
					opt.Excluding(p => p.SelectedMemberInfo.Name == nameof(Person.Id) &&
					                   p.SelectedMemberInfo.DeclaringType == typeof(Person)));
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
			1) При падении теста мы не узнаем из за каких полей упал тест, потому что метод AreEqual возвращает bool.
			2) При добавлении/удалении свойств из Person, придется дописывать/удалять необходимые сравнения в тесте.
				 => в некоторых кейсах подобный подход будет следстием разрастания кода в тестах
			3) При наличии замкнутых ссылок в свойстве Parent, можем получить StackOverflow
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