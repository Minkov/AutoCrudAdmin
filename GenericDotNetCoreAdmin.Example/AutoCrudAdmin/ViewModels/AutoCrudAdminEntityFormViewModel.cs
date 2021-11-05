namespace AutoCrudAdmin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using AutoCrudAdmin.Controllers;

    public class AutoCrudAdminEntityFormViewModel
    {
        public IEnumerable<FormControlViewModel> FormControls { get; set; }

        public EntityAction Action { get; set; }
    }

    public class FormControlViewModel
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public bool IsReadOnly { get; set; }
        public object Value { get; set; }
        
        public bool IsComplex { get; set; }
    }
}