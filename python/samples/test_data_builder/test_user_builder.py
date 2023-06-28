# фикстуры из файла conftest.py импортируются автоматически
from user_builder import TestUserBuilder


class TestTestDataBuilderSample:
    def test_works_with_object_mother(self, regular_user, admin):
        assert regular_user.name == "Triniti"
        assert regular_user.login == "tri"
        assert regular_user.password == "asdasd"
        assert regular_user.role == "ROLE_USER"

        assert admin.name == "Agent Smith"
        assert admin.login == "smith"
        assert admin.password == "qweqwe"
        assert admin.role == "ROLE_ADMIN"

    def test_works_with_builder(self):
        user = TestUserBuilder.user().build()
        admin_user = TestUserBuilder.user().in_admin_role().with_login("very_admin").build()
        assert user.name == "John Smith"
        assert user.login is None
        assert user.password == "42"
        assert user.role == "ROLE_USER"
        assert admin_user.name == "John Smith"
        assert admin_user.login == "very_admin"
        assert admin_user.password == "42"
        assert admin_user.role == "ROLE_ADMIN"
