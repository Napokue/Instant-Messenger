namespace ServerApplication
{
    static class Program
    {
        private static void Main(string[] args)
        {
            var server = new Server();
            server.StartServer();
        }
    }
}
