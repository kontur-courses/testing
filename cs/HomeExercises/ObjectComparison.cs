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

            /*
				1. Изменять тест придется, только при изменении или добавлении членов класса, которые нужно исключить
				   из сравнения.
				2. Если забыть добавить новые поля, которые нужно исключить из сравнения, то тест упадет 
				   на эквивалентных экземплярах.
			 */

            actualTsar
                .Should()
                .BeEquivalentTo(expectedTsar,
                    option =>
                        option.Excluding(ctx => ctx.SelectedMemberInfo.Name == nameof(Person.Id) &&
                                                ctx.SelectedMemberInfo.DeclaringType.Name == nameof(Person))
                .AllowingInfiniteRecursion());
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            /*
				Какие недостатки у такого подхода?

				1. Разные классы в объектном графе потребуют создания разных методов для их обхода.
				2. Изменение названий учавствующих в сравнении полей и строк потребует изменения тестов.
				3. Добавление новых полей и свойств, учавствующих в сравнении также потребует изменения тестов.
				4. Если забыть добавить проверку новых полей класса в AreEqual, то тест будет проходить на экземплярах,
				   которые не равны.
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