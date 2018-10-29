using FluentAssertions;
using FluentAssertions.Primitives;
using NUnit.Framework;

namespace HomeExercises
{
	public static class PersonExtension
	{
		public static void ShouldBeEquivalentTo(this Person expected, Person actual)
		{
			expected.Should().BeEquivalentTo(actual,
				options => options.Excluding(x =>
					x.SelectedMemberInfo.DeclaringType == typeof(Person) && x.SelectedMemberInfo.Name == nameof(Person.Id)));
        }
    }

    public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, new Person("Basili III of Russia", 28, 170, 60, null)));

			// Перепишите код на использование Fluent Assertions.
			expectedTsar.ShouldBeEquivalentTo(actualTsar);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, new Person("Basili III of Russia", 28, 170, 60, null)));

			// Какие недостатки у такого подхода? 
			/* В этом тесте используеться своя реализация AreEqual, поэтому каждый раз
			 когда будет меняться\добавляться какие-либо поля у класса его нужно будет переделывать
			 
			 Еще одна проблема будет, если тест упадет не будет понятна в чем проблема будет просто ожидалось тру, а найденно фалс*/
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
				new Person("Vasili III of Russia", 28, 170, 60, new Person("Basili III of Russia", 28, 170, 60, null)));
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