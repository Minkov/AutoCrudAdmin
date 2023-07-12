namespace AutoCrudAdmin;

using System.Collections.Generic;
using System.Linq;
using AutoCrudAdmin.Filters;

/// <summary>
/// The <see cref="AutoCrudAdminOptions"/> class provides various configuration options for the AutoCrudAdmin system.
/// </summary>
public class AutoCrudAdminOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoCrudAdminOptions"/> class.
    /// The Authorization and AsyncAuthorization properties with empty collections.
    /// </summary>
    public AutoCrudAdminOptions()
    {
        this.Authorization = Enumerable.Empty<IAutoCrudAuthFilter>();
        this.AsyncAuthorization = Enumerable.Empty<IAsyncAuthCrudFilter>();
    }

    /// <summary>
    /// Gets or sets the Authorization property, a collection of <see cref="IAutoCrudAuthFilter"/>s.
    /// These filters are used to perform synchronous authorization checks in the AutoCrudAdmin system.
    /// </summary>
    public IEnumerable<IAutoCrudAuthFilter> Authorization { get; set; }

    /// <summary>
    /// Gets or sets the Authorization property, a collection of <see cref="IAutoCrudAuthFilter"/>s.
    /// These filters are used to perform asynchronous authorization checks in the AutoCrudAdmin system.
    /// </summary>
    public IEnumerable<IAsyncAuthCrudFilter> AsyncAuthorization { get; set; }

    /// <summary>
    /// Gets or sets the LayoutName property, representing the layout name to be used in the AutoCrudAdmin system.
    /// </summary>
    public string LayoutName { get; set; }

    /// <summary>
    /// Gets or sets the ApplicationName property, representing the application name in the AutoCrudAdmin system.
    /// </summary>
    public string ApplicationName { get; set; }
}