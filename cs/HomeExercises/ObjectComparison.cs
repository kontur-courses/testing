using FluentAssertions;
using NUnit.Framework;
using System.Reflection;

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

            //expectedTsar.Name.Should().Be(actualTsar.Name);
            //expectedTsar.Age.Should().Be(actualTsar.Age);
            //expectedTsar.Height.Should().Be(actualTsar.Height);
            //expectedTsar.Weight.Should().Be(actualTsar.Weight);

            //actualTsar.Parent.Name.Should().Be(expectedTsar.Parent.Name);
            //actualTsar.Parent.Age.Should().Be(expectedTsar.Parent.Age);
            //actualTsar.Parent.Height.Should().Be(expectedTsar.Parent.Height);
            //actualTsar.Parent.Parent.Should().Be(expectedTsar.Parent.Parent);

            /* в первом варианте мы получаем больше данных после тестирования, по stacktrace можно увидеть
             какой тест упал, тогда как во втором варианте мы просто увидим что объекты не равны*/
            actualTsar.ShouldBeEquivalentTo(expectedTsar
                , o => o.Excluding(s => s.Id));//проблема: у parent ID учитывается
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
		}
        [Test]
        [Description("Альтернативное решение enchanced")]
        public void CheckCurrentTsar_WithCustomEqualityEnchanced()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));
            AreEqualEnchanced(actualTsar, expectedTsar);
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

        private void AreEqualEnchanced(Person actual, Person expected)
        {
            if (actual == expected) return;
            actual.Should().NotBeNull();
            expected.Should().NotBeNull();
            expected.Name.Should().Be(actual.Name);
            expected.Age.Should().Be(actual.Age);
            expected.Height.Should().Be(actual.Height);
            expected.Weight.Should().Be(actual.Weight);
            AreEqualEnchanced(actual.Parent, expected.Parent);

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