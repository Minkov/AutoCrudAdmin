namespace AutoCrudAdmin.ViewModels;

using System;

public class CheckboxFormControlViewModel : FormControlViewModel
{
    public bool IsSelected { get; set; }

    public override Type Type { get; set; } = typeof(bool);
}