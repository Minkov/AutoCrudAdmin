namespace AutoCrudAdmin.ViewModels;

using System.Collections.Generic;

public class AutoCrudAdminGridToolbarActionViewModel : GridAction
{
    public IDictionary<string, string> RouteValues { get; set; }
        = new Dictionary<string, string>();
}