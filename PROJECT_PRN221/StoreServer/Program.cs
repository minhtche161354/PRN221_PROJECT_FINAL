// See https://aka.ms/new-console-template for more information
using Microsoft.IdentityModel.Tokens;
using System.Net.Sockets;
using System.Net;
using StoreServer.Models;
using Newtonsoft.Json;
using System.Text;
using StoreSaleClient.NormModels;
using Microsoft.EntityFrameworkCore;

public class Program
{
    static async Task Main(string[] args)
    {
        string host = "127.0.0.1";
        int port = 1500;
        Name = "default";
        IPAddress localAddr = IPAddress.Parse(host);
        TcpListener server = new TcpListener(localAddr, port);
        server.Start();

        Console.WriteLine("Server Start!! Listening on port 1500...");

        while (true)
        {
            TcpClient client = await server.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);
        }
    }
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
            //Xu ly image
            if (request.StartsWith("ImageMode"))
            {
                // Process image data
                byte[] imageData = await ReceiveImageData(stream);
                ProcessImageData(Name,imageData);
                // Gui lai client
                string response = "Image received successfully!";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            else
            {
                // Process non-image data
                string response = ProcessRequest(request);
                // gui lai client
                byte[] EndResonse = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(EndResonse, 0, EndResonse.Length);
            }



        }
        client.Close();
    }
    private static string Name;
    static async Task<byte[]> ReceiveImageData(NetworkStream stream)
    {
        byte[] headerBuffer = new byte[4];
        int headerBytesRead = await stream.ReadAsync(headerBuffer, 0, headerBuffer.Length);

        if (headerBytesRead < 4)
        {
            Console.WriteLine("Invalid Image");
            return null;
        }
        /*
        int ReadSize = await stream.ReadAsync(holdFileName, 0, holdFileName.Length);
        string Filename = Encoding.UTF8.GetString(holdFileName, 0, ReadSize);
        fileName = Filename;
        Console.WriteLine(Filename);
        */
        // get the file name
        byte[] fileNameHeader = new byte[4];
        int HeaderFileNameRead = await stream.ReadAsync(fileNameHeader, 0, fileNameHeader.Length);

        int fileNameSize = BitConverter.ToInt32(fileNameHeader, 0);

        byte[] fileNameBuffer = new byte[fileNameSize];

        int fileNameBytesRead = await stream.ReadAsync(fileNameBuffer, 0, fileNameBuffer.Length);

        if (fileNameBytesRead < fileNameSize)
        {
            Console.WriteLine("Invalid File Name");
            return null;
        }
        string fileName = Encoding.UTF8.GetString(fileNameBuffer, 0, fileNameBytesRead);
        Name = null;
        Name = fileName;

        Console.WriteLine($"{fileName} bytes");


        int imageSize = BitConverter.ToInt32(headerBuffer, 0);
        byte[] imageBuffer = new byte[imageSize];
        int totalBytesRead = 0;
        int endByteRead = 0;
        while (totalBytesRead < imageSize)
        {
            int bytesRead = await stream.ReadAsync(imageBuffer, totalBytesRead, imageSize - totalBytesRead);
            endByteRead = bytesRead;
            if (bytesRead == 0)
            {
                Console.WriteLine("Connection Error");
                return null;
            }

            totalBytesRead += bytesRead;
   
    }
        Console.WriteLine($"Read {endByteRead} bytes. Total bytes read: {totalBytesRead}, Expected size: {imageSize}");

        return imageBuffer;
    }
    static void ProcessImageData(string fileName,byte[] imageData)
    {
        // Specify the folder path where you want to save the images
        string imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");

        // Create the Images folder if it doesn't exist
        if (!Directory.Exists(imagesFolderPath))
        {
            Directory.CreateDirectory(imagesFolderPath);
        }

        // Combine the folder path and the received filename to get the full path
        string imagePath = Path.Combine(imagesFolderPath, fileName);

        // Save the image data to the specified path
        File.WriteAllBytes(imagePath, imageData);

        // Optionally, you can perform additional actions or return a response to the client
        Console.WriteLine($"Image saved successfully at: {imagePath}");
    }
    static string ProcessRequest(string request)
    {
        using (Project_PRN221_ServerContext lmao = new Project_PRN221_ServerContext())
            try
            {
                string[] holder = request.Split("/**/");
                string mode = holder[0];
                string data = holder[1];
                switch (mode)
                {
                    case "get_all":
                        {
                            var ResultHolder = lmao.Products.Include(x=>x.Category).IgnoreAutoIncludes().ToList();
                            return JsonConvert.SerializeObject(ResultHolder);
                        }
                    case "save_Bill":
                        {
                            var BillHolder = JsonConvert.DeserializeObject<Bill>(data);
                            lmao.Bills.Add(BillHolder);
                            lmao.SaveChanges();
                            return "Success";
                        }
                    case "get_all_cate":
                        {
                            var ResultHolder = lmao.Categories.ToList();
                            return JsonConvert.SerializeObject(ResultHolder);
                        }
                    case "login_User_get":
                        {
                            var UserHolder = JsonConvert.DeserializeObject<LoginUser>(data);
                            var ResultHolder = lmao.Employees.Where(userName => userName.Email.Equals(UserHolder.UserName) && userName.Password.Equals(UserHolder.Password)).ToList().FirstOrDefault();
                            if (ResultHolder != null)
                            {
                                UserHolder.Details = ResultHolder;
                                return JsonConvert.SerializeObject(UserHolder);
                            }
                            return "Wrong UserName Or PassWord!!";
                        }
                    case "add_product":
                        {
                            var Productholder = JsonConvert.DeserializeObject<Product>(data);
                            lmao.Products.Add(Productholder);
                            lmao.SaveChanges();
                            return JsonConvert.SerializeObject(Productholder.ProductId);
                        }
                    case "update_product":
                        {
                            var Productholder = JsonConvert.DeserializeObject<Product>(data);
                            string result=null;
                            var updateProductHolder = lmao.Products.Where(x => x.ProductId == Productholder.ProductId).FirstOrDefault();
                            if (updateProductHolder != null)
                            {
                                result = JsonConvert.SerializeObject(updateProductHolder);
                                updateProductHolder.ProductName = Productholder.ProductName;
                                updateProductHolder.ExpirationDate = Productholder.ExpirationDate;
                                updateProductHolder.Price = Productholder.Price;
                                updateProductHolder.CategoryId = Productholder.CategoryId;
                                updateProductHolder.WarehouseArrivalDate = Productholder.WarehouseArrivalDate;
                                updateProductHolder.ProductImg = Productholder.ProductImg;
                                updateProductHolder.Quantity = Productholder.Quantity;
                            }
                            lmao.SaveChanges();
                            return result;
                        }
                    case "delete_product":
                        {
                            var intIdHolder = JsonConvert.DeserializeObject<int>(data);
                            var ProductHolder = lmao.Products.Where(x=>x.ProductId == intIdHolder).FirstOrDefault();
                            if(ProductHolder != null)
                            {
                                lmao.Remove(ProductHolder);
                                lmao.SaveChanges();
                            }
                            return JsonConvert.SerializeObject(intIdHolder);
                        }
                    case "image_mode":
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
                Console.WriteLine(ex.ToString());
                return "Server error " + ex.ToString();
            }
    }
}