namespace AutoCrudAdmin.ViewModels
{
    using System.Collections.Generic;

    public class AutoCrudAdminEntityFormViewModel
    {
        public IEnumerable<FormControlViewModel> FormControls { get; set; } = default!;

        public EntityAction Action { get; set; }

        public string? CustomActionName { get; set; }
    }
}