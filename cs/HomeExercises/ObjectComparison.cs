using FluentAssertions;
using FluentAssertions.Equivalency;
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

            AssertionOptions.AssertEquivalencyUsing(
                options => options.Excluding(subject => IsId(subject)));

            actualTsar.ShouldBeEquivalentTo(expectedTsar);
        }

        private static bool IsId(ISubjectInfo subject)
        {
            var memberInfo = subject.SelectedMemberInfo;
            return memberInfo.DeclaringType == typeof(Person) &&
                   memberInfo.Name == nameof(Person.Id);
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода? 
            /*Если хотя бы одно поле не совпадает (например, имя), то
             * тест не пройдет (в данной случае будет неинформативное сообщение, 
             * что ожидалось значение true, но было false) и будет неизвестно, 
             * какое именно поле не совпало. Тогда как при использовании FluentAssertions 
             * будет указано, где именно ошибка (строка), а также, что именно ожидалось 
             * и что было получено. 
             * Данный метод автоматически не реагирует на расширение класса. 
             * Также дублирует все поля для сравнения.
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
        public static int IdCounter;
        public int Age, Height, Weight;
        public int Id;
        public string Name;
        public Person Parent;

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