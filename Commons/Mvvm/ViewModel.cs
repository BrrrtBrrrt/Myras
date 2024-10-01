using Commons.Datatypes;
using Commons.Mvvm;
using System.Windows.Input;

namespace Commons.ViewModels
{
    public abstract class ViewModel : ClassNotifyPropertyChanged
    {
        #region Properties

        protected IDictionary<string, IViewModelCommand> Commands { get; }

        #endregion

        #region Constructors

        public ViewModel()
        {
            Commands = new Dictionary<string, IViewModelCommand>();
        }

        #endregion

        #region Public functions

        protected ICommand HandleCommandGetter(IViewModelCommand command)
        {
            if (!Commands.ContainsKey(command.Id))
                Commands.Add(command.Id, command);
            return Commands[command.Id];
        }

        protected void HandleCommandSetter(IViewModelCommand command)
        {
            Commands.Add(command.Id, command);
        }

        #endregion
    }
}
