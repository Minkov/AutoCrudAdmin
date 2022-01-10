namespace AutoCrudAdmin.ViewModels;

using System;

public class CheckboxFormControlViewModel : FormControlViewModel
{
    public bool IsChecked { get; set; }

    public override Type Type { get; set; } = typeof(bool);
}