using System;
using System.Collections.Generic;
using System.Text;

namespace HomeExercises
{
	public class Person
	{
		public static int IdCounter { get; private set; }
		public int Age { get; }
		public int Height { get; }
		public int Weight { get; }
		public string Name { get; }
		public Person? Parent { get; }
		public int Id { get; }

		public Person(string name, int age, int height, int weight, Person? parent)
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
