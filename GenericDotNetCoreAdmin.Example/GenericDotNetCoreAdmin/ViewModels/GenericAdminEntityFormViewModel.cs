namespace GenericDotNetCoreAdmin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using GenericDotNetCoreAdmin.Controllers;

    public class GenericAdminEntityFormViewModel
    {
        public IEnumerable<FormControlViewModel> FormControls { get; set; }

        public EntityAction Action { get; set; }
    }

    public class FormControlViewModel
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public object Value { get; set; }

        public bool IsReadOnly { get; set; }
    }
}