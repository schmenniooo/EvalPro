using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EvalProUI.model;

/// <summary>
/// Base class for all ViewModels. Provides INotifyPropertyChanged support.
/// </summary>
public class BaseViewModel : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Raises the <see cref="PropertyChanged"/> event.</summary>
    /// <param name="propertyName">Name of the changed property (auto-filled by compiler).</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>Sets the backing field and raises <see cref="PropertyChanged"/> if the value changed.</summary>
    /// <returns><c>true</c> if the value was changed; otherwise <c>false</c>.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
