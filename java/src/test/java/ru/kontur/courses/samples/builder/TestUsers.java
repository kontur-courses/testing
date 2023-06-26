package ru.kontur.courses.samples.builder;

public class TestUsers {

    public static User asRegularUser() {
        return new User("Triniti", "tri", "asdasd", "ROLE_USER");
    }

    public static User anAdmin() {
        return new User("Agent Smith", "smith", "qweqwe", "ROLE_ADMIN");
    }
}
