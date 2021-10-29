namespace GenericDotNetCoreAdmin.Helpers.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using GenericDotNetCoreAdmin.Extensions;
    using GenericDotNetCoreAdmin.ViewModels;

    public class FormControlsHelper
        : IFormControlsHelper
    {
        public IEnumerable<FormControlViewModel> GenerateFormControls<TEntity>(TEntity entity)
        {
            var entityType = typeof(TEntity);
            var primaryKeyProperty = entityType.GetPrimaryKeyPropertyInfo();
            return entityType.GetProperties()
                .Select(property => new FormControlViewModel
                {
                    Name = property.Name,
                    Type = property.PropertyType,
                    Value = property.GetValue(entity),
                    IsReadOnly = property == primaryKeyProperty,
                });
        }
    }
}