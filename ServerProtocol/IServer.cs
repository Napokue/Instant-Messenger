namespace ServerProtocol
{
    public interface IServer : IConnection
    {
        /// <summary>
        /// Handles incoming and outcoming socket messages
        /// </summary>
        /// <param name="obj"></param>
        void ClientMessages(object obj);

        /// <summary>
        /// Method to setup a connection
        /// </summary>
        void SetupConnection();
    }
}
