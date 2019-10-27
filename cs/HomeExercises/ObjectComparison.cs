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

            #region моя реализация
            actualTsar.Should().BeEquivalentTo(expectedTsar, 
                config => config.Excluding(o => o.SelectedMemberInfo.Name == nameof(Person.Id)));
            #endregion

            #region старая реализация
            
            Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
			Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
			Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
			Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);

			Assert.AreEqual(expectedTsar.Parent.Name, actualTsar.Parent.Name);
			Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
			Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
			Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);

            #endregion
        }

        [Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода? 

            #region описание недостатовков и сравнение с моей реализацией
            /*
             1) если тест падает, то нам бы хотелось знать на каких значениях, 
                но Assert.True вернет true или false. В моей реализации можно посмотреть на каких данных
                тест упал

             2) при добавлении/удалении свойства нужно будет дописывать/удалять строку со сравнением
                этого свойства. В моей реализации нужно только исключить свойства, которые не нужно проверять
             
             3) функция AreEqual привязана к классу Person. Если мы заходим сравнить экземпляры 
                другого класса, то нам нужно будет писать для него свою функцию AreEqual. В моей реализации
                достаточно заменить названия свойств, которые не нужно проверять
             
             4) Моя реализация более компактна

             5) Непонятно как сравнивает AreEqual т.е, чтобы понять, что AreEqual не сравнивает ID, нужно
                смотреть реализацию. В моей реализации явно указано какие свойства проверяться не нужно

             6) т.к Assert.True - самописная функция, то может произойти так, что в ней могут быть ошибки,
                которые приведут к неверному результату. В моей реализации не используются самописные функции
            */
            #endregion

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