// See https://aka.ms/new-console-template for more information
using Microsoft.IdentityModel.Tokens;
using System.Net.Sockets;
using System.Net;
using StoreServer.Models;
using Newtonsoft.Json;
using System.Text;

public class Program
{
    static async Task Main(string[] args)
    {
        string host = "127.0.0.1";
        int port = 1500;
        
        IPAddress localAddr = IPAddress.Parse(host);
        TcpListener server = new TcpListener(localAddr, port);
        server.Start();

        Console.WriteLine("Server Start!! Listening on port 1500...");

        while(true)
        {
            TcpClient client = await server.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);
        }
    }
    private static readonly List<TestModel> hold = new List<TestModel>() { new TestModel(1, "lmao") };
    static async Task HandleClientAsync(TcpClient client)
    {
        await using (NetworkStream stream = client.GetStream())
        {
            /*Prototype
            
            var data = hold;

            string serializedData = JsonConvert.SerializeObject(data);
            byte[] buffer = Encoding.UTF8.GetBytes(serializedData);

            await stream.WriteAsync(buffer, 0, buffer.Length);

             End_Of_Prototype*/

            byte[] hold = new byte[2048];
            int ReadSize = await stream.ReadAsync(hold, 0, hold.Length);
            //dich request sang string
            string request = Encoding.UTF8.GetString(hold, 0, ReadSize);
            //xu ly va gui lai phan hoi cho client
            string response = ProcessRequest(request);
            //gui lai client
            byte[] EndResonse = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(EndResonse, 0, EndResonse.Length);

        }
        client.Close();
    }
    static string ProcessRequest(string request)
    {
        try
        {
            string[] holder = request.Split("/**/");
            string mode = holder[0];
            string data = holder[1];
            switch (mode)
            {
                case "get_all":
                    {
                        return "";
                    }
                case "save_Bill":
                    {

                        return "";
                    }
                case "update_product":
                    {
                        return "";
                    }
                default:
                    {
                        return "Invalid request";
                    }
            }

        }
        catch (Exception ex)
        {
            return "Server error";
        }
    }
}