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

            actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
                options.Excluding(person => person.Id)
                    .Excluding(person => person.Parent.Id));
        }
        /* Преимущества подхода:
			1. Хорошая информативность. При непрохождении теста ясно показывается, какие поля не совпали.
			2. Хорошая расширяемость. При добавлении или удалении полей в классе, нужно внести минимум изменений в тесте.
			3. Хорошая читаемость. Из-за меньшего объема кода и понятного названия методов улучшается читаемость кода.
			*/

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            Assert.True(AreEqual(actualTsar, expectedTsar));
        }
        /* Недостаки подхода:
			1. Плохая информативность. При непрохождении теста выводится "Ожидалось True, но было False", из-за
			чего непонятно, какие поля не совпадают.
			2. Плохая расширяемость. При добавлении или удалении поля, придется изменять метод AreEqual, что можно
			забыть сделать.
			3. Функция сравнения не должна быть определена в классе тестов. Лучше, что бы она была определена в классе 
			Person или специальном классе для сравнения.
			*/

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