namespace Samples.TestDataBuilder
{
	public class User
	{
		private readonly string login;
		private readonly string name;
		private readonly string password;
		private readonly string role;

		public User(string name, string login, string password, string role)
		{
			this.name = name;
			this.login = login;
			this.password = password;
			this.role = role;
		}
	}
}