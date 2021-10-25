using System;
using System.ComponentModel;

namespace Ariadna.Core;

public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler<ExPropertyChangedEventArgs>? ExPropertyChanged;

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        PropertyChanged?.Invoke(this, new ExPropertyChangedEventArgs(propertyName, before, after));
        ExPropertyChanged?.Invoke(this, new ExPropertyChangedEventArgs(propertyName, before, after));
    }
}