namespace AutoCrudAdmin.ViewModels
{
    using System;

    public class FormControlViewModel
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public bool IsReadOnly { get; set; }

        public object Value { get; set; }

        public bool IsComplex { get; set; }
    }
}