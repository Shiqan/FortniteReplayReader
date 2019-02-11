using FortniteReplayReader.Core.Contracts;
using FortniteReplayReader.Core.Models;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Threading.Tasks;

namespace FortniteReplayObservers.Mqtt
{
    public class MqttObserver : FortniteObserver<PlayerElimination>, IObserver<PlayerElimination>
    {
        private IDisposable unsubscriber;
        private IMqttClient mqttClient;

        public MqttObserver()
        {
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("m24.cloudmqtt.com", 28142)
                .WithCredentials("hubuldat", "4hdHXzZ1q_LN")
                .WithTls()
                .Build();

            mqttClient.Disconnected += async (s, e) =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await mqttClient.ConnectAsync(options);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            };

            mqttClient.Connected += async (s, e) =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");
            };

            var connected = mqttClient.ConnectAsync(options).Result;
        }

        private string CreateTopic(PlayerElimination e)
        {
            return $"Fortnite/{e.Eliminator}/{e.Knocked}/{e.GunType}/";
        }
        private string CreateMessagePayload(PlayerElimination e)
        {
            var type = (e.Knocked) ? "knocked" : "eliminated";
            return $"{e.Eliminator} {type} {e.Eliminated} with {e.GunType}";
        }

        public void OnCompleted()
        {
            this.Unsubscribe();
        }

        public void OnError(Exception error)
        {
            Unsubscribe();
        }

        public void OnNext(PlayerElimination value)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("Fortnite/Shiqan/1")
                .WithPayload(CreateMessagePayload(value))
                .WithExactlyOnceQoS()
                .WithRetainFlag(false)
                .Build();

            mqttClient.PublishAsync(message);
        }

        public override void Subscribe(IObservable<PlayerElimination> provider)
        {
            if (provider != null)
            {
                unsubscriber = provider.Subscribe(this);
            }
        }

        public override void Unsubscribe()
        {
            mqttClient.Dispose();
            unsubscriber.Dispose();
        }
    }
}