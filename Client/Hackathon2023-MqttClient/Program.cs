namespace Hackathon2023_MqttClient
{
    internal class Program
    {
        private const string MQTT_SERVER_ADDRESS = "PUT_YOUR_SERVER_ADDRESS_HERE"; //e.g., a36fd4c.myserver.com
        private const ushort MQTT_SERVER_PORT = 8883;
        private const string MQTT_SERVER_USERNAME = "PUT YOUR USERNAME";
        private const string MQTT_SERVER_PASSWORD = "PUT YOUR PASSWORD";
        private const string MQTT_TOPIC = "hivemqdemo/commands";

        public static void Main()
        {
            Console.WriteLine("Client is running...");

            MqttHelper MqttService = new MqttHelper(MQTT_SERVER_ADDRESS, MQTT_SERVER_PORT, MQTT_SERVER_USERNAME, MQTT_SERVER_PASSWORD, MQTT_TOPIC);

            MqttService.Run();
            
            while (true) { } //preventing the program to exit - it keeps the MQTT client listening to the server
        }
    }
}
