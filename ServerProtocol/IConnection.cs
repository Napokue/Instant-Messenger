using System.Net.Sockets;

namespace ServerProtocol
{
    public interface IConnection
    {
        /// <summary>
        /// Method to close a connection
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// Method to send a message to or from the server  
        /// </summary>
        /// <param name="socket">The socket you want to send your message with</param>
        /// <param name="message">The message you want to send</param>
        void SendMessage(Socket socket, byte[] message);

        /// <summary>
        /// Return the message in a string you received.
        /// </summary>
        /// <param name="socket">The socket you want to receive your message with</param>
        string ReturnReceivedMessage(Socket socket);
    }
}
