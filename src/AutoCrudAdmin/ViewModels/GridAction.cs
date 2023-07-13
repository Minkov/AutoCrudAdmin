namespace AutoCrudAdmin.ViewModels;

/// <summary>
/// Represents an action that can be performed on a grid in the AutoCrudAdmin application.
/// </summary>
public class GridAction
{
    private string name = default!;

    /// <summary>
    /// Gets or sets the action to be performed.
    /// </summary>
    public string Action { get; set; } = default!;

    /// <summary>
    /// Gets or inits the name of the action. If not set, it defaults to the action.
    /// </summary>
    public string Name
    {
        get => this.name ??= this.Action;
        init => this.name = value;
    }
}