namespace HomeExercises
{
	public class ObjectComparison
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public ObjectComparison? Parent;
		public int Id;

		public ObjectComparison(string name, int age, int height, int weight, ObjectComparison? parent)
		{
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}