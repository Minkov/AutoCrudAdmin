namespace AutoCrudAdmin.Models;

using AutoCrudAdmin.ViewModels;
using System.Collections.Generic;

/// <summary>
/// A class for the administration's context.
/// </summary>
public class AdminActionContext
{
    /// <summary>
    /// Gets or sets the type of an entity's action.
    /// </summary>
    public EntityAction Action { get; set; }

    /// <summary>
    /// Gets or sets a dictionary with entities.
    /// </summary>
    public IDictionary<string, string> EntityDict { get; set; } = null!;

    /// <summary>
    /// Gets or sets the files for a form.
    /// </summary>
    public FormFilesContainer Files { get; set; } = null!;
}