using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.tests
{
	public class ObjectComparison_Should
	{
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckValuesOfTheCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new ObjectComparison("Ivan IV The Terrible", 54, 170, 70,
				new ObjectComparison("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
				.Excluding(tsar =>
					tsar.SelectedMemberInfo.Name == nameof(ObjectComparison.Id)
					&& tsar.SelectedMemberInfo.DeclaringType == typeof(ObjectComparison)));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new ObjectComparison("Ivan IV The Terrible", 54, 170, 70,
				new ObjectComparison("Vasili III of Russia", 28, 170, 60, null));

			//Какие недостатки у такого подхода? 
			//Нужно будет переписывать AreEqual в случае изменения полей ObjectComparison
			//Возможна ошибка при написании метода
			//Если тест упадет, сообщение об ошибке будет малоинформативным
			Assert.True(AreEqual(actualTsar, expectedTsar));
		}

		private bool AreEqual(ObjectComparison? actual, ObjectComparison? expected)
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

	public class TsarRegistry
	{
		public static ObjectComparison GetCurrentTsar()
		{
			return new ObjectComparison(
				"Ivan IV The Terrible", 54, 170, 70,
				new ObjectComparison("Vasili III of Russia", 28, 170, 60, null));
		}
	}
}