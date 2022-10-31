using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[SetUp]
		public void SetUp()
		{
			// не думаю, что есть известное дерево предков с глубиной 100
			AssertionOptions.FormattingOptions.MaxDepth = 100;
		}

		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
				options.Excluding(member => 
					member.DeclaringType == typeof(Person)  && member.Name == nameof(Person.Id)));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// Из очевидного, что при удалении/добавлении нового свойства(поля) из класса/в класс придется менять и код AreEqual


			// в случае ошибки, чтобы понять, в каком конкретном участке кода произошла ошибка, придется дебажить
			// (если смотреть на пример, то мы не сможем понять, у нас не совпадают имена или же возраст, без дебага)

			// AreEqual - не совсем говорящее название: как конкретно происходит сравнение на равенство из названия неясно
			// это уменьшает читабельность кода

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
		public int Age { get; set; }
		public int Height { get; set; }
		public int Weight { get; set; }
		public string Name { get; set; }
		public Person? Parent { get; set; }
		public int Id { get; set; }

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