﻿using FluentAssertions;
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


			actualTsar.Should().BeEquivalentTo(
				expectedTsar,
				options => options.Excluding(o => o.Path.EndsWith("Id"))
				);

			// Перепишите код на использование Fluent Assertions.
			// Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
			// Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
			// Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
			// Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);
			//
			// Assert.AreEqual(expectedTsar.Parent!.Name, actualTsar.Parent!.Name);
			// Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
			// Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
			// Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);
		}

		
		/*
		 * Тест, приведенный ниже, хуже моего, потому что:
		 *  - Больше объем кода и труднее читать
		 *  - В случае изменения в классе необходимо будет изменять код в тесте
		 */
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