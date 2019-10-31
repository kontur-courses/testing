using FluentAssertions;
using NUnit.Framework;
using System;

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

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,1,
				new Person("Vasili III of Russia", 28, 170, 60,1, null));


            //        actualTsar.ShouldBeEquivalentTo(expectedTsar, options => options
            //.Excluding(p => p.SelectedMemberPath.EndsWith("Id")));

            //Перепишите код на использование Fluent Assertions.
                        actualTsar.ShouldBeEquivalentTo(expectedTsar, options =>
                options
                        .Including(f => f.SelectedMemberPath.EndsWith("Name")
                        || f.SelectedMemberPath.EndsWith("Age")
                        || f.SelectedMemberPath.EndsWith("Height")
                        || f.SelectedMemberPath.EndsWith("Weight")
                        || f.SelectedMemberPath.EndsWith("Parent")
                        || f.SelectedMemberDescription.Contains("Property")));

        }
 

        [Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,1,
				new Person("Vasili III of Russia", 28, 170, 60,1, null));

            // Какие недостатки у такого подхода? 
            /* 1) При таком подходе мы не получаем информацию о том, где конкретно произошла ошибка
             * и в каких полях/свойствах у нас появились несовпадения.
             * 2) Также рекурсивный метод 
             * может привести к ошибке StackOverflowException. FluentAssertions же не даёт рекурсии
             * опуститься глубже 10 шагов, если мы явно этого не разрешим.
             * 3) При расширении класса Person нам придётся дописывать метод, что ухудшит его читаемость
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
				"Ivan IV The Terrible", 54, 170, 70,1,
				new Person("Vasili III of Russia", 28, 170, 60,1, null));
		}
	}

	public class Person
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person Parent;
		public int Id;
        public int TestProperty { get; set; }

        public Person(string name, int age, int height, int weight, int test, Person parent)
		{
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
            TestProperty = test;
		}
	}
}
