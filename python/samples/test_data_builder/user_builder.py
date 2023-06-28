from user import User


class TestUserBuilder:
    DEFAULT_NAME = "John Smith"
    DEFAULT_ROLE = "ROLE_USER"
    DEFAULT_PASSWORD = "42"

    def __init__(self):
        self.name = self.DEFAULT_NAME
        self.password = self.DEFAULT_PASSWORD
        self.role = self.DEFAULT_ROLE
        self.login = None

    @classmethod
    def user(cls):
        return cls()

    def with_name(self, new_name: str) -> 'TestUserBuilder':
        self.name = new_name
        return self

    def with_login(self, new_login: str) -> 'TestUserBuilder':
        self.login = new_login
        return self

    def with_password(self, new_password: str) -> 'TestUserBuilder':
        self.password = new_password
        return self

    def with_no_password(self) -> 'TestUserBuilder':
        self.password = None
        return self

    def in_user_role(self) -> 'TestUserBuilder':
        return self.in_role("ROLE_USER")

    def in_admin_role(self) -> 'TestUserBuilder':
        return self.in_role("ROLE_ADMIN")

    def in_role(self, new_role: str) -> 'TestUserBuilder':
        self.role = new_role
        return self

    def but(self) -> 'TestUserBuilder':
        return self.user().in_role(self.role).with_name(self.name).with_password(self.password).with_login(self.login)

    def build(self) -> User:
        return User(name=self.name, role=self.role, login=self.login, password=self.password)

    @staticmethod
    def regular_user() -> User:
        return TestUserBuilder.user().build()

    @staticmethod
    def admin() -> User:
        return TestUserBuilder.user().with_name("Neo").with_login("neo").in_admin_role().build()
