using Commons.Mvvm;

namespace Commons.ViewModels
{
    public abstract class ViewModelCommand<T> : IViewModelCommand<T>
        where T : ViewModel
    {
        #region Properties

        public T ViewModel { get; set; }
        public string Id { get; set; }

        #endregion

        #region Constructors

        protected ViewModelCommand(T viewModel, string id)
        {
            ViewModel = viewModel;
            Id = id;
        }

        #endregion

        #region Public functions

        public abstract bool CanExecute(object? parameter);

        public abstract void Execute(object? parameter);

        #endregion

        #region Events

        public event EventHandler? CanExecuteChanged;

        #endregion
    }
}
