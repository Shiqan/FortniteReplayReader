using FortniteReplayReader.Core.Contracts;

namespace FortniteReplayReader.Core.Models.Events
{
    public class TeamStats : IEvent
    {
        public uint Unknown { get; set; }
        public uint Position { get; set; }
        public uint TotalPlayers { get; set; }
        public EventMetadata EventMetadata { get; set; }
    }
}
