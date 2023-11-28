using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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

			//Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
			//Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
			//Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
			//Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);

			//Assert.AreEqual(expectedTsar.Parent!.Name, actualTsar.Parent!.Name);
			//Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
			//Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
			//Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);

			actualTsar.Should().Be(expectedTsar, new TsarEqualityComparer());
		}

		public class TsarEqualityComparer : IEqualityComparer<Person>
		{
			public bool Equals([AllowNull] Person x, [AllowNull] Person y)
			{
				if (ReferenceEquals(x, y))
					return true;

				if (x is null || y is null)
					return false;

				return x.Name == y.Name
					&& x.Age == y.Age
					&& x.Height == y.Height
					&& x.Weight == y.Weight
					&& Equals(x.Parent, y.Parent);
			}

			public int GetHashCode([DisallowNull] Person obj)
			{
				return HashCode.Combine(
					obj.Name,
					obj.Age,
					obj.Height,
					obj.Weight,
					obj.Parent is null ? 0 : GetHashCode(obj.Parent));
			}
		}

		/*
		Моё решение лучше тем, что из сообщения об ошибке можно понять, что пошло не так,
		а в тесте ниже сообщение неинформативное получается (с другой стороны у меня целое полотно разворачивается).
		При этом я согласен с тем, что проверку полей было бы неплохо вынести в отдельную конструкцию, чтобы в будущем 
		не нарушать DRY и вносить минимальное количество правок.
		Для этого можно реализовать IEqualityComparer<T> (мне пришлось обновить FluentAssertions).
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