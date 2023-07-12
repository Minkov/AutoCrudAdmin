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
        /// <param name="entityAction">The action to be performed on the entity.</param>
        /// <param name="complexOptionFilters">Optional. A dictionary containing complex option filters, based on which we are loading the data for the form control.</param>
        /// <param name="autocompleteType">Optional. The type of the property that will be used for autocomplete functionality.
        /// When passed, values for this property will not be loaded immediately, but when searched for.</param>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An enumerable collection of FormControlViewModel objects.</returns>
        IEnumerable<FormControlViewModel> GenerateFormControls<TEntity>(
            TEntity entity,
            EntityAction entityAction,
            IDictionary<string, Expression<Func<object, bool>>>? complexOptionFilters = null,
            Type? autocompleteType = null);

        string GetComplexFormControlNameForEntityName(string entityName);
    }
}