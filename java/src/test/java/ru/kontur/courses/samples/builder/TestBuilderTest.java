package ru.kontur.courses.samples.builder;

import org.junit.jupiter.api.Test;

public class TestBuilderTest {

    @Test
    public void worksWithObjectMother() {
        var user = TestUsers.asRegularUser();
        var adminUser = TestUsers.anAdmin();
        // Если в тестах нужно много комбинаций параметров, будет комбинаторный взрыв.
    }

    @Test
    public void worksWithBuilder() {
        var user = TestUserBuilder.aUser().build();
        var adminUser = TestUserBuilder.aUser().inAdminRole().build();
        // Такой код не ломается, при смене сигнатуры конструктора User.
        // Это важно, если таких тестов много.
    }
}
