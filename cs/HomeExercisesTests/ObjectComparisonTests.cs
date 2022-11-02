﻿using FluentAssertions;
using HomeExercises;
using NUnit.Framework;

namespace HomeExercisesTests;

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

		actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
			.Excluding(ctx => ctx.DeclaringType == typeof(Person) && ctx.Name == nameof(Person.Id))
			.AllowingInfiniteRecursion()
			.IgnoringCyclicReferences());
	}

	[Test]
	[Description("Альтернативное решение. Какие у него недостатки?")]
	public void CheckCurrentTsar_WithCustomEquality()
	{
		var actualTsar = TsarRegistry.GetCurrentTsar();
		var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			new Person("Vasili III of Russia", 28, 170, 60, null));

		// Какие недостатки у такого подхода?
		// - При падении теста без дебаггинга не получится понять на каком поле и уровне вложенности произошла ошибка.
		//   Похоже на антипаттерн Over specification
		// - При добавлении в класс новых полей необходимо корректировать тест
		// - Читаемость теста и легкость в проверке теста ниже, чем у теста с FluentAssertions
		// - Для того, чтобы понять логику работы теста придется искать и спускаться до AreEqual
		// - Если одна из Person будет ссылаться на Person, который уже есть выше по цепочке,
		//	 этот тест уйдет в бесконечную рекурсию и упадет на StackOverflowException
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