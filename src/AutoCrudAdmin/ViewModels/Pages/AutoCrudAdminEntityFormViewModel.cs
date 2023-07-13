namespace AutoCrudAdmin.ViewModels.Pages;

using System.Collections.Generic;

    public class AutoCrudAdminEntityFormViewModel
    {
        public IEnumerable<FormControlViewModel>? FormControls { get; set; }

        public EntityAction? Action { get; set; }

    public string? CustomActionName { get; set; }
}