namespace Hackathon2023_MqttClient
{
    internal class Program
    {
        public static void Main()
        {
            Console.WriteLine("Client is running...");

            MqttHelper MqttService = new MqttHelper("the_url", 8883, "username", "password");

            MqttService.Run();
            
            while (true) { } //preventing the program to exit - it keeps the MQTT client listening to the server
        }
    }
}
