using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
//using NUnit.Framework.Internal;
using System.Reflection;
using FluentAssertions.Common;
using NUnit.Framework.Internal;

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
			Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
			Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
			Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
			Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);

			Assert.AreEqual(expectedTsar.Parent.Name, actualTsar.Parent.Name);
			Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
			Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
			Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);
		}
		[Test]
		public void CheckCurrentTsar_Fluent()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));


			//actualTsar.Age.Should().Be(expectedTsar.Age);
			//actualTsar.Height.Should().Be(expectedTsar.Height);
			//actualTsar.Name.Should().Be(expectedTsar.Name);
			//actualTsar.Weight.Should().Be(expectedTsar.Weight);

			//actualTsar.Parent.Age.Should().Be(expectedTsar.Parent.Age);
			//actualTsar.Parent.Height.Should().Be(expectedTsar.Parent.Height);
			//actualTsar.Parent.Name.Should().Be(expectedTsar.Parent.Name);
			//actualTsar.Parent.Weight.Should().Be(expectedTsar.Parent.Weight);


			//Reflexy are better then unDRY
			var checkFields = new[] { "Age", "Name", "Weight", "Height" };
			actualTsar.Should().HaveSameFieldsAs(expectedTsar, checkFields);
			actualTsar.Parent.Should().HaveSameFieldsAs(expectedTsar.Parent, checkFields);

			// Перепишите код на использование Fluent Assertions.
			//Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
			//Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
			//Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
			//Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);

			//Assert.AreEqual(expectedTsar.Parent.Name, actualTsar.Parent.Name);
			//Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
			//Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
			//Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);
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
			// Кто-нибудь может начать использовать этот метод, думая что он проверяет нормальное равество полей, но будет проверять усеченное, что ведет к трудноотлавлеваемым ошибкам.
			// По хорошему нужно перегрузить Equals и GetHashCode, но это при условии, что нам действительно нужно такое рекурстивное сравнение.
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

	public static class FluentAssertionExtenstion
	{
		//I tried to generalized it to using MemberInfo, but it has no succsess(
		public static void HaveSamePropertiesAs(this FluentAssertions.Primitives.ObjectAssertions actual, object expexted, IEnumerable<string> properties = null)
		{
			var type = actual.Subject.GetType();
			if (type != expexted.GetType())
				throw new ArgumentException("Object types must be the same.");
			properties = properties ?? type.GetProperties().Select(x => x.ToString());
			foreach (var prop in properties.Select(type.GetProperty))
				prop.GetValue(actual.Subject).Should().Be(prop.GetValue(expexted));
		}
		public static void HaveSameFieldsAs(this FluentAssertions.Primitives.ObjectAssertions actual, object expexted, IEnumerable<string> fields = null)
		{
			var type = actual.Subject.GetType();
			if (type != expexted.GetType())
				throw new ArgumentException("Object types must be the same.");
			fields = fields ?? type.GetFields().Select(x => x.ToString());
			foreach (var field in fields.Select(type.GetField))
				field.GetValue(actual.Subject).Should().Be(field.GetValue(expexted));
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