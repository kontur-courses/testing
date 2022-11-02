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

			actualTsar
				.Should()
				.BeEquivalentTo(expectedTsar, options => 
					options
						.Excluding(obj => obj.Id)
						.Excluding(obj => obj.Parent)
				);

			expectedTsar
				.Parent
				.Should()
				.BeEquivalentTo(actualTsar.Parent, options => 
					options.Excluding(obj => obj.Id)
				);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// 1. При добавлении новых полей мы должны переписать метод AreEqual, добавив в него новые проверки;
			// 2. Если названия поля и нескольких полей класса Person изменится, то нам снова нужно будет менять условие в методе AreEqual, что неудобно;
			// 3. Если в классе Person много полей, то заниматься дебагом или изменением метода AreEqual будет неудобно из-за сложной чиатемости кода и пр.;
			// 4. Если мы хотим сравнить двух царей по конкретным полям, исключив некоторые, то нам будет нужно переписать метод AreEqual снова и 
			// делать так каждый раз, что снова крайне неудобно
			// 5. Если в классе Person будет поле ссылочного типа(например другого класса), то нам нужно дописывать логику сравнения для этого поля отдельно
			// 6. Если в классе Person тип поля со значимого изменится на ссылочный, то нужно будет переписывать логику сравнения для этого поля в методе AreEqual
			
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
