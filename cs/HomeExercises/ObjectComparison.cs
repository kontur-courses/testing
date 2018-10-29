using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;

namespace HomeExercises
{
	public static class FluentAssertionsExtensions
	{
		public static T ExcludeField<T>(
			this SelfReferenceEquivalencyAssertionOptions<T> options,
			string name) where T : SelfReferenceEquivalencyAssertionOptions<T>
			=> options.Excluding(info => info.SelectedMemberInfo.Name == name);
	}

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

			//Assert.AreEqual(expectedTsar.Parent.Name, actualTsar.Parent.Name);
			//Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
			//Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
			//Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);

			actualTsar.Should().BeEquivalentTo(expectedTsar, options 
				=> options.Excluding(f => f.SelectedMemberInfo.Name == "Id"));

            actualTsar.Should().BeEquivalentTo(expectedTsar, options => options.ExcludeField("Id"));

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
		}
		//Ответ:
		// [Недостатки "Альтернативного решения"]
		//1. При изменении названия любого из полей метод AreEqual придётся также изменить.
		//2. Если полей будет в разы больше, то написание проверки на эквивалентность
		//	 для каждого поля будет занимать много времени. Это нецелесообразно, отнимает
		//	 много сил и времени, которые можно потратить на что-то более полезное.
		// [Пояснения]
		//1. Использование Fluent Assertions сильно улучшает читаемость кода - тесты
		//	 действительно становятся аналогом документации.
		//2. Использовав Fluent Assertions, сама проверка заняла всего одну, крайне читаемую,
		//	 строку. В альтернативном же решении, был создан целый метод, который делает
		//	 то же самое.

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