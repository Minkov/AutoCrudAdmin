namespace AutoCrudAdmin.ViewModels.Pages;

using System.Collections.Generic;

/// <summary>
/// Represents a ViewModel for the AutoCrudAdmin entity form.
/// This ViewModel encapsulates the data necessary for rendering the form for a specific entity.
/// </summary>
public class AutoCrudAdminEntityFormViewModel
{
    /// <summary>
    /// Gets or sets the form controls for the entity.
    /// These form controls represent the various data fields for the entity.
    /// </summary>
    public IEnumerable<FormControlViewModel>? FormControls { get; set; }

    /// <summary>
    /// Gets or sets the action to be performed on the entity.
    /// The action is typically an enumeration representing actions like 'Create', 'Update', 'Delete', etc.
    /// </summary>
    public EntityAction? Action { get; set; }

    /// <summary>
    /// Gets or sets the custom name for the action.
    /// This is useful for situations where you want to customize the display text for the action button.
    /// </summary>
    public string? CustomActionName { get; set; }
}