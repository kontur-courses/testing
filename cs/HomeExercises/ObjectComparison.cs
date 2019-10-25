using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar_ShouldBeEqual()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Перепишите код на использование Fluent Assertions.
			CheckPersonsByFields(expectedTsar, actualTsar);
		}

		private void CheckPersonsByFields(Person expectedPerson, Person actualPerson)
		{
			if(expectedPerson == null || actualPerson == null) return;
			actualPerson.Age.Should().Be(expectedPerson.Age);
			actualPerson.Name.Should().Be(expectedPerson.Name);
			actualPerson.Height.Should().Be(expectedPerson.Height);
			actualPerson.Weight.Should().Be(expectedPerson.Weight);
			CheckPersonsByFields(expectedPerson.Parent, actualPerson.Parent);
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
			
			/* Если упадет данный тест, то разработчик сможет увидеть только лишь то, что
			 эти два объекта просто неравны.
			 В то время как в моем решении будет видно в каком свойстве разница и какие были значения 
			 этих свойств на момент выполнения теста.
			 Сам тест стал читабельнее*/
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