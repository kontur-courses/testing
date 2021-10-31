using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.ObjectComparisonTask
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

			actualTsar.Should().BeEquivalentTo(expectedTsar, config =>
				config
					.Excluding(info =>
						info.SelectedMemberInfo.DeclaringType.Name == nameof(Person)
						&& info.SelectedMemberInfo.Name == nameof(actualTsar.Id))
					.AllowingInfiniteRecursion()
			);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			/* ----|Про данный тест|----
             * Этот подход плох тем, что он плохо масштабируем. Если в классе Person добавится или
             * удалится какое то поле (или свойство), то в данном тесте их также вручную придется
             * добавлять и удалять.
             *
             * Данный подход плох еще и тем, что при передаче в него аргументов <actual> и <expected>
             * можно перепутать порядок. И пусть в данном случае этот факт не влияет на корректность
             * работы теста, но по прежнему остается возможность для разночтения. Если писать новые тесты
             * по аналогии с этим, то вполне реальна ситуация, когда в одном аргументы передаются
             * в порядке (<actual>, <expected>), а в другом (<expected>, <actual>)
             *
			 * Еще одна вещь, которая делает этот тест хуже - это отсутствие информации, если тест провалится.
			 * Если объекты не будут равны, то в отчете получим что-то вроде
			 * Expected: True
			 * But was: False
			 * И никаких деталей о том, где конкретно было несоответствие мы не получим.
			 *
             * ----|Сравнение с моим решением|----
             * 1) В моем решении присутствует возможность изменения порядка <actual> и <expected>,
             *		но в меньшей степени за счет меньшего количества обращений к сравниваемым объектам
             *		из написанного кода.
             *
             * 2) Тест CheckCurrentTsar_WithCustomEquality создает впечатление множества проверок
			 *		из-за многострочного логического выражения. Другими словами, если в коде теста
			 *		CheckCurrentTsar_WithCustomEquality разработчиком выражается мысль "Какие поля
			 *		мы сравниваем", то в моем решении выражается мысль "какие поля мы НЕ сравниваем".
			 *		В данном контексте я считаю, что гораздо лаконичнее будет сказать о том, что не
			 *		должно сравниваться.
             *
			 * 3) Мое решение более читаемо за счет именований из библиотеки FluentAssertions и за
			 *		счет пункта 2) данного комментария.
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