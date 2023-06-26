package ru.kontur.courses.samples.builder;

public class TestUserBuilder {
    public final String DEFAULT_NAME = "";
    public final String DEFAULT_ROLE = "";
    public final String DEFAULT_PASSWORD = "";
    private String name = DEFAULT_NAME;
    private String password = DEFAULT_PASSWORD;
    private String role = DEFAULT_ROLE;
    private String login;

    private TestUserBuilder() {
    }

    public static TestUserBuilder aUser() {
        return new TestUserBuilder();
    }

    public TestUserBuilder withName(String newName) {
        name = newName;
        return this;
    }

    public TestUserBuilder withLogin(String newLogin) {
        login = newLogin;
        return this;
    }

    public TestUserBuilder withPassword(String newPassword) {
        password = newPassword;
        return this;
    }

    public TestUserBuilder withNoPassword() {
        password = null;
        return this;
    }

    public TestUserBuilder inUserRole() {
        return inRole("ROLE_USER");
    }

    public TestUserBuilder inAdminRole() {
        return inRole("ROLE_ADMIN");
    }

    public TestUserBuilder inRole(String newRole) {
        this.role = newRole;
        return this;
    }

    public TestUserBuilder but() {
        return aUser().inRole(role).withName(name).withPassword(password).withLogin(login);
    }

    public User build() {
        return new User(name, login, password, role);
    }

    public User aRegularUser() {
        return aUser().build();
    }

    public User anAdmin() {
        return aUser()
                .withName("Neo")
                .withLogin("neo")
                .inAdminRole().build();
    }
}
