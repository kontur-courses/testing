namespace HomeExercises
{
    public class Person
    {
        public static int IdCounter = 0;
        public int Age, Height, Weight;
        public string Name;
        public Person? Parent;
        public int Id;
        public County country;

        public Person(string name, int age, int height, int weight, Person? parent, County? county)
        {
            Id = IdCounter++;
            Name = name;
            Age = age;
            Height = height;
            Weight = weight;
            Parent = parent;
            this.country = country;
        }
    }

    public class County
    {
        public int Id;
    }
}