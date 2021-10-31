using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Equivalency;
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

			// Перепишите код на использование Fluent Assertions.
			// Работает)))
			actualTsar.Should().BeEquivalentTo(expectedTsar,
				config => config.AllowingInfiniteRecursion()
					.Excluding(p => 
						p.SelectedMemberInfo.Name == nameof(expectedTsar.Id) && p.SelectedMemberInfo.MemberType == typeof(int)));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// Если убрать поле у класса Person, то этот тест будет вылетать с ошибкой при попытке доступа к несуществующему полю
			// Если добавить новые поля, то придётся дописывать их в тесте
			// Если не дописать, то разные значения новых полей будут проходить тест, чего быть не должно
			// И да, если тест не выполняется, то мы видим 
			//		Excpected: True
			//		But was: False
			// Тут сложно понять, какое поле положило тест, да и поле ли это было
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