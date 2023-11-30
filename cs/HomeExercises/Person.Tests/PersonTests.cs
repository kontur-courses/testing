using FluentAssertions;
using HomeExercises;
using NUnit.Framework;

namespace Person.Tests;

[TestFixture]
public class PersonTests
{
    [Test]
    public void Persons_ShouldBeEqual_When_HaveSamePropertiesAndParents()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new HomeExercises.Person("Ivan IV The Terrible", 54, 170, 70,
            new HomeExercises.Person("Vasili III of Russia", 28, 170, 60, null));

        actualTsar.Should().BeEquivalentTo(expectedTsar,
            config => config.Excluding(ctx => ctx.SelectedMemberInfo.Name == "Id"));
    }

    // Недостатки этого метода сравнения:
    // 1. AreEqual(...) сильно связан со структурой класса Person:
    //	  любая модификация этого класса может потребовать изменения проверяющего метода.
    // 2. AreEqual(...) включает в себя слишком много проверок. Из-за этого мы получим
    //    неинформативное сообщение об ошибке в случае разности проверяемых объектов.
    // 3. В методе типа AreEqual(...) легко забыть о каком-либо свойстве, т.е. допустить ошибку в сравнении.
    // 4. Если у проверяемого объекта много свойств, то AreEqual(...) тяжело читать.
    // 5. Возможно переполнение стека из-за рекурсивных вызовов AreEqual(...) внутри самого себя.
    // 6. Для любого другого класса придётся писать новую версию метода AreEqual(...).
    private bool AreEqual(HomeExercises.Person? actual, HomeExercises.Person? expected)
    {
        if (actual == expected)
            return true;

        if (actual == null || expected == null)
            return false;

        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}