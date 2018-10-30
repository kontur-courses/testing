using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;

namespace HomeExercises
{
	public static class SelfReferenceEquivalencyAssertionOptionsExtensions
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

			var fieldName = nameof(Person.Id);

            actualTsar.Should().BeEquivalentTo(expectedTsar, options => options.ExcludeField(fieldName));
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
		//1. Если полей будет в разы больше, то написание проверки на эквивалентность
		//	 для каждого поля будет занимать много времени. Это нецелесообразно, отнимает
		//	 много сил и времени, которые можно потратить на что-то более полезное.
		//2. Такой тест крайне неинформативен: при его провале, разработчик не сможет сразу понять
		//   почему и на каких данных этот тест провалился.
		//3. При чтении такого теста придётся заглянуть и в реализацию метода AreEqual, и в
		//   реализацию самого класса Person, чтобы понять, что при сравнении игнорируется
		//   поле Id.
		// [Пояснения]
		//1. Использование Fluent Assertions сильно улучшает читаемость кода - тесты
		//	 действительно становятся аналогом документации.

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