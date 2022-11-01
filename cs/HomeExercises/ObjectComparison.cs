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


			actualTsar.Should().BeEquivalentTo(
				expectedTsar,
				options => options.Excluding(o => o.Path.EndsWith("Id"))
				);

		}

		
		/*
		 * Тест, приведенный ниже, хуже моего, потому что:
		 *  - Больше объем кода и труднее читать
		 *  - В случае изменения в классе необходимо будет изменять код в тесте
		 */
		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			Assert.True(actualTsar.Equal(expectedTsar));
		}
	}
}