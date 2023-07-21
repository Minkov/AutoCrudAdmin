namespace AutoCrudAdmin.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using AutoCrudAdmin.Enumerations;
using AutoCrudAdmin.Extensions;

/// <summary>
/// Represents a ViewModel for a form control in the AutoCrudAdmin application.
/// This ViewModel encapsulates the data necessary for rendering a form control, including its name, type, value, and other configurations.
/// </summary>
public class FormControlViewModel
{
    private string? displayName;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormControlViewModel"/> class.
    /// </summary>
    public FormControlViewModel()
        => this.Options = Enumerable.Empty<object>();

    /// <summary>
    /// Gets or sets the name of the form control.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the form control.
    /// If not set, it defaults to the name of the form control, with spaces inserted between camel case words.
    /// </summary>
    public string DisplayName
    {
        get => this.displayName
               ?? (this.Name.Any(char.IsWhiteSpace)
                   ? this.Name
                   : this.Name.ToSpaceSeparatedWords());
        set => this.displayName = value;
    }

    /// <summary>
    /// Gets or sets the type of the form control. This is typeof(object) by default.
    /// </summary>
    public virtual Type Type { get; set; } = typeof(object);

    /// <summary>
    /// Gets or sets a value indicating whether the form control is read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the form control is hidden.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets or sets the value of the form control.
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Gets or sets the options for the form control. This is used for controls like dropdowns and multi-choice checkboxes.
    /// </summary>
    public IEnumerable<object> Options { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the form control is a database set.
    /// </summary>
    public bool IsDbSet { get; set; }

    /// <summary>
    /// Gets or sets the type of the form control.
    /// </summary>
    public FormControlType FormControlType { get; set; }

    /// <summary>
    /// Gets or sets the controller for the autocomplete feature of the form control.
    /// </summary>
    public string? FormControlAutocompleteController { get; set; }

    /// <summary>
    /// Gets or sets the entity id for the autocomplete feature of the form control.
    /// </summary>
    public string? FormControlAutocompleteEntityId { get; set; }
}