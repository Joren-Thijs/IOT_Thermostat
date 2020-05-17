﻿using IOT_Thermostat.API.DeviceClient.MQTT.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.Rpc;
using MQTTnet.Extensions.Rpc.Options;
using MQTTnet.Protocol;
using System;
using System.Threading.Tasks;

namespace Mqtt.Client.AspNetCore.DeviceClient
{
    public class MqttDeviceClient : IDeviceClient
    {
        private readonly IMqttClientOptions Options;

        private readonly IMqttRpcClientOptions rpcOptions;

        private IMqttClient client;

        private MqttRpcClient rpcClient;

        public MqttDeviceClient()
        {
            Options = MqttDeviceClientOptionsLoader.LoadMqttClientOptions();
            rpcOptions = MqttDeviceClientOptionsLoader.LoadMqttRpcClientOptions();
            client = new MqttFactory().CreateMqttClient();
            rpcClient = new MqttRpcClient(client, rpcOptions);
            SetupClient();
        }

        private void SetupClient()
        {
            client.UseApplicationMessageReceivedHandler(OnMessageAsync);
        }

        public virtual async Task OnMessageAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            string topic = eventArgs.ApplicationMessage.Topic;
            System.Console.WriteLine("A message is received");
            System.Console.WriteLine(topic);
            if (eventArgs.ApplicationMessage.Topic.EndsWith("/ping"))
            {
                await RespondToPing(topic);
            }

            if (eventArgs.ApplicationMessage.Topic.EndsWith("/ms"))
            {
                await HandleMeasurement(eventArgs.ApplicationMessage);
            }
        }

        private async Task RespondToPing(string topic)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic + "/response")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();
            await client.PublishAsync(message);
        }

        private async Task HandleMeasurement(MqttApplicationMessage message)
        {
            
        }

        public async Task StartClientAsync()
        {
            await client.ConnectAsync(Options);
            System.Console.WriteLine("Client is connected");
            await client.SubscribeAsync("+/ms");
            await client.SubscribeAsync("+/ping");
            System.Console.WriteLine("Subscribed on a channel");
            if(!client.IsConnected)
            {
                await client.ReconnectAsync();
            }
        }

        public Task StopClientAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task SetDeviceStatus(string deviceName)
        {
            string topic = deviceName + ".cmd.status";
            string payload = "{\"status\":\"true\"}";
            await rpcClient.ExecuteAsync(TimeSpan.FromSeconds(5), topic, payload, MqttQualityOfServiceLevel.AtLeastOnce);
        }
    }
}
