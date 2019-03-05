using FortniteReplayReader.Core.Contracts;
using FortniteReplayReader.Core.Models;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FortniteReplayObservers.Mqtt
{
    public class MqttObserver : FortniteObserver<PlayerElimination>, IObserver<PlayerElimination>
    {
        private IDisposable unsubscriber;
        private IMqttClient mqttClient;
        private Dictionary<PlayerElimination, int> _cache;
        private MqttSettings _settings;

        public MqttObserver(Dictionary<PlayerElimination, int> cache)
        {
            _settings = ReadSettingsFile<MqttSettings>();
            _cache = cache ?? new Dictionary<PlayerElimination, int>();

            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId($"mqttnet_{Guid.NewGuid()}")
                .WithTcpServer(_settings.HostName, _settings.Port)
                .WithCredentials(_settings.UserName, _settings.Password)
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
            if (!string.IsNullOrWhiteSpace(_settings.Topic))
            {
                // replace placeholders
                var topic = new StringBuilder(_settings.Topic);
                topic.Replace("[Eliminator]", e.Eliminator);
                topic.Replace("[Eliminated]", e.Eliminated);
                topic.Replace("[Knocked]", e.Knocked.ToString());
                topic.Replace("[GunType]", e.GunType.ToString());
                return $"Fortnite/{topic}";
            }
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
            if (_cache.ContainsKey(value)) return;

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

    public class MqttSettings
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Topic { get; set; }
    }
}