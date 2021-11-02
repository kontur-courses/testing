namespace HomeExercises
{
    public class TsarRegistry
    {
        public static Person GetCurrentTsar()
        {
            return new Person("Ivan IV The Terrible", 54, 170, 70,
                 new Person("Vasili III of Russia", 28, 170, 60, null));
        }

        public static Person GetCurrentTsar0()
        {
            var p = new Person("", 28, 170, 60, null);
            p.Parent = p;
            return p;
        }
    }
}