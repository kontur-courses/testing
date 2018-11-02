using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		/* Достоинства подхода ниже:
		 *
		 * + Отлично читается
		 *
		 * + Выдает информативные сообщение при непрохождении - а именно: какое поле не совпало
		 *
		 * + Легко расширяем. Например, достаточно написать допольнительные условия в .Excluding
		 *
		 * + Использует методы .Equals() соответствующих классов, которые уже где-то определены
		 */

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
					.AllowingInfiniteRecursion()  // ибо по дефолту 10 уровней вложенности
					.Excluding(prop =>
						prop.SelectedMemberInfo.DeclaringType == typeof(Person) &&
						prop.SelectedMemberInfo.Name == nameof(Person.Id)));
		}


		/* Недостатки подхода ниже:
		 *
		 * + При непрохождении теста, на экран выведется Expected: True\n But was: False.
		 * 	 Данное сообщение вообще неинформативно
		 *
		 * + По сути здесь реализован метод .Equals() класса Person. Но такие вещи надо реализовывать
		 *   внутри класса Person. Далее AreEqual будет сам вызывать переопределенные методы
		 *
		 * + Каждый раз при добавлении поля необходимо добавлять новую строчку в метод AreEqual
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