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
        public MainWindow()
        {
            InitializeComponent();
            int error_counter = 0;
            if (!ConnectToServer()&&error_counter<=3)
            {
                error_counter++;
                MessageBox.Show("Server is not connected! Retrying...");
            }
        }
        private bool ConnectToServer()
        {         
            try
            {
                client = new TcpClient("127.0.0.1", 1500);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }
        private async void SendRequest(string request)
        {
            string[] holder = request.Split("/**/");
            string mode = holder[0];
            string data = holder[1];
            if (client==null||!client.Connected)
            {
                MessageBox.Show("Server is not connected...");
                return;
            }
            using (NetworkStream stream = client.GetStream())
            {
                byte[] bytesRead = Encoding.UTF8.GetBytes(request);
                await stream.WriteAsync(bytesRead, 0, bytesRead.Length);

                switch (mode)
                {
                    case "get_all":
                        {
                            return;
                        }
                    case "save_Bill":
                        {

                            return;
                        }
                    case "update_product":
                        {
                            return;
                        }
                    default:
                        {
                            return;
                        }
                }
            }
        }
    }
}
