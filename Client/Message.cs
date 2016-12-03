using System.Windows.Controls;
using System.Windows.Media;

namespace ClientApplication
{
    public static class Message
    {
        /// <summary>
        /// Create a ListViewItem with text and a color
        /// </summary>
        /// <param name="text">The text that the message contains</param>
        /// <param name="textColor">The text color</param>
        /// <returns></returns>
        public static ListViewItem CreateMessage(string text, SolidColorBrush textColor)
        {
            return new ListViewItem
            {
                Content = text,
                Foreground = textColor
            };
        }
    }
}
