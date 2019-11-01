using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class ObjectComparison
    {
	    private Person actualTsar;
        Person expectedTsar;
        [SetUp]
	    public void SetUp()
	    {
		     actualTsar = TsarRegistry.GetCurrentTsar();
		    expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			    new Person("Vasili III of Russia", 28, 170, 60, null));
	    }

        [Test]
        [Description("Проверка текущего царя")]
        [Category("ToRefactor")]
        public void CheckCurrentTsar()
        {
            actualTsar
                .Should().BeEquivalentTo(expectedTsar,
                    config => config
                        .Excluding(o => o.SelectedMemberInfo.Name == nameof(Person.Id)));
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            // 1. Функция AreEqual может содержать ошибки, не проверять некоторые поля,
            // при чтении её реализации необходимо помнить о том как устроены другой кусок кода что затрудняет проверку,
            // 2. при этом в случае не совпадения каких-то полей информациию о том,
            // какое же поле не совпало с предполагаемым придется добывать сомастоятельно,
            // 3. рекурсивный алгоритм может привести к бесконечному циклу
            //например:
            var first = new Person("Ivan IV The Terrible", 54, 170, 70, null);
            first.Parent = first;
            var second = new Person("Ivan IV The Terrible", 54, 170, 70, null);
            second.Parent = second;
            //4. не понятно какие поля учавствуют в сравнении а какие нет
            //5. при появлении новых полей нужно переписывать метод
			//6.Информация возвращаемая данным тестом не дает понимание где и как произошла ошибка
			//а значит и не совпадение выявить сложно Экспектед тру бат вос фолс не очень информативно

            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));
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