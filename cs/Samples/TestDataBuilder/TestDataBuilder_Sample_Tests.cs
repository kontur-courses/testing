using NUnit.Framework;

namespace Samples.TestDataBuilder
{
	[TestFixture]
	public class TestDataBuilder_Sample_Tests
	{
		[Test]
		public void WithObjectMother()
		{
			var user = TestUsers.ARegularUser();
			var adminUser = TestUsers.AnAdmin();
			// Если в тестах нужно много комбинаций параметров, будет комбинаторный взрыв.
		}

		[Test]
		public void WithBuilder()
		{
			var user = TestUserBuilder.AUser().Build();
			var adminUser = TestUserBuilder.AUser().InAdminRole().Build();
			// Такой код не ломается, при смене сигнатуры конструктора User.
			// Это важно, если таких тестов много.
		}
	}
}