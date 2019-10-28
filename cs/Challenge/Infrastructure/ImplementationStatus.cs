namespace Challenge.Infrastructure
{
	public class ImplementationStatus
	{
		public ImplementationStatus(string name, string[] fails)
		{
			Name = name;
			Fails = fails;
		}

		public readonly string Name;
		public readonly string[] Fails;
	}
}