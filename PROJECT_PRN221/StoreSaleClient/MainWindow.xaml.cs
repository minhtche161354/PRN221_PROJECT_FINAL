using Newtonsoft.Json;
using StoreSaleClient.Models;
using StoreSaleClient.NormModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoreSaleClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private List<Product> products;
        public List<Product> Page;
        public static int Index;
        public MainWindow()
        {
            products = new List<Product>();
            Index = 0;
            InitializeComponent();
            int error_counter = 0;
            if (!ConnectToServer() && error_counter <= 3)
            {
                error_counter++;
                MessageBox.Show("Server is not connected! Retrying...");
            }
            loadData();

        }
        public async void loadData()
        {
            try
            {
                products.Clear();
                string hold = await SendRequest("get_all/**/");
                products = JsonConvert.DeserializeObject<List<Product>>(hold);
                if (products != null)
                {
                    if (products.Skip(Index * 6).Count() > 0)
                    {
                        Page = products.Skip(Index * 6).Take(6).ToList();
                    }
                }

                GenerateBlocks(Page);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Increase auto Interface
        /// </summary>
        /// <returns></returns>
        private void GenerateBlocks(List<Product> page)
        {
            // Set the number of columns in the UniformGrid
            blockUniformGrid.Columns = 3;

            foreach (Product product in page)
            {
                var block = new Border
                {
                    Background = Brushes.LightBlue,
                    Margin = new Thickness(10),
                    Padding = new Thickness(10)
                };

                var textBlock = new TextBlock
                {
                    Text = product.ProductName,
                    Foreground = Brushes.DarkBlue,
                    FontSize = 16
                };
                var image = new Image
                {
                    Source = new BitmapImage(new Uri("D:\\project_PRN\\Project_PRN221_ShopSale_ClientServer\\PROJECT_PRN221\\StoreSaleClient\\Images\\product.png")),
                    Width = 50,
                    Height = 50
                };
                var button = new Button
                {
                    Name = "buttonO" + product.ProductId,
                    Content = "Add Product",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom
                };


                var quantityTextBox = new TextBox
                {
                    Name = "numberO" + product.ProductId,
                    Text = "1",
                    Width = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                button.Click += (sender, e) =>
                {
                    int quantity;
                    if (int.TryParse(quantityTextBox.Text, out quantity))
                    {
                        AddToCurrentOrder(product.ProductId, quantity);
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity!");
                    }
                };
                var stackPanel = new StackPanel
                {
                    Margin = new Thickness(10)
                };
                stackPanel.Children.Add(image);
                stackPanel.Children.Add(textBlock);
                stackPanel.Children.Add(button);
                stackPanel.Children.Add(quantityTextBox);


                block.Child = stackPanel;

                blockUniformGrid.Children.Add(block);
            }
        }
        private List<OrderModel> listOrder;
        private void AddToCurrentOrder(int productID, int quant)
        {
            if (listOrder == null)
            {
                listOrder = new List<OrderModel>();

            }
            var productHolder = products.Where(x => x.ProductId == productID).ToList().FirstOrDefault();
            if (productHolder != null)
            {
                if (productHolder.Quantity < quant)
                {
                    MessageBox.Show("Invalid quantity!");
                }
                else
                {
                    listOrder.Add(new OrderModel
                    {
                        Product = products.Where(x => x.ProductId == productID).ToList().FirstOrDefault(),
                        quantity = quant
                    });
                }
            }

        }
        private bool ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 1500);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private async Task<string> SendRequest(string request)
        {
            string[] holder = request.Split("/**/");
            string mode = holder[0];
            string data = holder[1];
            if (client == null || !client.Connected)
            {
                MessageBox.Show("Server is not connected...");
                return null;
            }
            using (NetworkStream stream = client.GetStream())
            {
                byte[] RequestBytes = Encoding.UTF8.GetBytes(request);
                await stream.WriteAsync(RequestBytes, 0, RequestBytes.Length);

                switch (mode)
                {
                    case "get_all":
                        {
                            byte[] buffer = new byte[2048];
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
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
                            return "";
                        }
                }
            }
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            int error_counter = 0;
            if (!ConnectToServer() && error_counter <= 3)
            {
                error_counter++;
                MessageBox.Show("Server is not connected! Retrying...");
            }
            blockUniformGrid.Children.Clear();
            if (Index > 0)
                Index--;
            else
                Index = 0;
            loadData();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            int error_counter = 0;
            if (!ConnectToServer() && error_counter <= 3)
            {
                error_counter++;
                MessageBox.Show("Server is not connected! Retrying...");
            }
            blockUniformGrid.Children.Clear();
            if((Index+1)*6<=products.Count())
            Index++;
            loadData();
        }
    }
}
