using FluentAssertions;
using NUnit.Framework;
using System.Windows;

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

            actualTsar.ShouldBeEquivalentTo(expectedTsar, options => options
                .Excluding(ts => ts.Id)
                .Excluding(ts => ts.SelectedMemberPath.EndsWith("Parent.Id")));
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода?

            /* 1. Если Assert завершился неудачей, непонятно, из-за какого именно из полей
             * провалилось условие: само сообщение о неудаче не сообщает никакой
             * полезной информации, а только то, что тест "зафэйлился".
             * 2. При добавлении новых полей в Person можно забыть поменять метод AreEqual.
             * 3. В методе AreEqual нет защиты от рекурсивной зависимости объектов: 
             * если у двух объектов Parent - это они сами, то мы словим
             * StackOverflowException.
             * 4. Если появятся какие-либо сложные поля с отличающимися от основного класса
             * внутренней структурой и/или правилами определения равенства, придётся либо
             * нагромождать новыми проверками существующий метод AreEqual, либо
             * писать ещё один.
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