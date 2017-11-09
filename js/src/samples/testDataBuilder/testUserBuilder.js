import User from "./user";

const DEFAULT_NAME = "John Smith";
const DEFAULT_ROLE = "ROLE_USER";
const DEFAULT_PASSWORD = "42";


export default class TestUserBuilder {
    constructor() {
        this.name = DEFAULT_NAME;
        this.password = DEFAULT_PASSWORD;
        this.role = DEFAULT_ROLE;
        this.login = null;
    }

    static aRegularUser() {
        return this.aUser().build();
    }

    static anAdmin() {
        return this.aUser()
            .withName("Neo")
            .withLogin("neo")
            .inAdminRole()
            .build();
    }

    static aUser() {
        return new TestUserBuilder();
    }

    static but() {
        return this.aUser()
            .inRole(this.role)
            .withName(this.name)
            .withPassword(this.password)
            .withLogin(this.login)
    }

    withName(newName) {
        this.name = newName;
        return this;
    }

    withLogin(newLogin) {
        this.login = newLogin;
        return this;
    }

    withPassword(newPassword) {
        this.password = newPassword;
        return this;
    }

    withNoPassword() {
        this.password = null;
        return this;
    }

    inUserRole() {
        return this.inRole("ROLE_USER");
    }

    inAdminRole() {
        return this.inRole("ROLE_ADMIN");
    }

    inRole(newRole) {
        this.role = newRole;
        return this;
    }

    build() {
        return new User(this.name, this.login, this.password, this.role);
    }
}
