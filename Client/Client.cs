using System;
using System.Net.Sockets;
using System.Text;
using ServerProtocol;

namespace ClientApplication
{
    class Client : IClient
    {
        public Socket ClientSocket { get; private set; }
        public static bool IsClientLoggedIn;
        private readonly MainWindow _mainScreen;

        public Client(MainWindow mainScreen)
        {
            _mainScreen = mainScreen;
        }

        public bool SetupConnection()
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                ClientSocket.Connect(ServerInformation.ServerEndPoint);

                // Sets the boolean to true
                IsClientLoggedIn = IsLoggedIn();
                _mainScreen.UpdateServerStatus();
                return true;
            }
            catch (Exception)
            {
                CloseConnection();
                return false;
            }
        }

        public void CloseConnection()
        {
            if (ClientSocket != null && ClientSocket.Connected)
            {
                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Close();

                // Sets the boolean to false
                IsClientLoggedIn = IsLoggedIn();
                _mainScreen.UpdateServerStatus();
            }
        }

        public void SendMessage(Socket socket, byte[] message)
        {
            socket.Send(message);
        }

        public string ReturnReceivedMessage(Socket socket)
        {
            // Data buffer for incoming data.
            var dataBuffer = new byte[1024];
            return Encoding.ASCII.GetString(dataBuffer, 0, socket.Receive(dataBuffer));
        }

        public bool IsLoggedIn()
        {
            return !IsClientLoggedIn;
        }
    }
}
