namespace AutoCrudAdmin.ViewModels;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a ViewModel for a toolbar action in the AutoCrudAdmin's grid.
/// This ViewModel encapsulates the data necessary for rendering an action on the toolbar, including route values and form controls.
/// </summary>
public class AutoCrudAdminGridToolbarActionViewModel : GridAction
{
    /// <summary>
    /// Gets or sets the route values for the toolbar action.
    /// The route values represent additional parameters that will be passed in the URL when the action is invoked.
    /// </summary>
    public IDictionary<string, string> RouteValues { get; set; }
        = new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets the form controls for the toolbar action.
    /// These controls represent the various inputs that will be displayed in the form associated with the action.
    /// </summary>
    public IEnumerable<FormControlViewModel> FormControls { get; set; }
        = Enumerable.Empty<FormControlViewModel>();
}