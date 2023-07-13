namespace AutoCrudAdmin.Models;

using AutoCrudAdmin.ViewModels;
using System.Collections.Generic;

public class AdminActionContext
{
    public EntityAction Action { get; set; }

    public IDictionary<string, string> EntityDict { get; set; } = default!;

    public FormFilesContainer Files { get; set; } = default!;
}