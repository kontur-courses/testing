using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Common;
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
			//Ты прав. Здесь я забыл, что изначально код повторялся 2 раза(для person и для person.Parent)
			actualTsar.ShouldBeEquivalentTo(expectedTsar, options => options
				.Excluding(person => person.Id)
				.Excluding(person => person.Parent.Id));
			//Но в таком случае, если person.Parent.Parent будет != null, то родитель родителя не будет проверяться
			
//			CheckPersonsByFields(actualTsar, expectedTsar);
			//Так будет исправленно. Но тогда, если несовпадение произошло в свойстве person.Parent,
			//в сообщении будет сказано только название свойства, но не то, что оно в родителе
			//Что важнее ?
		}

		private static void CheckPersonsByFields(Person actualPerson, Person expectedPerson)
		{
			if(actualPerson == null || expectedPerson == null) return;
			actualPerson.ShouldBeEquivalentTo(expectedPerson, options => options
				.Excluding(person => person.Id)
				.Excluding(person => person.Parent));
			CheckPersonsByFields(actualPerson.Parent, expectedPerson.Parent);
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
			 Изменения свойств в классе не приведут к большим изменениям в тестах.
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