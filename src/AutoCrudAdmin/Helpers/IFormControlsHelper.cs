namespace AutoCrudAdmin.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using AutoCrudAdmin.ViewModels;

    public interface IFormControlsHelper
    {
        IEnumerable<FormControlViewModel> GenerateFormControls<TEntity>(
            TEntity entity,
            EntityAction entityAction,
            IDictionary<string, Expression<Func<object, bool>>> complexOptionFilters = null);

        string GetComplexFormControlNameForEntityName(string entityName);
    }
}