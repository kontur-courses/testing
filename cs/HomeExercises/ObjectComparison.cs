using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
			Assert.True(TsarEquals(typeof(Person), actualTsar, expectedTsar, new HashSet<string> {"IdCounter", "Id"}));
			// Не работает(((
			//actualTsar.Should().BeEquivalentTo(expectedTsar, config => config.Excluding(p => p.Id));
		}
		
		private static bool TsarEquals(IReflect type, Person? actual, Person? expected, ICollection<string> propertiesDontCompare)
		{
			if (actual == expected)
				return true;

			var properties = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			
			foreach (var propertyInfo in properties)
			{
				if (propertiesDontCompare.Contains(propertyInfo.Name))
					continue;
				
				var actualPropertyValue = propertyInfo.GetValue(actual);
				var expectedPropertyValue = propertyInfo.GetValue(expected);
				
				if (actualPropertyValue == null && expectedPropertyValue == null)
					continue;
				if (actualPropertyValue is null || expectedPropertyValue is null)
					return false;

				if (propertyInfo.FieldType == typeof(Person))
				{
					if (!TsarEquals(typeof(Person), actualPropertyValue as Person, expectedPropertyValue as Person, propertiesDontCompare))
						return false;
				}
				else if (!actualPropertyValue.Equals(expectedPropertyValue)) 
					return false;
			}

			return true;
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// Если убрать поле у класса Person, то этот тест будет вылетать с ошибкой при попытке доступа к несуществующему полю
			// Если добавить новые поля, то придётся дописывать их в тесте
			// Если не дописать, то разные значения новых полей будут проходить тест, чего быть не должно
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