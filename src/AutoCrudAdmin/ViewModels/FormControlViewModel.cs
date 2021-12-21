namespace AutoCrudAdmin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FormControlViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormControlViewModel"/> class.
        /// </summary>
        public FormControlViewModel()
            => this.Options = Enumerable.Empty<object>();

        public string Name { get; set; }

        public Type Type { get; set; }

        public bool IsReadOnly { get; set; }

        public object? Value { get; set; }

        public IEnumerable<object> Options { get; set; }

        public bool IsComplex { get; set; }
    }
}