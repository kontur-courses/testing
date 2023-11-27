using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя. Информативная реализация")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			// Эта реализация теста будет выводить информативные сообщения!
			
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// В начальном наборе тестов был сделан акцент на то, что у объекта класса Person 
			// должнен быть родитель первого поколения.
			actualTsar.Parent.Should().NotBe(null, "Должен быть родитель 1-го поколения");
			
			// Полностью повторяем логику проверки до рефакторинга. 
			// Из родителей проверяем только первое поколение, причём не сравниваем у них вес.
			
			actualTsar.Should().BeEquivalentTo(expectedTsar, config =>
			{
				return config.Excluding(person => person.Id)
					.Excluding(person => person.Parent!.Parent)
					.Excluding(person => person.Parent!.Weight)
					.Excluding(person => person.Parent!.Id);
			});
		}
		
		[Test]
		[Description("Проверка текущего царя и всех его родителей")]
		[Category("AlternativeTest")]
		public void CheckCurrentTsar_WithCustomEquality_Informative()
		{
			// Этим шаблоном мы исключаем все идентификаторы из проверки.
			// То есть исключаем Id у 1-го, 2-го, ..., n - 1, n родителя.
			const string excludingPattern = @"^(Parent\.)*Id$";
			
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Эта версия теста является информативным аналогом метода CheckCurrentTsar_WithCustomEquality(...)
			// До сих пор возможна глубокая рекурсия!
			actualTsar.Should().BeEquivalentTo(expectedTsar, config =>
			{
				return config.Excluding(ctx => Regex.IsMatch(ctx.SelectedMemberPath, excludingPattern));
			});
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			/* Недостатки этого подхода:
			 * 1. AreEqual(...) сильно связан со структурой класса Person:
			 *	  любая модификация этого класса может потребовать изменения проверяющего метода.
			 * 2. AreEqual(...) включает в себя слишком много проверок. Из-за этого мы получим
			 *    неинформативное сообщение об ошибке в случае разности проверяемых объектов.
			 * 3. В методе типа AreEqual(...) легко забыть о каком-либо свойстве, т.е. допустить ошибку в сравнении.
			 * 4. Если у проверяемого объекта много свойств, то AreEqual(...) тяжело читать.
			 * 5. Возможно переполнение стека из-за рекурсивных вызовов AreEqual(...) внутри самого себя.
			 * 6. Для любого другого класса придётся писать новую версию метода AreEqual(...).
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