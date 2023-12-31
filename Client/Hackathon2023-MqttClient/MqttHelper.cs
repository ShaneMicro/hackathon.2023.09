﻿using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using HiveMQtt.Client.Results;
using HiveMQtt.MQTT5.ReasonCodes;

namespace Hackathon2023_MqttClient
{
    public class MqttHelper
    {
        private HiveMQClientOptions options;
        private readonly string MQTT_TOPIC;

        public MqttHelper(string hostname, ushort port, string username, string password, string topic)
        {
            options = new HiveMQClientOptions
            {
                Host = hostname,
                Port = port,
                UseTLS = true,
                UserName = username,
                Password = password,
            };
            MQTT_TOPIC = topic;
        }

        public async Task Run()
        {
            var client = new HiveMQClient(options);

            Console.WriteLine($"Connecting to {options.Host} on port {options.Port} ...");

            // Connect
            ConnectResult connectResult;
            try
            {
                connectResult = await client.ConnectAsync().ConfigureAwait(false);
                if (connectResult.ReasonCode == ConnAckReasonCode.Success)
                {
                    Console.WriteLine($"Connection succeeded: {connectResult}");
                }
                else
                {
                    // FIXME: Add ToString
                    Console.WriteLine($"Connection failed: {connectResult}");
                    Environment.Exit(-1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error connecting to the MQTT Broker with the following error: {e.Message}");
                Environment.Exit(-1);
            }

            client.OnMessageReceived += (sender, args) =>
            {
                string received_message = args.PublishMessage.PayloadAsString;
                Console.WriteLine($"--> Command received: {received_message}");

                foreach(string cmd in received_message.Split(';')){
                    KeyboardHelper.Run(cmd);
                }
            };

            Console.WriteLine($"Subscribing to topic {MQTT_TOPIC}...");
            
            SubscribeResult result = await client.SubscribeAsync(MQTT_TOPIC).ConfigureAwait(true);
        }

    }
}
