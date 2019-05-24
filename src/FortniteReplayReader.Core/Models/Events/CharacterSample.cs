using FortniteReplayReader.Core.Contracts;

namespace FortniteReplayReader.Core.Models.Events
{
    public class CharacterSample : IEvent
    {
        public EventMetadata EventMetadata { get; set; }
        public string Unknown { get; set; }
    }
}