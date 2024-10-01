using System.ComponentModel;

namespace Commons.Datatypes
{
    public abstract class ClassNotifyPropertyChanged : INotifyPropertyChanged
    {
        public void PropertyHasChanged(object? sender, string propertyName)
        {
            PropertyChanged?.Invoke(sender, new(propertyName));
        }

        public void PropertyHasChangedSetterHandler<T>(object sender, ref T currentValue, T newValue, string propertyName, Action? preChangeNotificationCallback = null, Action? postChangeNotificationCallback = null)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;
            currentValue = newValue;
            preChangeNotificationCallback?.Invoke();
            PropertyChanged?.Invoke(sender, new(propertyName));
            postChangeNotificationCallback?.Invoke();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
