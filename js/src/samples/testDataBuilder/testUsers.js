import User from "./user";

export function aRegularUser() {
    return new User("Triniti", "tri", "asdasd", "ROLE_USER");
}

export function anAdmin() {
    return new User("Agent Smith", "smith", "qweqwe", "ROLE_ADMIN");
}
