namespace Commons.Mvvm
{
    public interface IWindowView : IView
    {
        public string Title { get; set; }
        public bool? DialogResult { get; set; }
        public void Show();
        public bool? ShowDialog();
    }
}
