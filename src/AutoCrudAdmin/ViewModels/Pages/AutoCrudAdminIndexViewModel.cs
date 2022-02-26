namespace AutoCrudAdmin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AutoCrudAdminIndexViewModel
    {
        public Func<IHtmlHelper<AutoCrudAdminIndexViewModel>, IHtmlContent> GenerateGrid { get; set; }

        public IEnumerable<AutoCrudAdminGridToolbarActionViewModel> ToolbarActions { get; set; }
    }
}