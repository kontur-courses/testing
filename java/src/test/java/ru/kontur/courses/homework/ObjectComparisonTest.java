package ru.kontur.courses.homework;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class ObjectComparisonTest {

    @Test
    public void chechCurrentTasr() {
        var actualTsar = TsarRegistry.getCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

        // Перепишите код на использование Fluent assertions assertJ
        assertEquals(actualTsar.name, expectedTsar.name);
        assertEquals(actualTsar.age, expectedTsar.age);
        assertEquals(actualTsar.height, expectedTsar.height);
        assertEquals(actualTsar.weight, expectedTsar.weight);

        assertEquals(expectedTsar.parent.name, actualTsar.parent.name);
        assertEquals(expectedTsar.parent.age, actualTsar.parent.age);
        assertEquals(expectedTsar.parent.height, actualTsar.parent.height);
        assertEquals(expectedTsar.parent.parent, actualTsar.parent.parent);
    }

    @Test
    public void checkCurrentTsarWithCustomEquality() {
        var actualTsar = TsarRegistry.getCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода?
        assertTrue(areEqual(actualTsar, expectedTsar));
    }

    private boolean areEqual(Person actual, Person expected) {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
                actual.name.equals(expected.name)
                        && actual.age == expected.age
                        && actual.height == expected.height
                        && actual.weight == expected.weight
                        && areEqual(actual.parent, expected.parent);
    }
}
