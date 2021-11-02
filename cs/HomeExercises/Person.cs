using System;

namespace HomeExercises
{
	public class Person
	{
		public Guid Id { get; }
		public string Name { get; set; }
		public int Age { get; set; }
		public int Height { get; set; }
		public int Weight { get; set; }
		public Person? Parent { get; set; }

		public Person(string name, int age, int height, int weight, Person? parent)
		{
			Id = Guid.NewGuid();
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}