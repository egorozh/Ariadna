using System.ComponentModel;

namespace Ariadna.Core;

public class ExPropertyChangedEventArgs : PropertyChangedEventArgs
{
    public object? OldValue { get; }
    public object? NewValue { get; }

    public ExPropertyChangedEventArgs(string propertyName, object? oldValue, object? newValue)
        : base(propertyName)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}