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

			actualTsar.Should().BeEquivalentTo(expectedTsar, options => 
				options.Excluding(p => p.SelectedMemberPath.EndsWith("Id")));
            // Плюсы такого подхода:
            // По сути, плюсами такого подхода является отсутствие перечисленных ниже минусов с:
            //
            // Примечание:
            // Без знания того, как работает метод BeEquivalentTo, преимущество над первым недостатком подхода,
            // использованного в тесте CheckCurrentTsar_WithCustomEquality, сводится на нет и, более того,
			// на то, чтобы разобраться, что именно происходит в этом методе, уйдет еще больше времени
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

            // Недостатки такого подхода: 
			// 1. Меньшая читаемость кода. Для того, чтобы понять, что именно происходит в этом тесте,
			//    приходится смотреть реализацию метода AreEqual
            // 2. Если тест свалится, то не будет показано, при проверке какого именно поля возникла ошибка
            // 3. Меньшая расширяемость. При каждом добавлении нового поля/свойства в класс Person
            //    придется прописывать дополнительную проверку в методе AreEqual
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