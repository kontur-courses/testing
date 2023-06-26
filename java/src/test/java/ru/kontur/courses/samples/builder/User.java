package ru.kontur.courses.samples.builder;

public class User {
    private String login;
    private String name;
    private String password;
    private String role;

    public User(String login, String name, String password, String role) {
        this.login = login;
        this.name = name;
        this.password = password;
        this.role = role;
    }
}
