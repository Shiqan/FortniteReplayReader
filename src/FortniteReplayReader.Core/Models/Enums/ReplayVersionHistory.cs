namespace FortniteReplayReader.Core.Models
{
    public enum ReplayVersionHistory : uint
    {
        Initial = 0,
        FixedSizeFriendlyName = 1,
        Compression = 2,
        RecordedTimestamp = 3,
        StreamChunkTimes = 4,
        FriendlyNameEncoding = 5,
        NewVersion,
        Latest = NewVersion - 1
    }
}