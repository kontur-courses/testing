namespace HomeExercises
{
	public static class TsarRegistry
	{
		public static Person GetCurrentTsar()
		{
			return new Person(
				"Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
		}
	}

	public class Person
	{
		private static int _idCounter;
		public readonly int Age;
		public readonly int Height;
		public readonly int Weight;
		public readonly string Name;
		public Person? Parent;
		public int Id;


		public Person(string name, int age, int height, int weight, Person? parent)
		{
			Id = _idCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}