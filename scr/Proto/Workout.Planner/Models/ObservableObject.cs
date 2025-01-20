namespace Workout.Planner.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Checks if the new value is different from the field's value. If it is, triggers the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="field">Field.</param>
        /// <param name="value">New property value.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Returns <see langword="false"/> if field equals value, otherwise <see langword="true"/>.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Triggers the PropertyChanged event.Create new EventArgs with passed property name.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        protected void RaisePropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
