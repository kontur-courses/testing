package ru.kontur.courses.samples;

import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;

import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;

public class Parametrized {

    @ParameterizedTest
    @ValueSource(strings = {"123", "1.1", "1.1e1", "1.1e-1", "-0.1"})
    public void parseDouble(String input) {
        assertDoesNotThrow(() -> {
            Double.parseDouble(input);
        });
    }
}
