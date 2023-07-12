namespace AutoCrudAdmin.ViewModels;
using System;

public class ExpandableMultiChoiceCheckBoxFormControlViewModel : CheckboxFormControlViewModel
{
    public FormControlViewModel? Expand { get; set; }

    public string ExpandedValuePrefix { get; set; } = Guid.NewGuid().ToString();
}