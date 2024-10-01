using CommonsWpf.Mvvm;
using MnistLib.View;

namespace Mnist
{
    /// <summary>
    /// Interaction logic for WindowMnist.xaml
    /// </summary>
    public partial class WindowMnist : WindowView, IMnist
    {
        public WindowMnist()
        {
            InitializeComponent();
        }
    }
}
