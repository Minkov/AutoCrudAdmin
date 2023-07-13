namespace AutoCrudAdmin.Models;

using AutoCrudAdmin.ViewModels;
using System.Collections.Generic;

/// <summary>
/// The `AdminActionContext` class represents the context of an action in the AutoCrudAdmin system.
/// It includes information about the action, the entity involved in the action, and any associated files.
/// </summary>
public class AdminActionContext
{
    /// <summary>
    /// Gets or sets the action being performed. The action is of type <see cref="EntityAction"/>,
    /// an enum representing the different possible actions in the AutoCrudAdmin system.
    /// </summary>
    public EntityAction Action { get; set; }

    /// <summary>
    /// Gets or sets a dictionary representing the entity involved in the action.
    /// The keys are the entity property names and the values are the corresponding property values.
    /// </summary>
    public IDictionary<string, string> EntityDict { get; set; } = default!;

    /// <summary>
    /// Gets or sets a `FormFilesContainer` object representing the files associated with the action.
    /// </summary>
    public FormFilesContainer Files { get; set; } = default!;
}