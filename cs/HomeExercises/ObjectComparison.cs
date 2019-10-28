using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
        /* 
         * Чем были плохи исходные тесты?
         *      1) Трудно читать
         *      2) Легко при написании теста упустить важные поля, которые надо проверить
         *      3) Трудно поддерживать (при добавлении новых полей в класс People приходится фиксить тест)
         *      4) Если тест упал, то не понятно, что именно не так.
         *      
         * Почему сейчас хорошо?
         *      1) Тест стал интуитивно понятный
         *      2) Все нужные поля проверяются под капотом метода BeEquivalentTo, а те, которые надо исключить прописываем явно
         *      3) Теперь при изменении класса People тест менять не надо
         *      4) Понятный вывод в случае неудачи теста из которого можно понять, где ошибка.
         * 
         */
        [Test]
        [Description("Проверка текущего царя")]
        [Category("ToRefactor")]
        public void CheckCurrentTsar()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();

            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));


            actualTsar.Should()
                      .BeEquivalentTo(expectedTsar, options => options.Excluding(info => info.SelectedMemberInfo.Name.Equals(nameof(Person.Id))));
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            /* Какие недостатки у такого подхода? 
             * 
             * Будет непоняно в чем ошибка, 
             * сложно читать
             * неочевидно, что тест написан правильно
             * легко ошибиться в написании такого теста
             * 
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