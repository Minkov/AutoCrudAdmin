namespace AutoCrudAdmin.ViewModels;

/// <summary>
/// Represents a ViewModel for a dropdown item in the AutoCrudAdmin application.
/// This ViewModel encapsulates the data necessary for rendering a dropdown item, including its display name and value.
/// </summary>
public class DropDownViewModel
{
    /// <summary>
    /// Gets or sets the display name of the dropdown item.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the value of the dropdown item.
    /// </summary>
    public object Value { get; set; } = default!;
}