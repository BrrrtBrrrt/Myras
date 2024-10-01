using Commons.ViewModels;
using System.Windows.Input;

namespace Commons.Mvvm
{
    public interface IViewModelCommand : ICommand
    {
        #region Properties

        protected internal string Id { get; set; }

        #endregion
    }

    public interface IViewModelCommand<T> : IViewModelCommand
        where T : ViewModel
    {
        #region Properties

        protected T ViewModel { get; set; }

        #endregion
    }
}
