using FortniteReplayReader.Core.Models.Events;

namespace FortniteReplayObservers.Mqtt
{
    public class MqttObserver : BaseMqttObserver<PlayerElimination>
    {
        public MqttObserver() : base()
        {
        }
    }
}