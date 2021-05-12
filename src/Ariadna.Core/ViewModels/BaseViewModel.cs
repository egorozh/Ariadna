using System;
using System.ComponentModel;

namespace Ariadna.Core
{
    /// <summary>
    /// Имплементация <see cref="INotifyPropertyChanged"/> 
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public event EventHandler<OldAndNewValuesPropertyChangedEventArgs> ExPropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            PropertyChanged?.Invoke(this, new OldAndNewValuesPropertyChangedEventArgs(propertyName, before, after));
            ExPropertyChanged?.Invoke(this, new OldAndNewValuesPropertyChangedEventArgs(propertyName, before, after));
        }
    }
}