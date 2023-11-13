using Microsoft.Win32;
using Newtonsoft.Json;
using StoreSaleClient.Models;
using StoreSaleClient.NormModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
            if (navframe.CanGoBack)
            {
                navframe.GoBack();
            }

        }
        private string searchString;
        private TcpClient client;
        private List<Product> products;
        public List<Product> Page;
        public static int Index;
        private readonly int NumOPage = 9;
        public static LoginUser loginUser;

        public MainWindow()
        {
            products = new List<Product>();
            Index = 0;
            InitializeComponent();
            Connector();
            loadData();
            lstOrderView.ItemsSource = null;
            DataContext = this;
        }
        public void login()
        {
            loginUser = new LoginUser
            {
                UserName = "defaultUser",
                Role = 0
            };
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
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        products = products.Where(x => x.ProductName.ToLower().Contains(searchString.ToLower())).ToList();
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
                        Page = products.Skip(Index * NumOPage).Take(NumOPage).Where(x => x.ProductName.ToLower().Contains(query)).ToList();
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
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
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
                    Source = (string.IsNullOrEmpty(product.ProductImg)) ? new BitmapImage(new Uri("pack://application:,,,/Images/product_PlaceHolder.png")) : new BitmapImage(new Uri(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images", product.ProductImg))),
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
                bool flag = false;
                foreach (var item in listOrder)
                {
                    if (item.Product.ProductId == productHolder.ProductId)
                    {
                        if (productHolder.Quantity <= item.quantity)
                        {
                            MessageBox.Show("Invalid quantity!");
                            return;
                        }
                        item.quantity += quant;
                        flag = true;
                        lstOrderView.ItemsSource = null;
                        lstOrderView.ItemsSource = listOrder;
                        TotalTxtBlock.Text = getTotal().ToString() + " $";
                        return;
                    }
                }
                if (productHolder.Quantity < quant)
                {
                    MessageBox.Show("Invalid quantity!");
                }
                else
                {

                    if (!flag)
                        listOrder.Add(new OrderModel
                        {
                            Product = products.Where(x => x.ProductId == productID).ToList().FirstOrDefault(),
                            quantity = quant
                        });
                }
            }
            TotalTxtBlock.Text = getTotal().ToString() + " $";
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
                            byte[] buffer = new byte[100000];
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        }
                    case "get_all_cate":
                        {
                            byte[] buffer = new byte[2048];
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        }
                    case "save_Bill":
                        {
                            byte[] buffer = new byte[2048];
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        }
                    case "add_product":
                        {
                            byte[] buffer = new byte[2048];
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        }
                    case "update_product":
                        {
                            byte[] buffer = new byte[2048];
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        }
                    case "login_User_get":
                        {
                            return "";
                        }
                    case "delete_product":
                        {
                            byte[] buffer = new byte[2048];
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        }
                    case "ImageMode":
                        {
                            string filePath = !string.IsNullOrEmpty(data) && !System.IO.Path.GetFileName(data).Equals("product_PlaceHolder.png") ? data : null;
                            // Read the selected image file into a byte array
                            byte[] imageData = File.ReadAllBytes(filePath);

                            // Get the file name and extension
                            string fileName = System.IO.Path.GetFileName(filePath);
                            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

                            // byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                            // Send the image data to the server
                            byte[] header = BitConverter.GetBytes(imageData.Length);
                            await client.GetStream().WriteAsync(header, 0, header.Length);
                            // Send the file name size
                            byte[] fileNameHeader = BitConverter.GetBytes(fileNameBytes.Length);
                            await client.GetStream().WriteAsync(fileNameHeader, 0, fileNameHeader.Length);
                            //send the file

                            await client.GetStream().WriteAsync(fileNameBytes, 0, fileNameBytes.Length);
                            //await stream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);
                            // Send the image data
                            await client.GetStream().WriteAsync(imageData, 0, imageData.Length);

                            // Wait for a response from the server if needed
                            byte[] responseBuffer = new byte[2048];
                            int bytesRead = await client.GetStream().ReadAsync(responseBuffer, 0, responseBuffer.Length);
                            return Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
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
            if ((Index + 1) * NumOPage <= products.Count())
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

        private void DelFromOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var dataitem = button.DataContext;

            var itemID = (dataitem as OrderModel)?.Product.ProductId;
            if (listOrder != null)
                foreach (var item in listOrder)
                {
                    if (item.Product.ProductId == itemID)
                    {
                        listOrder.Remove(item);
                        lstOrderView.ItemsSource = null;
                        lstOrderView.ItemsSource = listOrder;
                        TotalTxtBlock.Text = getTotal().ToString() + " $";
                        TotalTxtBlock.VerticalAlignment = VerticalAlignment.Center;
                        return;
                    }
                }

        }
        private decimal? getTotal()
        {
            decimal? total = 0;
            try
            {
                foreach (var item in listOrder)
                {
                    total = total + item.Product.Price * item.quantity;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return total;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ClearListOrderCurrent();
        }
        private void ClearListOrderCurrent()
        {
            listOrder.Clear();
            lstOrderView.ItemsSource = null;
            lstOrderView.ItemsSource = listOrder;
            TotalTxtBlock.Text = getTotal().ToString() + " $";
            TotalTxtBlock.VerticalAlignment = VerticalAlignment.Center;
        }

        private async void CheckOutBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<BillDetail> BillDetailHolder = new List<BillDetail>();
                var total = getTotal();
                foreach (var item in listOrder)
                {
                    BillDetailHolder.Add(new BillDetail
                    {
                        ProductId = item.Product.ProductId,
                        Quantity = item.quantity,
                        TotalPrice = item.Product.Price * item.quantity,
                    });
                }
                Bill hold = new Bill
                {
                    BillDate = DateTime.Now,
                    BillDetails = BillDetailHolder,
                    TotalWorth = total,
                    Employee = null,
                    PaymentMethodId = (CheckOutCbb.Text.ToLower().Equals("cash")) ? 1 : 2
                };
                Connector();
                string result = await SendRequest("save_Bill/**/" + JsonConvert.SerializeObject(hold));

                ClearListOrderCurrent();
                MessageBox.Show(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
            }
        }
        public async Task<List<Category>> GetEditScreenData()
        {
            try
            {
                string hold = await SendRequest("get_all_cate/**/");
                var end = JsonConvert.DeserializeObject<List<Category>>(hold);
                if (end != null)
                {
                    return end;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + " Internal Error!!");
            }
            return null;
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Login_Popup_Click(object sender, RoutedEventArgs e)
        {
            if (LoginPopup.Visibility == Visibility.Visible)
            {
                LoginPopup.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoginPopup.Visibility = Visibility.Visible;
            }
        }
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (LoginPopup.Visibility == Visibility.Visible)
            {
                LoginPopup.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoginPopup.Visibility = Visibility.Visible;
            }
        }

        private void DownLoadImage_Click(object sender, RoutedEventArgs e)
        {

        }
        private async void LoadEditProductData()
        {
            ProductListData.ItemsSource = null;
            ProductCategoryCbb.ItemsSource = null;
            searchString = null;
            Connector();
            loadData();
            Connector();
            ProductCategoryCbb.ItemsSource = await GetEditScreenData();
            ProductCategoryCbb.SelectedIndex = 0;
            foreach (var item in products)
            {
                if (string.IsNullOrEmpty(item.ProductImg))
                {
                    item.ProductImg = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images", "product_PlaceHolder.png");
                }
                else
                {
                    item.ProductImg = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images", item.ProductImg);
                }
            }
            ProductListData.ItemsSource = products;
        }
        private async void EditProductList_Click(object sender, RoutedEventArgs e)
        {
            if (Edit_Pop_Up.Visibility == Visibility.Visible)
            {
                Connector();
                loadData();
                ProductListData.ItemsSource = null;
                Edit_Pop_Up.Visibility = Visibility.Collapsed;
                ProductCategoryCbb.ItemsSource = null;
            }
            else
            {

                ProductCategoryCbb.ItemsSource = null;
                searchString = null;
                Connector();
                loadData();
                Connector();
                ProductCategoryCbb.ItemsSource = await GetEditScreenData();
                ProductCategoryCbb.SelectedIndex = 0;
                foreach (var item in products)
                {
                    if (string.IsNullOrEmpty(item.ProductImg))
                    {
                        item.ProductImg = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images", "product_PlaceHolder.png");
                    }
                    else
                    {
                        item.ProductImg = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images", item.ProductImg);
                    }
                }
                ProductListData.ItemsSource = products;
                Edit_Pop_Up.Visibility = Visibility.Visible;
            }
        }

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            //if(string.IsNullOrEmpty(FilePathTxt.Text))
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    FilePathTxt.Text = openFileDialog.FileName;
                }
            }
        }
        private void saveButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FilePathTxt.Text))
            {
                string selectedFilePath = FilePathTxt.Text;

                string targetFolder = "Images";

                // prototype //string destinationPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), targetFolder, System.IO.Path.GetFileName(selectedFilePath));
                string destinationPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), targetFolder, ProductIDTxt.Text + System.IO.Path.GetExtension(selectedFilePath));

                try
                {
                    File.Copy(selectedFilePath, destinationPath, true);

                    MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a file before saving.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string? FileUploadLocal(int productID)
        {
            string destinationPath = null;
            if (!string.IsNullOrEmpty(FilePathTxt.Text))
            {
                string selectedFilePath = FilePathTxt.Text;

                string targetFolder = "Images";

                // prototype
                destinationPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), targetFolder, System.IO.Path.GetFileName(selectedFilePath));
                // destinationPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), targetFolder, productID.ToString() + System.IO.Path.GetExtension(selectedFilePath));

                try
                {
                    File.Copy(selectedFilePath, destinationPath, true);

                    MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a file before saving.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return destinationPath;
        }

        private void Product_Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            ProductIDTxt.Clear();
            ProductNameTxt.Clear();
            ProductPriceTxt.Clear();
            ProductQuantityTxt.Clear();
            ProductArriveDate.Text = string.Empty;
            ProductExpirationDate.Text = string.Empty;
            ProductCategoryCbb.SelectedIndex = 0;
            FilePathTxt.Clear();
        }

        private async void AddProductBtn_Click(object sender, RoutedEventArgs e)
        {
            Product hold = new Product
            {
                ProductName = ProductNameTxt.Text,
                Price = Convert.ToDecimal(ProductPriceTxt.Text),
                ProductImg = !string.IsNullOrEmpty(FilePathTxt.Text) && !System.IO.Path.GetFileName(FilePathTxt.Text).Equals("product_PlaceHolder.png") ? System.IO.Path.GetFileName(FilePathTxt.Text) : null,
                WarehouseArrivalDate = ProductArriveDate.SelectedDate,
                ExpirationDate = ProductExpirationDate.SelectedDate,
                CategoryId = Convert.ToInt32(ProductCategoryCbb.SelectedValue),
                Quantity = Convert.ToInt32(ProductQuantityTxt.Text)
            };
            Connector();
            var intBack = Convert.ToInt32(await SendRequest("add_product/**/" + JsonConvert.SerializeObject(hold)));
            if (intBack != null)
            {
                if (FilePathTxt.Text != null)
                {
                    Connector();
                    //SendRequest("ImageMode/**/"+ FilePathTxt.Text);
                    SendRequest("ImageMode/**/" + FileUploadLocal(intBack));
                }
            }
            LoadEditProductData();
        }

        private async void UpdateProductBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Product hold = new Product
                {
                    ProductId = Convert.ToInt32(ProductIDTxt.Text),
                    ProductName = ProductNameTxt.Text,
                    Price = Convert.ToDecimal(ProductPriceTxt.Text),
                    ProductImg = !string.IsNullOrEmpty(FilePathTxt.Text) && !System.IO.Path.GetFileName(FilePathTxt.Text).Equals("product_PlaceHolder.png") ? System.IO.Path.GetFileName(FilePathTxt.Text) : null,
                    WarehouseArrivalDate = ProductArriveDate.SelectedDate,
                    ExpirationDate = ProductExpirationDate.SelectedDate,
                    CategoryId = Convert.ToInt32(ProductCategoryCbb.SelectedValue),
                    Quantity = Convert.ToInt32(ProductQuantityTxt.Text)
                };
                Connector();
                var end = JsonConvert.DeserializeObject<Product>(await SendRequest("update_product/**/" + JsonConvert.SerializeObject(hold)));

                if (FilePathTxt.Text != null)
                {
                    if (!System.IO.Path.GetFileName(FilePathTxt.Text).Equals(end.ProductImg))
                    {
                        Connector();
                        //SendRequest("ImageMode/**/"+ FilePathTxt.Text);
                        SendRequest("ImageMode/**/" + FileUploadLocal(0));
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Please select a product!");
            }
            LoadEditProductData();
        }

        private async void DeleteProductBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Connector();
                int result = Convert.ToInt32(await SendRequest("delete_product/**/" + JsonConvert.SerializeObject(Convert.ToInt32(ProductIDTxt.Text))));
                MessageBox.Show("Delete Product number " + result + " Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select a Product!");
            }
            LoadEditProductData();
        }
    }
}
