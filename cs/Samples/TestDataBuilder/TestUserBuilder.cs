﻿namespace Samples.TestDataBuilder
{
	public class TestUserBuilder
	{
		public const string DEFAULT_NAME = "John Smith";
		public const string DEFAULT_ROLE = "ROLE_USER";
		public const string DEFAULT_PASSWORD = "42";
		private string name = DEFAULT_NAME;
		private string password = DEFAULT_PASSWORD;
		private string role = DEFAULT_ROLE;
		private string login;

		private TestUserBuilder()
		{
		}

		public static TestUserBuilder AUser()
		{
			return new TestUserBuilder();
		}

		public TestUserBuilder WithName(string newName)
		{
			name = newName;
			return this;
		}

		public TestUserBuilder WithLogin(string newLogin)
		{
			login = newLogin;
			return this;
		}

		public TestUserBuilder WithPassword(string newPassword)
		{
			password = newPassword;
			return this;
		}

		public TestUserBuilder WithNoPassword()
		{
			password = null;
			return this;
		}

		public TestUserBuilder InUserRole()
		{
			return InRole("ROLE_USER");
		}

		public TestUserBuilder InAdminRole()
		{
			return InRole("ROLE_ADMIN");
		}

		public TestUserBuilder InRole(string newRole)
		{
			role = newRole;
			return this;
		}

		public TestUserBuilder But()
		{
			return AUser()
				.InRole(role)
				.WithName(name)
				.WithPassword(password)
				.WithLogin(login);
		}

		public User Build()
		{
			return new User(name, login, password, role);
		}

		public static User ARegularUser()
		{
			return AUser().Build();
		}

		public static User AnAdmin()
		{
			return AUser()
				.WithName("Neo")
				.WithLogin("neo")
				.InAdminRole()
				.Build();
		}
	}
}