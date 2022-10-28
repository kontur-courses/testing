using System;
using System.Net.Sockets;
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
			
			// Данная проверка позволяет сравнивать двух людей по всем полям,
			// не учитывая Id самих людей и всех их родителей
			actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
				options.Excluding(member => member.Path.EndsWith("Id")));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			/*
			 * 1) При такой проверке тест будет падать при несовпадении хоть одного поля, хоть нескольких.
			 *    При этом не будет никакой информации о том, какое именно из полей не совпало.
			 *    Это очень сильно затрудняет процесс тестирования и заставляет вручную проходиться отладкой по тесту,
			 *    и сравнивать значения свойст. Таким образом тест можно частично отнести к антипаттерну Loudmouth. 
			 */
			
			// Плюсы у такого подходы, которые можно перенять
			/*
			 * 1) Метод AreEqual, благодаря рекурсивным вызовам, позволяет без дублирования кода
			 *    проверить всех родителей царя на совпадение
			 * 2) Легко расширять, за счёт отсутствия дублирования кода для царя и родителей
			 * 3) Сравнение универсально для любых объектов типа Person, то есть можно использовать
			 *    в других тестах при сравнении людей.
			 */
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