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
			actualTsar.Should().BeEquivalentTo(expectedTsar, 
				options => options.Excluding(o => o.SelectedMemberInfo.Name == "Id"));
			/* Это решение лучше чем нижнее тем, 
			 * что Fluent Assertions позволяет использовать гибкий инструмент
			 * BeEquivalentTo, который позволяет сравнивать два объекта.
			 * При задании опций можно настроить то, по каким полям следует сравнивать
			 * эти объекты. То есть это решает проблему неизвестности, и мы явно обозначаем
			 * то, что хотим сравнить. В данном случае мы исключаем у всех экземпляров сравнения
			 * поле Id, в том числе и у поля Parent.
			 * Так же, если будут добавлены/удалены какие-то поля/свойства,
			 * нам не придётся заново переписывать тест. Только в случае если мы хотим
			 * исключить что-то из сравнвения.
			 * Ну и FA даёт нам понять где и в каком месте тест упал(если упал),
			 * что решает третью проблему теста ниже.
			*/
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

	public class TsarRegistry
	{
		public static Person GetCurrentTsar()
		{
			return new Person(
				"Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
		}
	}

	public class Person
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person? Parent;
		public int Id;

		public Person(string name, int age, int height, int weight, Person? parent)
		{
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}