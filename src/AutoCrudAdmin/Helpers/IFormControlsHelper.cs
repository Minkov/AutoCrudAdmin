namespace AutoCrudAdmin.Helpers
{
    using System.Collections.Generic;
    using AutoCrudAdmin.ViewModels;

    public interface IFormControlsHelper
    {
        IEnumerable<FormControlViewModel> GenerateFormControls<TEntity>(TEntity entity);
    }
}