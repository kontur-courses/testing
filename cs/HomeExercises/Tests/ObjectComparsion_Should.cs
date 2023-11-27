using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.Tests
{
	public class ObjectComparison_Should
	{
		private Person actualTsar;
		private Person expectedTsar;

		[SetUp]
		public void SetUp()
		{
			actualTsar = TsarRegistry.GetCurrentTsar();
			expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
		}
		
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			actualTsar.Should().BeEquivalentTo(expectedTsar, options => 
				options
					.IncludingFields()
					.Excluding(p => p.Id)
					.Excluding(p => p.Parent!.Id)
					.Excluding(p => p.Parent!.Weight)
					.Excluding(p => p.Parent!.Parent)
				);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			Assert.True(AreEqual(actualTsar, expectedTsar));

			// Какие недостатки у такого подхода? 
			// 1. При большой родословной будут проверяться все предки. И, при наличии расхождения у родителей, нам будет сложно понять в каком объекте произошло падение теста 
			// 2. Из названия теста непонятно, что именно значит CustomEquality и какое у него поведение
			// 3. Не показывается информация, на каком именно объекте тест упал, что усложняет разработку
			// 4. При расширении класса Person нужно будет дописывать AreEqual, а в FluentAssertions достаточно указать IncludingFields в конфигурации и все поля автоматически будут сравниваться
			// 5. Повышается читабельность и тесты проще писать
			
			// Улучшайзинги
			// 1. Также я вынес инициализацию объектов в метод SetUp, помеченный соответствующим атрибутом
			// 2. Для структурированности проекта вынес тесты в папку Tests
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