using FortniteReplayReader.Core.Contracts;
using FortniteReplayReader.Core.Models;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FortniteReplayObservers.Mqtt
{
    public class MqttObserver : FortniteObserver<PlayerElimination>, IObserver<PlayerElimination>
    {
        private IDisposable unsubscriber;
        private IMqttClient mqttClient;
        private Dictionary<PlayerElimination, int> _playerEliminations;

        public MqttObserver(Dictionary<PlayerElimination, int> playerEliminations)
        {
            _playerEliminations = playerEliminations ?? new Dictionary<PlayerElimination, int>();

            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId($"mqttnet_{Guid.NewGuid()}")
                .WithTcpServer("m24.cloudmqtt.com", 28142)
                .WithCredentials("hubuldat", "4hdHXzZ1q_LN")
                .WithTls()
                .Build();

            mqttClient.Connected += async (s, e) =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");
            };

            var task = mqttClient.ConnectAsync(options);
            task.Wait();
        }

        private string CreateTopic(PlayerElimination e)
        {
            return $"Fortnite/{e.Eliminator}/{e.Knocked}/{e.GunType}/";
        }
        private string CreateMessagePayload(PlayerElimination e)
        {
            return JsonConvert.SerializeObject(e);
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
            if (_playerEliminations.ContainsKey(value)) return;

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(CreateTopic(value))
                .WithPayload(CreateMessagePayload(value))
                .WithExactlyOnceQoS()
                .WithRetainFlag(false)
                .Build();

            var task = mqttClient.PublishAsync(message);
            task.Wait();
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
            var task = mqttClient.DisconnectAsync();
            task.Wait();

            mqttClient.Dispose();
            unsubscriber.Dispose();
        }
    }
}