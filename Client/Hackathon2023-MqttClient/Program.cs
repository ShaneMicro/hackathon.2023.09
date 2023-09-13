namespace Hackathon2023_MqttClient
{
    internal class Program
    {
        public static void Main()
        {
            Console.WriteLine("Client is running...");

            MqttHelper MqttService = new MqttHelper("***", 8883, "***", "***");

            MqttService.Run();
            
            while (true) { } //preventing the program to exit - it keeps the MQTT client listening to the server
        }
    }
}
