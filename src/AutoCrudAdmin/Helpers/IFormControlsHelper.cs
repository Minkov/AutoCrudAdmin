namespace AutoCrudAdmin.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using AutoCrudAdmin.ViewModels;

    public interface IFormControlsHelper
    {
        /// <summary>
        /// Generates the form controls for the respective entity.
        /// </summary>
        /// <param name="entity">The entity we want to generate the form controls for.</param>
        /// <param name="entityAction">Create, edit or delete form control action.</param>
        /// <param name="complexOptionFilters">Filters for generating complex form controls.</param>
        /// <param name="autocompleteType">The type of the autocomplete form control.</param>
        /// <typeparam name="TEntity">The entity we are generating the form controls for.</typeparam>
        /// <returns>The generated form controls.</returns>
        IEnumerable<FormControlViewModel> GenerateFormControls<TEntity>(
            TEntity entity,
            EntityAction entityAction,
            IDictionary<string, Expression<Func<object, bool>>> complexOptionFilters = null,
            Type autocompleteType = null);

        string GetComplexFormControlNameForEntityName(string entityName);
    }
}