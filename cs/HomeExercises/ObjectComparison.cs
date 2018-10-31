using System;
using FluentAssertions;
using NUnit.Framework;
using FluentAssertions.Equivalency;

namespace HomeExercises
{
	public static class EquivalencyAssertionOptionsExcludeMemberExtension
	{
		public static EquivalencyAssertionOptions<T> ExcludeMember<T>(
			this EquivalencyAssertionOptions<T> options, String memberName)
		{
			return options.Excluding(ctx => 
				ctx.SelectedMemberInfo.Name == memberName &&
			    ctx.SelectedMemberInfo.DeclaringType == typeof(T));
		}
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

			actualTsar.ShouldBeEquivalentTo(expectedTsar, 
				options => options.ExcludeMember("Id"));
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
			 * 1 проблема: при падении теста не будет понятно, что именно упало
			 *		(доступна только информация expected True, actual False)
			 * 2 проблема: при изменении класса Person придётся изменять функцию AreEqual
			 * 3 проблема: существует вероятность уйти в бесконечную рекурсию при наличии циклических ссылок
			 *
			 * Подход с использованием FluentAssertions данных недостатков лишен:
			 *	место, где произошло несоответствие, показывается точно;
			 *	тест минимально зависит от реализации Person;
			 *	циклические ссылки отслеживаются автоматически
			 */
			Assert.True(AreEqual(actualTsar, expectedTsar));
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