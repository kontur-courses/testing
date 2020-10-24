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

			// Перепишите код на использование Fluent Assertions.
			actualTsar.Should().BeEquivalentTo(
				expectedTsar, 
				options => options
					.AllowingInfiniteRecursion()
					.Excluding(ctx => ctx.SelectedMemberPath.EndsWith(nameof(Person.Id))));
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
			1) Если тест упадет, мы не узнаем, какие именно значения не совпадают,
			 то есть нам придется сверять поля вручную.
			2) Решение с FluentAssertions куда лучше читается и легче понимается.
			3) Добавление новых свойств в класс Person может привести к необходимости 
			писать много кода, если эти свойства не сравниваются простым "==", а 
			требуют каких-то кастомных сравнивателей. Например, массивы, которые нам 
			зачем-то понадобилось сравнивать без учета порядка - с Fluent Assertions 
			сможем написать понятно в одну строчку, и не придется изобретать велосипед. 

			Upd. Теперь в CheckCurrentTsar() поля не сравниваются вручную.
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