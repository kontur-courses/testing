class User:
    def __init__(self, name: str, role: str, login: str = None, password: str = None):
        self.name = name
        self.role = role
        self.login = login
        self.password = password
