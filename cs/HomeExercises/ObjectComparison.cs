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
			
			
			actualTsar.Should().BeEquivalentTo(
				expectedTsar,
				options => options
					.IncludingNestedObjects()
					.AllowingInfiniteRecursion()
					.IncludingFields()
					.Excluding(ctx => $".{ctx.SelectedMemberPath}".EndsWith(".Id")
					)
			);
			
			
			// Перепишите код на использование Fluent Assertions.
			// Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
			// Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
			// Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
			// Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);
			//
			// Assert.AreEqual(expectedTsar.Parent!.Name, actualTsar.Parent!.Name);
			// Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
			// Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
			// Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			// 
			// 1)	Этот тест должен проверять метод GetCurrentTsar(),
			//	т.е. тест проверяет то, что текущий царь - тот, которого мы ожидаем.
			//	Но в этом решении есть написанный нами метод AreEqual(Person? actual, Person? expected).
			//	И может показаться, что мы проверяем этот метод сравнения,
			//	особенно, если его расположить в другом месте, например в Person.
			//
			// 2)	Метод AreEqual тоже требует отдельныйх тестов, т.к. он написан нами и содержит логику. 
			//
			// 3)	Но, так или иначе, если метод AreEqual написан с ошибкой,
			//	то упадет и этот тест, даже если текущий царь - верный.
			//
			// 4)	При добавлении нового поля в Person нужно не забыть добавить его в метод AreEqual.
			//
			// 5)	Не предусмотренно обработка рекурсии. Во-первых, возможно исключение StackOverflowException,
			//	во-вторых нет настройки для максимальной глубины рекурсии при сравнении. 
			//
			// 6)	Использование метода Assert.True влечет плохую читаемость кода,
			//	а также абсолютно не информативное сообщение при провале теста
			//
			// 7)	Такое решение менее гибкое, чем использование BeEquivalentTo.
			
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