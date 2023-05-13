package ru.kontur.courses.homework;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

public class NumberValidatorTest {

    @Test
    public void test() {
        assertThrows(IllegalArgumentException.class, () -> {
            new NumberValidator(-1, 2, true);
        });
        assertDoesNotThrow(() -> {
            new NumberValidator(1, 0, true);
        });
        assertThrows(IllegalArgumentException.class, () -> {
            new NumberValidator(-1, 2, false);
        });
        assertDoesNotThrow(() -> {
            new NumberValidator(1, 0, true);
        });

        assertTrue(new NumberValidator(17, 2, true).isValidNumber("0.0"));
        assertTrue(new NumberValidator(17, 2, true).isValidNumber("0"));
        assertTrue(new NumberValidator(17, 2, true).isValidNumber("0.0"));
        assertFalse(new NumberValidator(3, 2, true).isValidNumber("00.00"));
        assertFalse(new NumberValidator(3, 2, true).isValidNumber("-0.00"));
        assertTrue(new NumberValidator(17, 2, true).isValidNumber("0.0"));
        assertFalse(new NumberValidator(3, 2, true).isValidNumber("+0.00"));
        assertTrue(new NumberValidator(4, 2, true).isValidNumber("+1.23"));
        assertFalse(new NumberValidator(3, 2, true).isValidNumber("+1.23"));
        assertFalse(new NumberValidator(17, 2, true).isValidNumber("0.000"));
        assertFalse(new NumberValidator(3, 2, true).isValidNumber("-1.23"));
        assertFalse(new NumberValidator(3, 2, true).isValidNumber("a.sd"));
    }
}
