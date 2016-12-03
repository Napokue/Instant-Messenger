
namespace ServerProtocol
{
    public class Events
    {
        public delegate void DlgMessageNotify();

        public static event DlgMessageNotify MessageNotify;

        private static void OnMessageNotify()
        {
            var handler = MessageNotify;
            handler?.Invoke();
        }
    }
}
