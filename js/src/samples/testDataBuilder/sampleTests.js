import * as testUsers from "./testUsers";
import TestUserBuilder from "./testUserBuilder";


describe("testDataBuilder sample tests", () => {
    it("WithObjectMother", () => {
        const user = testUsers.aRegularUser();
        const adminUser = testUsers.anAdmin();
        // Если в тестах нужно много комбинаций параметров, будет комбинаторный взрыв.
    });

    it("WithBuilder", () => {
        const user = TestUserBuilder.aUser().build();
        const adminUser = TestUserBuilder.aUser().inAdminRole().build();
        // Такой код не ломается, при смене сигнатуры конструктора User.
        // Это важно, если таких тестов много.
    });
});
