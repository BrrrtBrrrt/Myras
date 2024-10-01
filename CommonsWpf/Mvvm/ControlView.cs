using Commons.Mvvm;
using System.Windows;
using System.Windows.Controls;

namespace CommonsWpf.Mvvm
{
    public class ControlView : Control, IControlView
    {
        public MessageBoxResultType MessageBoxShow(string text)
        {
            return MessageBoxShow(text, string.Empty, MessageBoxButtonType.OK);
        }

        public MessageBoxResultType MessageBoxShow(string text, string caption)
        {
            return MessageBoxShow(text, text, MessageBoxButtonType.OK);
        }

        public MessageBoxResultType MessageBoxShow(string text, string caption, MessageBoxButtonType messageBoxButton)
        {
            MessageBoxButton messageBoxButtonTypeWpf = messageBoxButton switch
            {
                MessageBoxButtonType.OK => MessageBoxButton.OK,
                MessageBoxButtonType.OKCancel => MessageBoxButton.OKCancel,
                MessageBoxButtonType.YesNoCancel => MessageBoxButton.YesNoCancel,
                MessageBoxButtonType.YesNo => MessageBoxButton.YesNo,
                _ => MessageBoxButton.OK,
            };
            MessageBoxResult result = MessageBox.Show(text, caption, messageBoxButtonTypeWpf);
            return result switch
            {
                MessageBoxResult.None => MessageBoxResultType.None,
                MessageBoxResult.OK => MessageBoxResultType.OK,
                MessageBoxResult.Cancel => MessageBoxResultType.Cancel,
                MessageBoxResult.Yes => MessageBoxResultType.Yes,
                MessageBoxResult.No => MessageBoxResultType.No,
                _ => MessageBoxResultType.None,
            };
        }
    }
}
