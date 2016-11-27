using System.Net;
using System.Net.Sockets;

namespace ServerProtocol
{
    public abstract class ServerInformation
    {
        /// <summary>
        /// The IP address of the server
        /// </summary>
        public static IPAddress ServerIpAddress = IPAddress.Parse("127.0.0.1");

        /// <summary>
        /// The port of the server
        /// </summary>
        public static int serverPort = 5000;

        /// <summary>
        /// EndPoint of the server (IP + port)
        /// </summary>
        public static IPEndPoint ServerEndPoint = new IPEndPoint(ServerIpAddress, serverPort);

        /// <summary>
        /// Check if the socket is still connected to the server
        /// </summary>
        /// <param name="socket">The socket you want to check the connection with</param>
        /// <returns>Returns true if there is a connection and false if there is no connection</returns>
        public static bool IsSocketConnected(Socket socket)
        {
            // true if Listen has been called and a connection is pending
            // true if data is available for reading
            // true if connection has been closed, reset or termininated
            var socketResponse = socket.Poll(1000, SelectMode.SelectRead);

            // true if socket receives no data
            var socketReceive = (socket.Available == 0); // Check if the clientSocket is receiving data.
            return !socketResponse || !socketReceive;
        }

    }
}
