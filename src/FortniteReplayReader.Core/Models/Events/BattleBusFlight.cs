using FortniteReplayReader.Core.Contracts;

namespace FortniteReplayReader.Core.Models.Events
{
    public class BattleBusFlight : IEvent
    {
        public EventMetadata EventMetadata { get; set; }
    }
}