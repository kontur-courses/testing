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
			actualTsar.ShouldBeEquivalentTo(expectedTsar, 
				options => options.Excluding(si => si.Id)
					.Excluding(si => si.Parent.Id)
					.ExcludingMissingMembers());
			// Почему это решение лучше.
			// Если тест падёт, мы сможем получить детальную информацию, что пошло не так.
			// Читаемость выше, хотя это неточно.... Скорей для людей, которые хорошо знакомы с fluent Assertions,
			// Код будет более очевидным
			// Компктность, хотя это тоже спорный плюс, и не всегда хорошо.
			// Есть вариант с явным сравнением полей, actualTsar.Age.Should().Be(expect)
			// Но метод ShouldBeEquivalentTo проделывает тоже самое
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
			// Если тест упадёт, будет очень сложно обноружить проблемное место,
			// так как это сравнение не даст нам информацию в каком именно месте ломается программа
			// При добавление нового поля в Person, код придётся дорабатывать вносить правки
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