using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ServerProtocol;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Client client;
        public MainWindow()
        {
            InitializeComponent();
            client = new Client(this);

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

        /* C# delegates are similar to pointers to functions, in C or C++.
         * A delegate is a reference type variable that holds the reference to a method.
         * The reference can be changed at runtime. Delegates are especially 
         * used for implementing events and the call-back methods.
         */
        private delegate void MessageCallBack(string data);


        // I have to do some research about invoking.
        // Invoke = aanroepen.
        private void AddMessageToChatWindow(string data)
        {
            var msg = Message.CreateMessage(data, new SolidColorBrush(Colors.Black));
            chatWindow.Items.Add(msg);
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!client.SetupConnection()) return;

            Thread t = new Thread(ClientMessages);
            t.SetApartmentState(ApartmentState.STA);

            t.Start();
        }

        private void ClientMessages()
        {
            try
            {
                while (ServerInformation.IsSocketConnected(client.clientSocket))
                {
                    var data = client.ReturnReceivedMessage(client.clientSocket);

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
                client.CloseConnection();
            }
        }

        private void txtMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtMessage.Text.Trim().Length > 0)
            {
                btnSendMessage.IsEnabled = true;
            }
            else
            {
                btnSendMessage.IsEnabled = false;
            }
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
            client.CloseConnection();
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            client.SendMessage(client.clientSocket, Encoding.ASCII.GetBytes(txtMessage.Text.Trim()));
            txtMessage.Text = "";
        }
    }
}
