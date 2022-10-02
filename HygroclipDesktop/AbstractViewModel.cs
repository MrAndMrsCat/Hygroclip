using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HygroclipDesktop
{
    internal class AbstractViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void SetValue<T>(ref T property, T value, [CallerMemberName] string? propertyName = null)
        {
            if (property != null)
            {
                if (property.Equals(value)) return;
            }

            property = value;
            OnPropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
