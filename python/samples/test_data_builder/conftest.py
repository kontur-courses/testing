import pytest

from user import User


@pytest.fixture
def regular_user():
    return User(name="Triniti", role="ROLE_USER", login="tri", password="asdasd")


@pytest.fixture
def admin():
    return User(name="Agent Smith", role="ROLE_ADMIN", login="smith", password="qweqwe")
