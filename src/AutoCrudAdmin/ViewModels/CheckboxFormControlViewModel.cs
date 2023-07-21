namespace AutoCrudAdmin.ViewModels;

using System;

/// <summary>
/// Represents a ViewModel for a checkbox form control in the AutoCrudAdmin application.
/// This ViewModel encapsulates the data necessary for rendering a checkbox control, including its check state.
/// </summary>
public class CheckboxFormControlViewModel : FormControlViewModel
{
    /// <summary>
    /// Gets or sets a value indicating whether the checkbox is checked.
    /// </summary>
    public bool IsChecked { get; set; }

    /// <summary>
    /// Gets or sets the type of the form control.
    /// For a checkbox, this is always typeof(bool).
    /// </summary>
    public override Type Type { get; set; } = typeof(bool);
}