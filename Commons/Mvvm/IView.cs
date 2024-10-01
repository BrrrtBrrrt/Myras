namespace Commons.Mvvm
{
    public interface IView
    {
        public object DataContext { get; set; }

        public MessageBoxResultType MessageBoxShow(string text);
        public MessageBoxResultType MessageBoxShow(string text, string caption);
        public MessageBoxResultType MessageBoxShow(string text, string caption, MessageBoxButtonType messageBoxButton);
    }
}
