using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerProtocol;

namespace ServerApplication
{
    class Server : IServer
    {
        private readonly Socket _serverSocket;
        private readonly List<Socket> _clientList;

        public Server()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientList = new List<Socket>();
        }

        /// <summary>
        /// Starting up the server
        /// </summary>
        public void StartServer()
        {
            try
            {
                SetupConnection();
                StartServerLoop();
            }
            catch (Exception)
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// The main server loop
        /// </summary>
        private void StartServerLoop()
        {
            while (true)
            {
                // Program is suspended while waiting for an incoming connection
                _serverSocket.Listen(10);
                var clientSocket = _serverSocket.Accept();
                Console.WriteLine($"Connected: {clientSocket.RemoteEndPoint}");
                BroadCast($"Connected: {clientSocket.RemoteEndPoint}");
                // Queue the method and assign it to the first available thread
                ThreadPool.QueueUserWorkItem(ClientMessages, clientSocket);
            }
        }
        
        public void ClientMessages(object obj)
        {
            var clientSocket = (Socket) obj;
            _clientList.Add(clientSocket);
            
            while (ServerInformation.IsSocketConnected(clientSocket))
            {
                var data = ReturnReceivedMessage(clientSocket);
                if (data.Length <= 0) continue;

                var message = $"{clientSocket.RemoteEndPoint}: {data}";
                Console.WriteLine(message); 
                BroadCast(message);
            }
            if (!ServerInformation.IsSocketConnected(clientSocket))
            {
                _clientList.Remove(clientSocket);
                Console.WriteLine($"Disconnected: {clientSocket.RemoteEndPoint}");
                BroadCast($"Disconnected: {clientSocket.RemoteEndPoint}");
            }
        }

        public void SetupConnection()
        {
            try
            {
                Console.WriteLine("The socket has been successfully bound to the server its EndPoint.");
                _serverSocket.Bind(ServerInformation.ServerEndPoint);
            }
            catch (Exception)
            {
                Console.WriteLine("The socket could not be bound to the server its EndPoint.");
            }
        }

        public void CloseConnection()
        {
            Console.WriteLine("The server could not be started.");
            _serverSocket.Close();
        }

        public void SendMessage(Socket socket, byte[] message)
        {
            socket.Send(message);
        }

        /// <summary>
        /// Sends a message from the server to all clients
        /// </summary>
        /// <param name="message"></param>
        private void BroadCast(string message)
        {
            foreach (var client in _clientList)
            {
                SendMessage(client, Encoding.ASCII.GetBytes(message));
            }
        }

        public string ReturnReceivedMessage(Socket socket)
        {
            try
            {
                // Data buffer for incoming data
                var dataBuffer = new byte[socket.ReceiveBufferSize];
                return Encoding.ASCII.GetString(dataBuffer, 0, socket.Receive(dataBuffer));
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
