using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		private static Dictionary<string, object?> GetFields(Person? person)
		{
			return person?.GetType()
				       .GetFields()
				       .Where(f => f.Name != "Parent" && f.Name != "Id")
				       .Select(f => KeyValuePair.Create(f.Name, f.GetValue(person)))
				       .ToDictionary(k => k.Key, v => v.Value)
			       ?? new Dictionary<string, object?>();
		}

		private static void FieldsChecking
			(IDictionary<string, object?> actual, IDictionary<string, object?> expected)
		{
			actual.Count.Should().Be(expected.Count);
			foreach (var key in expected.Keys)
			{
				actual.Should().ContainKey(key, $"should be field {key}");
				actual[key].Should().Be(expected[key], $"{key} should be {expected[key]}");
			}
		}

		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar_WithReflection()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			//этот подход лучше, т.к. есть дополнительная инфа о том, где произошла ошибка
			//также он не зависит от добавления новых полей,
			// минус в том, что если по новому полю не нужно сравнивать, придется дополнять метод GetFields

			FieldsChecking(GetFields(actualTsar), GetFields(expectedTsar));
			FieldsChecking(GetFields(actualTsar.Parent), GetFields(expectedTsar.Parent));
		}

		[Test]
		public void CheckCurrentTsar_WithoutReflection()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			var unwantedMembers = new Regex("\\.Id$|^Id$", RegexOptions.Compiled);

			//Данное решение проще, чем с рефлексией и понятнее

			actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
				.Excluding(p => unwantedMembers.IsMatch(p.SelectedMemberPath)));
		}


		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Малоинформативен и можно забыть переписать AreEqual при добавлении новых полей
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