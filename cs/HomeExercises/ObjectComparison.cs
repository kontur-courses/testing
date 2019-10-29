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

            actualTsar.Should().BeEquivalentTo(expectedTsar,
                option => option
                .Excluding((FluentAssertions.Equivalency.IMemberInfo o) =>
                o.SelectedMemberInfo.DeclaringType == typeof(Person) &&
                o.SelectedMemberInfo.Name == nameof(Person.Id)));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода?
            // Так как метод AreEqual возращает значение типа bool,
            // то в случае неравенства объектов, тест сообщит о том что ожидал true, а получил false,
            // при этом не указав какие именно поля не совпадали у объектов.
            // Из-за этого может понадовиться вручную дебажить тест чтобы узнать положение несовпадающего поля.
            // К тому же нельзя узнать было ли несовпадающее поле только одно или их было несколько,
            // что затруднит исправление, так как ты не узнаешь правильно ли исправил одно значения, пока не исправишь остальные.

            // При расширении класса и добывлении в него новых полей придётся дописывать в условие новые сравнения,
            // поэтому если сравнение объектов будет зависить от этой новой переменной (например год рождения),
            // то при равенстве старых полей тест будет сообщать о равенстве объектов, в то время как года рождения могут отличаться.
            // При этих же условиях моя реализация тесты перестанет проходить, чем сообщит о том что тест устарел и его стоит переписать.

            Assert.True(AreEqual(actualTsar, expectedTsar));
		}

		private bool AreEqual(Person actual, Person expected)
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
		public Person Parent;
		public int Id;

		public Person(string name, int age, int height, int weight, Person parent)
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