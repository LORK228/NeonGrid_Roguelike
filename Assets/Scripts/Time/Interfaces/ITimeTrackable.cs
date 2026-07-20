namespace Time.Interfaces
{
    public interface ITimeTrackable
    {
        void RecordState();
        void RewindState();
        void StartRewind();
        void StopRewind();
    }
}