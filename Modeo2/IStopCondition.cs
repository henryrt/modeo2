namespace RTH.Modeo2
{
    public interface IStopCondition
    {
        bool ShouldStop(ICollectionManager cm);
        void Initialize();
    }
}