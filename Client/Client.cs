using System;
using System.Net.Sockets;
using System.Text;
using ServerProtocol;

namespace Client
{
    class Client : IClient
    {
        public Socket clientSocket { get; private set; }
        public static bool IsClientLoggedIn;
        private readonly MainWindow _mainScreen;

        public Client(MainWindow mainScreen)
        {
            _mainScreen = mainScreen;
        }

        public bool SetupConnection()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(ServerInformation.ServerEndPoint);

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
            if (clientSocket != null && clientSocket.Connected)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();

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
