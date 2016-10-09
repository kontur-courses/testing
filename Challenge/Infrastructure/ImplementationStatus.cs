namespace Challenge.Infrastructure
{
    public class ImplementationStatus
    {
        public ImplementationStatus(string name, int failsCount)
        {
            Name = name;
            FailsCount = failsCount;
        }

        public string Name;
        public int FailsCount;
    }
}