namespace HomeExercises;

public class Person
{
    public static int IdCounter;
    public int Age, Height, Weight;
    public int Id;
    public string Name;
    public Person? Parent;

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