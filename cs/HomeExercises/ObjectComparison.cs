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
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

            // Перепишите код на использование Fluent Assertions.
            actualTsar.ShouldBeEquivalentTo(expectedTsar, config => config.Excluding(x => x.SelectedMemberInfo.Name == "Id"));
            /// Решение с использованием Fluent Assertions более краткое и более "человекочитаемое",
            /// в нем легче разобраться, меньше вероятность того, что делая тесты на равенство всех
            /// полей объектов сделаешь ошибку из-за копипаста, или какое-то поле пропустишь.
            /// И оно лучше расширяется: при появлении в классе Person новых полей - в тесте ничего 
            /// менять не надо. Кроме указания полей, которые не нужно сравнивать. Но это легче, т.к.
            /// они "всплывут" при падении теста.
		}

        [Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			Assert.True(AreEqual(actualTsar, expectedTsar));
            /// Более громоздкое (за счет проверки всех полей в методе AreEqual),
            /// хуже при расширении, т.е. при добавлении или изменении полей - 
            /// надо будет и AreEqual править.
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