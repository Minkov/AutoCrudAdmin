namespace AutoCrudAdmin.ViewModels;

using System.Collections.Generic;
using System.Linq;

public class AutoCrudAdminGridToolbarActionViewModel : GridAction
{
    public IDictionary<string, string> RouteValues { get; set; }
        = new Dictionary<string, string>();

    public IEnumerable<FormControlViewModel> FormControls { get; set; }
        = Enumerable.Empty<FormControlViewModel>();
}