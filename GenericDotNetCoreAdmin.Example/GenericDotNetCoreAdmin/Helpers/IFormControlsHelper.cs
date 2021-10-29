namespace GenericDotNetCoreAdmin.Helpers
{
    using System.Collections.Generic;
    using GenericDotNetCoreAdmin.ViewModels;

    public interface IFormControlsHelper
    {
        IEnumerable<FormControlViewModel> GenerateFormControls<TEntity>(TEntity entity);
    }
}