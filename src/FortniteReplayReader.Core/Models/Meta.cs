using System;

namespace FortniteReplayReader.Core.Models
{
    public class ReplayMetadata
    {
        public uint LengthInMs { get; set; }
        public uint NetworkVersion { get; set; }
        public uint Changelist { get; set; }
        public string FriendlyName { get; set; }
        public DateTime Timestamp { get; set; }
        public long TotalDataSizeInBytes { get; set; }
        public bool IsLive { get; set; }
        public bool IsCompressed { get; set; }
        public ReplayVersionHistory FileVersion { get; set; }
    }
}
