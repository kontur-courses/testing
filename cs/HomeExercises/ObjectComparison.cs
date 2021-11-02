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
			
			// Перепишите код на использование Fluent Assertions
			expectedTsar.Should().BeEquivalentTo(actualTsar,
				options => options.Excluding(ctx => ctx.SelectedMemberInfo.Name.Equals(nameof(Person.Id))
				                                    && ctx.SelectedMemberInfo.DeclaringType == typeof(Person)));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 

			// 1. Сложно расширяемое и контролируемое решение:
			// 		Добавление/удаление свойств и полей в классе Person либо приведет к ошибке компиляции,
			// 		либо приведет к логической ошибке.
			// 2. Циклические ссылки:
			//		В случае возникновения циклической ссылки типа actualTsar.Parent = actualTsar,
			//		тест уйдет в бесконечный цикл.

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