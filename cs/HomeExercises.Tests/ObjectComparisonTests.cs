using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.Tests;

public class ObjectComparisonTests
{
	[Test]
	[Description("Проверка текущего царя")]
	[Category("ToRefactor")]
	public void CheckCurrentTsar()
	{
		var actualTsar = TsarRegistry.GetCurrentTsar();

		var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			new Person("Vasili III of Russia", 28, 170, 60, null));

		actualTsar.Should().BeEquivalentTo(expectedTsar,
			options => options.Excluding(memberInfo =>
				memberInfo.SelectedMemberInfo.Name == nameof(Person.Id)
				&& memberInfo.SelectedMemberInfo.DeclaringType == typeof(Person)));
	}

	[Test]
	[Description("Альтернативное решение. Какие у него недостатки?")]
	public void CheckCurrentTsar_WithCustomEquality()
	{
		var actualTsar = TsarRegistry.GetCurrentTsar();
		var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			new Person("Vasili III of Russia", 28, 170, 60, null));

		/* Какие недостатки у такого подхода?
		 1. При добавлении нового поля придется править метод AreEqual (наиболее вероятно)
		 2. Легко совершить ошибку в методе AreEqual или забыть дописать какую-либо проверку
		 3. Сложнее читается из-за метода AreEqual
		 4. При проверке с помощью FluentAssertions будет показано какое поле не прошло проверку, здесь не будет
		 5. Ппроверка с помощью FLuentAssertion выглядит более лаконично
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