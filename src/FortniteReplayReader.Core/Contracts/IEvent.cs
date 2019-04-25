using FortniteReplayReader.Core.Models;

namespace FortniteReplayReader.Core.Contracts
{
    public interface IEvent
    {
        EventMetadata EventMetadata { get; set; }
    }
}
