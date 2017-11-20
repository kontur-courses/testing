import * as testUsers from "./testUsers";
import TestUserBuilder from "./testUserBuilder";


describe("testDataBuilder sample tests", () => {
    it("works with ObjectMother", () => {
        const user = testUsers.aRegularUser();
        const adminUser = testUsers.anAdmin();
        // Если в тестах нужно много комбинаций параметров, будет комбинаторный взрыв.
    });

    it("works with Builder", () => {
        const user = TestUserBuilder.aUser().build();
        const adminUser = TestUserBuilder.aUser().inAdminRole().build();
        // Такой код не ломается, при смене сигнатуры конструктора User.
        // Это важно, если таких тестов много.
    });
});
