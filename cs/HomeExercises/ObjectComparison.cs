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
            Test_PersonsAreEqual(expectedTsar, actualTsar);

			// Используя такую проверку можно увидеть на каком именно поле провалился тест.
			// Также, если в классе появится новое поле, то достаточно добавить одну строку кода
			// в метод Test_PersonsAreEqual. И в нем по-прежнему остается рекурсивная 
			// Также данный метод лучше тем что использует нерекурсивную проверку
			// из-за которой может произойти переполнение стэка
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

			// Если тест упадет, то будет непонятно, в каком именно поле будет ошибка
			// Так как метод возвращает лишь ответ верны объекты или неверны
			// Также если будет слишком глубокая рекурсия родителей, то в теории может
			// произойти переполнение стэка
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

		public static void Test_PersonsAreEqual(Person expected, Person actual)
		{
			string because = "Person";
			while (expected != null)
			{
				actual.Should().NotBeNull();
				actual.Name.Should().Be(expected.Name, $"because names of {because} should be equal");
				actual.Age.Should().Be(expected.Age, $"because ages of {because} should be equal");
				actual.Height.Should().Be(expected.Height, $"because heights of {because} should be equal");
				actual.Weight.Should().Be(expected.Weight, $"because weights of {because} should be equal");
				expected = expected.Parent;
				actual = actual.Parent;
				because = "Parent of " + because;
			}

			actual.Should().BeNull($"because parent of {because} should be null");
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