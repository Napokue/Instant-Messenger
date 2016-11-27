using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerProtocol;

namespace Server
{
    class Server : IServer
    {
        private readonly Socket serverSocket;
        private readonly List<Socket> clientList;

        public Server()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientList = new List<Socket>();
        }

        /// <summary>
        /// Starting up the server
        /// </summary>
        public void StartServer()
        {
            Console.WriteLine("Starting the server...");
            Console.WriteLine("Enter IP adress:");
            ServerInformation.ServerIpAddress = IPAddress.Parse(Console.ReadLine());
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
                serverSocket.Listen(10);
                var clientSocket = serverSocket.Accept();
                Console.WriteLine($"Connected: {clientSocket.RemoteEndPoint}");

                // Queue the method and assign it to the first available thread
                ThreadPool.QueueUserWorkItem(ClientMessages, clientSocket);
            }
        }
        
        public void ClientMessages(object obj)
        {
            var clientSocket = (Socket) obj;
            clientList.Add(clientSocket);
            
            while (ServerInformation.IsSocketConnected(clientSocket))
            {
                var data = ReturnReceivedMessage(clientSocket);

                if (data.Length <= 0) continue;

                // Output the sent message from the client
                Console.WriteLine($"{clientSocket.RemoteEndPoint}: {data}"); 

                // "Forward" the sent message from client to the other clients
                foreach (var client in clientList)
                {
                    SendMessage(client, Encoding.ASCII.GetBytes(clientSocket.RemoteEndPoint + ": " + data));
                }
            }
            if (!ServerInformation.IsSocketConnected(clientSocket))
            {
                Console.WriteLine($"Disconnected {clientSocket.RemoteEndPoint}");
                clientList.Remove(clientSocket);
            }
        }

        public void SetupConnection()
        {
            try
            {
                serverSocket.Bind(ServerInformation.ServerEndPoint);
                Console.WriteLine("The socket has been successfully bound to the server its EndPoint.");
            }
            catch (Exception)
            {
                Console.WriteLine("The socket could not be bound to the server its EndPoint.");
            }
        }

        public void CloseConnection()
        {
            Console.WriteLine("The server could not be started.");
            serverSocket.Close();
        }

        public void SendMessage(Socket socket, byte[] message)
        {
            socket.Send(message);
        }

        public string ReturnReceivedMessage(Socket socket)
        {
            try
            {
                // Data buffer for incoming data
                var dataBuffer = new byte[1024];
                return Encoding.ASCII.GetString(dataBuffer, 0, socket.Receive(dataBuffer));
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
