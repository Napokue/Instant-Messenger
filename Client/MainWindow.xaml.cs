using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ServerProtocol;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Client _client;
        public MainWindow()
        {
            InitializeComponent();
            _client = new Client(this);

            // Set the default server status on start
            UpdateServerStatus();
        }
        
        /// <summary>
        /// Update the server status label
        /// </summary>
        public void UpdateServerStatus()
        {
            lbServerStatus.Text = "Server status:";
            if (Client.IsClientLoggedIn)
            {
                lbServerStatus.Text += "Online";
                lbServerStatus.Foreground = new SolidColorBrush(Colors.Green);
                btnConnect.IsEnabled = false;
                btnDisconnect.IsEnabled = true;
                txtMessage.IsEnabled = true;
            }
            else
            {
                lbServerStatus.Text += "Offline";
                lbServerStatus.Foreground = new SolidColorBrush(Colors.Red);
                btnConnect.IsEnabled = true;
                btnDisconnect.IsEnabled = false;
                txtMessage.IsEnabled = false;
            }
        }

        private delegate void MessageCallBack(string data);


        private void AddMessageToChatWindow(string data)
        {
            var msg = Message.CreateMessage(data, new SolidColorBrush(Colors.Black));
            chatWindow.Items.Add(msg);
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!_client.SetupConnection()) return;

            Thread t = new Thread(ClientMessages);
            t.SetApartmentState(ApartmentState.STA);

            t.Start();
        }

        private void ClientMessages()
        {
            try
            {
                while (ServerInformation.IsSocketConnected(_client.ClientSocket))
                {
                    var data = _client.ReturnReceivedMessage(_client.ClientSocket);

                    if (data.Length > 0)
                    {
                        chatWindow.Dispatcher.Invoke(new MessageCallBack(AddMessageToChatWindow), data);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void OnFormClosing(object sender, CancelEventArgs e)
        {
            if (Client.IsClientLoggedIn)
            {
                _client.CloseConnection();
            }
        }

        private void txtMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnSendMessage.IsEnabled = txtMessage.Text.Trim().Length > 0;
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSendMessage_Click(sender, e);
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            _client.CloseConnection();
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            _client.SendMessage(_client.ClientSocket, Encoding.ASCII.GetBytes(txtMessage.Text.Trim()));
            txtMessage.Text = string.Empty;
        }
    }
}
