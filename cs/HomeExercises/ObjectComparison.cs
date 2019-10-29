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

		    actualTsar.ShouldBeEquivalentTo(expectedTsar,
			options => options.Excluding(person => person.SelectedMemberPath.EndsWith("Id")));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
		    var actualTsar = TsarRegistry.GetCurrentTsar();
		    var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			new Person("Vasili III of Russia", 28, 170, 60, null));

		    // Какие недостатки у такого подхода?
		    // При изменении класса Person нужно будет изменить также и метод AreEqual.
		    // При использовании альтернативного решения в случае ошибки не будет говориться, 
		    //в каком месте она произошла,
		    //в отличие от FluentAssertions, который даст подробную информацию.
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
