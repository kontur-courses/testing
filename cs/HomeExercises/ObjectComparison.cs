using System.Collections.Generic;
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
            // Перепишите код на использование Fluent Assertio
            actualTsar.Name.Should().Be(expectedTsar.Name);
            actualTsar.Age.Should().Be(expectedTsar.Age);
            actualTsar.Height.Should().Be(expectedTsar.Height);
            actualTsar.Weight.Should().Be(expectedTsar.Weight);
            expectedTsar.Parent.Name.Should().Be(actualTsar.Parent.Name);
            expectedTsar.Parent.Age.Should().Be(actualTsar.Parent.Age);
            expectedTsar.Parent.Height.Should().Be(actualTsar.Parent.Height);
            expectedTsar.Parent.Parent.Should().Be(actualTsar.Parent.Parent);
            //или так
            actualTsar.ShouldBeEquivalentTo(expectedTsar, opitons => opitons.Excluding(x => x.SelectedMemberPath.EndsWith("Id")));
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

			Assert.IsTrue(AreEqual(actualTsar,expectedTsar));
            //В таком подходе в независимости от количества полей класса person можно сравнивать объекты этого класса по всем возможным полям
            //Так же можно указать те поля, которые сравнивать не нужно
            //Пишется намного меньше кода, однако читаемость у него такая себе
            actualTsar.ShouldBeEquivalentTo(expectedTsar, opitons => opitons.Excluding(x => x.SelectedMemberPath.EndsWith("Id")));
            //в старом подходе используется рекурсия для сравнения поля parent, что может вызвать переполнение StackOverflow,
            //рекурсию в старом способе можно заменить на  stack, тогда память будет выделяться в куче а не в стэке, а размер куча = ОЗУ, размер стека фиксирован
        }
		// Примерно так выглядит нерекурсивный метод, можно было и по другому
        private bool AreEqual(Person actual, Person expected)
        {
            var personStack = new Stack<Person>();
            personStack.Push(actual);
            while (personStack.Count != 0)
            {
                actual = personStack.Pop();
                if (actual == expected) return true;
                if (actual == null || expected == null) return false;
                if (actual.Name == expected.Name
                    && actual.Age == expected.Age
                    && actual.Height == expected.Height
                    && actual.Weight == expected.Weight)
                {
	                personStack.Push(actual.Parent);
	                expected = expected.Parent;
                }
                else return false;
            }
            return false;
        }
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
