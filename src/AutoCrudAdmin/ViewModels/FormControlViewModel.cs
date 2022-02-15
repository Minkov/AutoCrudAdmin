namespace AutoCrudAdmin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoCrudAdmin.Enumerations;

    public class FormControlViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormControlViewModel"/> class.
        /// </summary>
        public FormControlViewModel()
            => this.Options = Enumerable.Empty<object>();

        public string Name { get; set; }

        public virtual Type Type { get; set; } = typeof(object);

        public bool IsReadOnly { get; set; }

        public bool IsHidden { get; set; }

        public object? Value { get; set; }

        public IEnumerable<object> Options { get; set; }

        public bool IsDbSet { get; set; }

        public FormControlType FormControlType { get; set; }
    }
}