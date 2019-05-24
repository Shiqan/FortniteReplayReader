using FortniteReplayReader.Core.Contracts;

namespace FortniteReplayReader.Core.Models.Events
{
    public class ZoneUpdate : IEvent
    {
        public EventMetadata EventMetadata { get; set; }
    }
}