using FluentAssertions;
using NUnit.Framework;
using HomeExercises;

namespace HomeExersises_Test
{
    [TestFixture]
    public class PersonComparison
    {
        [Test]
        [Description("Проверка текущего царя")]
        public void CheckCurrentTsar()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Если попадается циклическая ссылка в сообщении видно все несовпадающие поля,
            // и есть уведомление "Cyclic reference to type ... detected"
            actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
                .Excluding(o => o.Path == "Id" || o.Path.EndsWith(".Id"))
                .AllowingInfiniteRecursion());
        }

        [Test]
        [Description("Проверка текущего царя / Нужно внести изменения в метод AreEqual!")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            /*
            НЕДОСТАТКИ:
            1. Чтобы тест продолжал быть актуальным,
            при добавлении полей или свойств в класс Person,
            нужно также не забыть внести изменения в метод AreEqual.

            2. (общий) Полезнее будет переопределить метод Equals для Person.
            И в случае, когда мы не являемся одноверенно авторами тестируемого класса,
            лучше использовать метод Equals потому, что именно он будет использоваться
            для сравнения объектов класса в коде, а не наш тестовый "AreEqual".
            Наша проверка не учитывает то, как объекты должны сравниваться по задумке автора класса.
             
            3. В методе AreEqual много условий - есть простор для ошибки.
            При добавлении полей или свойств в Person, его понтенциально будет тяжело читать  
            
            5. Малоинформативное сообщение при падении теста
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
}