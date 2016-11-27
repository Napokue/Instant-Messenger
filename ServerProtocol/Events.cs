
namespace ServerProtocol
{
    public class Events
    {
        public delegate void dlgMessageNotify();

        public static event dlgMessageNotify MessageNotify;

        private static void OnMessageNotify()
        {
            var handler = MessageNotify;
            handler?.Invoke();
        }
    }
}
