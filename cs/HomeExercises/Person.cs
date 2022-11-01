namespace HomeExercises
{
	public class Person
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person? Parent;
		public int Id;

		public Person(string name, int age, int height, int weight, Person? parent)
		{
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}

		public static bool Equal(Person? actual, Person? expected)
		{
			if (actual == expected) return true;
			if (expected == null) return false;
			return
				actual.Name == expected.Name
				&& actual.Age == expected.Age
				&& actual.Height == expected.Height
				&& actual.Weight == expected.Weight
				&& Equal(actual.Parent, expected.Parent);
		}
		
		public bool Equal(Person? expected)
		{
			return Equal(this, expected);
		}
	}
}