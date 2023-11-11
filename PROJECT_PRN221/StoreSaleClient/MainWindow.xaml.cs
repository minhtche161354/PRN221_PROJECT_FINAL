using Newtonsoft.Json;
using StoreSaleClient.Models;
using StoreSaleClient.NormModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
        private List<OrderModel> listOrder;
        //Navigation area
        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var selected = sidebar.SelectedItem as NavButton;

            navframe.Navigate(selected.Navlink);

        }
        private string searchString;
        private TcpClient client;
        private List<Product> products;
        public List<Product> Page;
        public static int Index;
        private readonly int NumOPage = 6;
        
        public MainWindow()
        {
            products = new List<Product>();
            Index = 0;
            InitializeComponent();
            Connector();
            loadData();
            DataContext = this;
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
                    if(!string.IsNullOrEmpty(searchString)) 
                    {
                    products = products.Where(x=>x.ProductName.ToLower().Contains(searchString.ToLower())).ToList();
                    }
                    if (products.Skip(Index * NumOPage).Count() > 0)
                    {
                        Page = products.Skip(Index * NumOPage).Take(NumOPage).ToList();
                    }
                }

                GenerateBlocks(Page);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public async void Search(string query)
        {
            try
            {
                products.Clear();
                string hold = await SendRequest("get_all/**/");
                products = JsonConvert.DeserializeObject<List<Product>>(hold);
                if (products != null)
                {
                    if (products.Skip(Index * NumOPage).Count() > 0)
                    {
                        Page = products.Skip(Index * NumOPage).Take(NumOPage).Where(x=>x.ProductName.ToLower().Contains(query)).ToList();
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
            blockUniformGrid.Children.Clear();
            foreach (Product product in page)
            {
                var block = new Border
                {
                    Background = Brushes.LightGray,
                    Margin = new Thickness(10),
                    Padding = new Thickness(10),
                    CornerRadius = new CornerRadius(8),
                };

                var textBlock = new TextBlock
                {
                    Text = product.ProductName,
                    Foreground = Brushes.DarkBlue,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center,
                };
                var textBlock2 = new TextBlock
                {
                    Text = product.Price.ToString(),
                    Foreground = Brushes.Red,
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
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
                    TextAlignment = TextAlignment.Center,
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
                stackPanel.Children.Add(textBlock2);
                stackPanel.Children.Add(button);
                stackPanel.Children.Add(quantityTextBox);
                


                block.Child = stackPanel;
                
                blockUniformGrid.Children.Add(block);
            }
        }
        
        private void AddToCurrentOrder(int productID, int quant)
        {
            if (listOrder == null)
            {
                lstOrderView.Items.Clear();
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
                
                lstOrderView.ItemsSource = null;
                lstOrderView.ItemsSource = listOrder;
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

        private void Connector()
        {
            try
            {
                int error_counter = 0;
                if (!ConnectToServer() && error_counter <= 3)
                {
                    error_counter++;
                    MessageBox.Show("Server is not connected! Retrying...");
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            Connector();
            blockUniformGrid.Children.Clear();
            if (Index > 0)
                Index--;
            else
                Index = 0;
            loadData();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            Connector();
            blockUniformGrid.Children.Clear();
            if((Index+1)* NumOPage <= products.Count())
            Index++;
            loadData();
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            Connector();
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                searchString = txtSearch.Text;
            }
            else
            {
                searchString = null;
            }
            loadData();
        }
    }
}
