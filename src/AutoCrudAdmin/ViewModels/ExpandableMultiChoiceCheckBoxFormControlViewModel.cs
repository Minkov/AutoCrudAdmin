namespace AutoCrudAdmin.ViewModels;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a ViewModel for an expandable multi-choice checkbox form control in the AutoCrudAdmin application.
/// This ViewModel encapsulates the data necessary for rendering an expandable multi-choice checkbox form control.
/// </summary>
public class ExpandableMultiChoiceCheckBoxFormControlViewModel : CheckboxFormControlViewModel
{
    /// <summary>
    /// Gets or sets the list of form controls to be expanded when this checkbox is checked.
    /// </summary>
    public List<FormControlViewModel>? Expand { get; set; }

    /// <summary>
    /// Gets or sets the prefix for the expanded value. This is set to a new GUID by default.
    /// </summary>
    public string ExpandedValuePrefix { get; set; } = Guid.NewGuid().ToString();
}