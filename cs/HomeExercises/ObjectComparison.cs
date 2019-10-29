using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class Test_TsarRegistry
	{
		private Person actualKing;
		private Person expectedKing;

		[SetUp]
		public void SetUp()
		{
			// Так как в обоих тестах одни и те же данные
			// Можно их инициализировать в Сетапе
			actualKing = TsarRegistry.GetCurrentTsar();
			expectedKing = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
        }

		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentKing()
		{
			// Использовать Fluent API лучше тем, что можно сразу определить
			// где ожидаемое значение, а где полученное.
			actualKing.Should().NotBeNull();
			actualKing.Name.Should().Be(expectedKing.Name);
			actualKing.Age.Should().Be(expectedKing.Age);
			actualKing.Height.Should().Be(expectedKing.Height);
			actualKing.Weight.Should().Be(expectedKing.Weight);

			actualKing.Parent.Should().NotBeNull();
			actualKing.Parent.Name.Should().Be(expectedKing.Parent.Name);
			actualKing.Parent.Age.Should().Be(expectedKing.Parent.Age);
			actualKing.Parent.Height.Should().Be(expectedKing.Parent.Height);
			actualKing.Parent.Weight.Should().Be(expectedKing.Parent.Weight);
        }

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			// Какие недостатки у такого подхода?
			// Если тест не пройдет, то будет непонятно, где именно
			// Assert.True(AreEqual(actualKing, expectedKing));

			// В данном решении проверяется не только отец текущего человека,
			// но и все отцы отцов
			// Также, если тест упадет то можно увидеть, на чем ошибка 
			Test_PersonsAreEqual(expectedKing, actualKing, "current King");
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


		public static void Test_PersonsAreEqual(Person expected, Person actual, string because)
		{
			actual.Name.Should().Be(expected.Name, $"because names of {because} should be equal");
			actual.Age.Should().Be(expected.Age, $"because ages of {because} should be equal");
			actual.Height.Should().Be(expected.Height, $"because heights of {because} should be equal");
			actual.Weight.Should().Be(expected.Weight, $"because weights of {because} should be equal");

			if (expected.Parent != null)
			{
				actual.Parent.Should().NotBeNull($"because parent of {because} shouldn't be null");
				Test_PersonsAreEqual(expected.Parent, actual.Parent, "parent of " + because);
			}
			else
			{
				actual.Parent.Should().BeNull($"because parent of {because} should be null");
			}
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