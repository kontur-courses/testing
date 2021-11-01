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
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Перепишите код на использование Fluent Assertions.

			actualTsar.Should().BeEquivalentTo(expectedTsar, 
				options => options.Excluding(
					o => o.SelectedMemberInfo.Name == nameof(actualTsar.Id) 
					&& o.SelectedMemberInfo.DeclaringType.Name == nameof(Person)));
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
				При увеличении количества полей, нам придётся каждый раз переписывать этот метод,
				чтобы учесть все новые параметры.
				Так же скрывается то по каким параметрам мы будем проводить сравнение,
				вдруг окажется что для равенства достаточно совпадения имени.
				Такой тест ещё и не информативен в плане ошибки, то есть если тест падает,
				скрывается информация почему он упал, вкупе с предыдущим пунктом, это создаст большую путаницу.
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