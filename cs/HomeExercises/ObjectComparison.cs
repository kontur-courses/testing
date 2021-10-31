using System;
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
			
			// Пояснение: Можно было бы обойтись и ctx.SelectedMemberPath.EndsWith("Id")
			// 			  Но при добавлении в класс Person свойств или полей заканчивающихся на "Id" (MemberId ...),
			//			  тест будет их игнорировать - нежелательное поведение
			expectedTsar.Should().BeEquivalentTo(actualTsar, 
				options => options.Excluding(person => person.Id)
										 .Excluding(ctx => ctx.SelectedMemberPath.EndsWith("Parent.Id")));
			
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			
			// 1. Сложно расширяемое и контролируемое решение:
			// 		Добавление/удаление свойств и полей в классе Person либо приведет к ошибке компиляции,
			// 		либо приведет к логической ошибке.
			// 2. Циклические ссылки:
			//		В случае возникновения циклической ссылки типа actualTsar.Parent = actualTsar,
			//		тест уйдет в бесконечный цикл.
			
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