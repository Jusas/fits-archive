using System.ComponentModel;

namespace FitsArchiveUI.Utils
{
    /// <summary>
    /// Typed implementation of PropertyChangedEventArgs that also provides the new and the old value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyChangedExtendedEventArgs<T> : PropertyChangedEventArgs
    {
        public virtual T OldValue { get; private set; }
        public virtual T NewValue { get; private set; }

        public PropertyChangedExtendedEventArgs(string propertyName, T oldValue, T newValue)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}