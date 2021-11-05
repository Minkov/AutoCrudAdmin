namespace AutoCrudAdmin.ViewModels
{
    using System.Collections.Generic;

    public class AutoCrudAdminEntityFormViewModel
    {
        public IEnumerable<FormControlViewModel> FormControls { get; set; }

        public EntityAction Action { get; set; }
    }
}