namespace AutoCrudAdmin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Gets the administration's index view model.
    /// </summary>
    public class AutoCrudAdminIndexViewModel
    {
        /// <summary>
        /// Gets or sets the administration's grid.
        /// </summary>
        public Func<IHtmlHelper<AutoCrudAdminIndexViewModel>, IHtmlContent> GenerateGrid { get; set; } = null!;

        /// <summary>
        /// Gets or sets the toolbar actions of the administration.
        /// </summary>
        public IEnumerable<AutoCrudAdminGridToolbarActionViewModel> ToolbarActions { get; set; } = null!;
    }
}