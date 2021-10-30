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

    public abstract class FormControlViewModel
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public bool IsReadOnly { get; set; }
    }

    public class SimpleFormControlViewModel
        : FormControlViewModel
    {
        public object Value { get; set; }
    }

    public class ComplexFormControlViewModel
        : FormControlViewModel
    {
        public IEnumerable<object> Keys { get; set; }
        public IEnumerable<object> Values { get; set; }
    }
}