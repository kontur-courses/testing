using NUnit.Framework;

namespace MakeItFluent
{
	public class ObjectComparison
	{
		public Person actualTsar;
		public Person expectedTsar;

		[SetUp]
		public void SetUp()
		{
			actualTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
			expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
		}

		[Test]
		[Description("Сравниваем царей")]
		[Category("Easy")]
		public void ObjectComparisonTest()
		{
			// Имеется два царя actualTsar и expectedTsar. Нужно их сравнить используя FluentAssertions.
			Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
			Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
			Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
			Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);

			Assert.AreEqual(expectedTsar.Parent.Name, actualTsar.Parent.Name);
			Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
			Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
			Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);

			// Пример кастомного ассерта. 
			// Рассказать наставнику, чем такой кастомный ассерт уступает FluentAssertion'у.
			Assert.True(AreEqualPersons(actualTsar, expectedTsar));
		}

		private bool AreEqualPersons(Person actual, Person expected)
		{
			if (actual.Parent != null && expected.Parent != null)
				return actual.Name == expected.Name &&
				       actual.Age == expected.Age &&
				       actual.Height == expected.Height &&
				       actual.Weight == expected.Weight &&
				       AreEqualPersons(actual.Parent, expected.Parent);
			return actual.Name == expected.Name &&
			       actual.Age == expected.Age &&
			       actual.Height == expected.Height &&
			       actual.Weight == expected.Weight;
		}

		public class Person
		{
			public int Age, Height, Weight;
			public string Name;
			public Person Parent;

			public Person(string name, int age, int height, int weight, Person parent)
			{
				// Ментальная гимнастика: после того, как решишь эту задачу, представь:
				// 1) Полей не 5, а 15. Соответственно Используя обычный Assert.AreEqual на
				// каждое поле, нужно написать не 8, а 28 проверок.
				// 2) Каждое из полей сложное, т.е. не относится к системным типам или ещё
				// лучше, что это большие объекты. Тогда количество проверок растёт геометрически
				Name = name;
				Age = age;
				Height = height;
				Weight = weight;
				Parent = parent;
			}
		}
	}
}