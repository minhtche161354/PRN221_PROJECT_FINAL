// using System;
using Newtonsoft.Json;
using StoreServer.Models;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        TcpClient client = new TcpClient("127.0.0.1", 1500);

        using (NetworkStream stream = client.GetStream())
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            // Convert the received bytes to a string
            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            var deserializedData = JsonConvert.DeserializeObject<List<TestModel>>(receivedData);

            Console.WriteLine("Received data from server:");
            foreach(var item in deserializedData)
            {
                Console.WriteLine(item.ToString());
            }
        }
        Console.ReadLine();
        client.Close();
    }
}