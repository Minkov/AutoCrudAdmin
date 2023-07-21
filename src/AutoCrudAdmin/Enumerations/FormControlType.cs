namespace AutoCrudAdmin.Enumerations;

/// <summary>
/// The FormControlType enumeration defines the types of form controls that can be used AutoCrudAdmin.
/// </summary>
public enum FormControlType
{
    /// <summary>
    /// Represents an automatic control type, where the type of control is determined by the system.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Represents a multi-choice checkbox control, where the user can select multiple options.
    /// </summary>
    MultiChoiceCheckbox = 1,

    /// <summary>
    /// Represents a textarea control, where the user can enter multiple lines of text.
    /// </summary>
    TextArea = 2,

    /// <summary>
    /// Represents an expandable multi-choice checkbox control, where the user can select multiple options and the list of options can be expanded.
    /// </summary>
    ExpandableMultiChoiceCheckBox = 3,

    /// <summary>
    /// Represents an autocomplete control, where the user's input is completed automatically as they type.
    /// </summary>
    Autocomplete = 4,
}
