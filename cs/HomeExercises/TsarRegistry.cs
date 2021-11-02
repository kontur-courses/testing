namespace HomeExercises
{
    public class TsarRegistry
    {
        //public static Person GetCurrentTsar()
        //{
        //   return new Person("Ivan IV The Terrible", 54, 170, 70, 
        //        new Person("Vasili III of Russia", 28, 170, 60, null));
        //}

        //public static Person GetCurrentTsar11()
        //{
        //    return
        //        new Person("", 28, 170, 60,
        //        new Person("", 28, 170, 60,
        //        new Person("", 28, 170, 60,
        //        new Person("", 28, 170, 60,
        //        new Person("", 28, 170, 60,
        //        new Person("", 28, 170, 60,
        //        new Person("", 28, 170, 60,
        //        new Person("", 28, 170, 60,
        //        new Person("", 28, 170, 60,
        //        new Person("", 28, 170, 60,
        //        new Person("", 28, 170, 60,
        //        new Person("", 28, 170, 60, null))))))))))));
        //}

        public static Person GetCurrentTsar()
        {
            return new Person("Ivan IV The Terrible", 54, 170, 70,
                 new Person("Vasili III of Russia", 28, 170, 60, null, 
                 new County { Id = 4}), new County {Id=3 });
        }
    }
}