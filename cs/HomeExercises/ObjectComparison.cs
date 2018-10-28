using System;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace HomeExercises
{
	public class ObjectComparison
	{
		private void AreEqualByFields(Person actual, Person expected)
		{
			foreach (var field in actual.GetType().GetFields())
			{
				if (field.Name == "Id" || field.Name == "Parent" ||
				    field.Name == "IdCounter")
					continue;
				var actualCurrentPropertyValue =
					actual.GetType().GetField(field.Name).GetValue(actual);
				var expectedCurrentPropertyValue =
					expected.GetType().GetField(field.Name).GetValue(expected);

				actualCurrentPropertyValue.Should()
					.Be(expectedCurrentPropertyValue,
						String.Format("Fields {0} should be equal", field.Name));
			}
        }

		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{	
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			AreEqualByFields(actualTsar, expectedTsar);
			AreEqualByFields(actualTsar.Parent, expectedTsar.Parent);

			// Перепишите код на ипользование Fluent Assertions.
			/*Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
			Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
			Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
			Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);

			Assert.AreEqual(expectedTsar.Parent.Name, actualTsar.Parent.Name);
			Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
			Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
			Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);*/
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			/*В этом случае будет не ясно какие именно поля не равны.
			 Оба теста проверяют слишком много информации, что очень неудобно
			 для понимания того, что именно пошло не так. Так же при подобной реализации
			 будет необходимо переписывать метод AreEqual при добавлении нового поля 
			 в класс Person. В моем решении метод сравнения сообщит об ошибке 
			 в тот момент, когда она произойдет. Есть пояснение какие именно поля не 
			 совпадают. При изменении класса Person нет необходимости менять что-либо,
			 так как поля взяты с помощью рефлексии.*/
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