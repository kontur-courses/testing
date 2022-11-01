using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar_TwoIdenticalTsars_ShouldBeEqual()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			//.AllowingInfiniteRecursion() будет иметь в случае, если у человека (царя) > 10 предков
			actualTsar.Should().BeEquivalentTo(expectedTsar, options =>
				options.AllowingInfiniteRecursion()
					.Excluding(o => Regex.IsMatch(o.SelectedMemberPath, @"(Id)$")));
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		//Недостатки такого подхода:
		// 1) Неинформативно (в результате падения теста в логгере мы увидим лишь Expected: "True But was:  False".
		//	В предложенном варианте видим очень наглядно, на каких данных тест проваливается;
		// 2) Низкая читаемость. В дереве тестов не будет видно, каким должен быть результат теста.
		//	В предложенном варианте имя метода переименовано в соответствии со стандартом названия тестов (в виде спецификации);
		// 3) Тест тяжело масштабировать. При добавлении новых свойств в класс Person нужно добавлять их в метод AreEqual вручную.
		//	В предложенном решении все добавляемые свойста автоматически добавляются в список для сравнения.
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

		
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
			return new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null)); ;
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