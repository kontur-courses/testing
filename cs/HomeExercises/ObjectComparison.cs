using System;
using System.Linq;
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
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			var actualTsar = TsarRegistry.GetCurrentTsar();

			actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
            options.Excluding(tsar => tsar.SelectedMemberInfo.DeclaringType == typeof(Person)
            && tsar.SelectedMemberInfo.Name == nameof(Person.Id)));

            // Плюсы такого подхода:
            // 1) можно добавлять поля c минимальными изменениями в методе BeEquivalentTo()
            // 2) уменьшается количество строк кода и, соответственно, увеличивается читаемость
            // 3) методы FluenAssertion делают код намного гибче - здесь, например,
            // мы рекурсивно исключили поле Id из сравнения.
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода?

            // 1) Можно сделать, например, так:
			// actualTsar.Parent.Parent = actualTsar;
			// expectedTsar.Parent.Parent = expectedTsar;
			// и тест уходит в бесконечную рекурсию.
			// А моё решение быстро замечает, что есть зацикленность и падает
			// с соответствующим сообщением.
			// 2) При добавлении полей в класс Person
			// тест придется каждый раз дописывать.
            // 3) При падении непонятно, в каком поле ошибка
            // 4) При изменении условий совпадения нужно искать
            // вручную каждую лишнюю строчку.
            // 5) Критериев для сравнений вообще может быть много разных -
            // вдруг мы захотим выявить царей, правивших приблизительно в одну эпоху
            // или правивших примерно одно и то же количество лет. В таком случае
			// каждый раз придется писать подходящий AreEqual().
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