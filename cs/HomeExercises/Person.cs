namespace HomeExercises
{
	public class Person
	{
		private static int _idCounter;
		public readonly int Age, Height, Weight;
		public readonly string Name;
		public readonly Person? Parent;
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