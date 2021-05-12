using System.ComponentModel;

namespace Ariadna.Core
{
    public class OldAndNewValuesPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public object OldValue { get; }
        public object NewValue { get; }

        public OldAndNewValuesPropertyChangedEventArgs(string propertyName, object oldValue, object newValue) : base(
            propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}