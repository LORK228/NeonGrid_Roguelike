namespace Time.Interfaces
{
    public interface ITimeTracker
    {
        void StartRewind();
        void StopRewind();
        void RecordState();
        void RewindState();
        void ClearHistory();
    }
}