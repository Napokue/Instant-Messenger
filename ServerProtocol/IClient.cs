namespace ServerProtocol
{
    public interface IClient : IConnection
    {
        /// <summary>
        /// Method to check if the client is logged in or not
        /// </summary>
        /// <returns>Returns the status of the client.</returns>
        bool IsLoggedIn();

        /// <summary>
        /// Method to setup a connection
        /// </summary>
        bool SetupConnection();
    }
}
