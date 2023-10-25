namespace AutoCrudAdmin;

/// <summary>
/// Holds various constant values used in the AutoCrudAdmin application.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Holds constant values related to entities in the AutoCrudAdmin application.
    /// </summary>
    public static class Entity
    {
        /// <summary>
        /// Constant string for the name of a single primary key.
        /// </summary>
        public const string SinglePrimaryKeyName = "PK";
    }

    /// <summary>
    /// Holds CSS class names used in the AutoCrudAdmin application.
    /// </summary>
    public static class CssClassNames
    {
        /// <summary>
        /// CSS class name for a form group in Bootstrap.
        /// </summary>
        public const string FormGroup = "form-group";

        /// <summary>
        /// CSS class name for a form control in Bootstrap.
        /// </summary>
        public const string FormControl = "form-control";

        /// <summary>
        /// CSS class name for a checkbox in a form in Bootstrap.
        /// </summary>
        public const string FormCheckbox = "form-check";

        /// <summary>
        /// CSS class name for a checkbox label in a form in Bootstrap.
        /// </summary>
        public const string FormCheckboxLabel = "form-check-label";

        /// <summary>
        /// CSS class name for a checkbox input in a form in Bootstrap.
        /// </summary>
        public const string FormCheckboxInput = "form-check-input";

        /// <summary>
        /// CSS class name for an inline checkbox in a form in Bootstrap.
        /// </summary>
        public const string FormCheckboxInline = "form-check-inline";

        /// <summary>
        /// CSS class name for a hidden element in Bootstrap.
        /// </summary>
        public const string Hide = "d-none";

        /// <summary>
        /// CSS class name for an expandable element.
        /// </summary>
        public const string ExpandableClassName = "expandable";
    }

    /// <summary>
    /// Holds keys for TempData used in the AutoCrudAdmin application.
    /// </summary>
    public static class TempDataKeys
    {
        /// <summary>
        /// Key for storing and retrieving success messages in TempData.
        /// </summary>
        public const string SuccessMessage = "SuccessMessage";

        /// <summary>
        /// Key for storing and retrieving danger messages in TempData.
        /// </summary>
        public const string DangerMessage = "DangerMessage";
    }

    /// <summary>
    /// Holds constants related to the grid component in the AutoCrudAdmin application.
    /// </summary>
    public static class Grid
    {
        /// <summary>
        /// Default maximum length for strings in a grid column.
        /// </summary>
        public const int DefaultColumnStringMaxLength = 40;
    }

    /// <summary>
    /// Holds constants related to HTML elements in the AutoCrudAdmin application.
    /// </summary>
    public static class Html
    {
        /// <summary>
        /// HTML for a line break.
        /// </summary>
        public const string NewLine = "<br />";
    }

    /// <summary>
    /// Holds constants related to partial views in the AutoCrudAdmin application.
    /// </summary>
    public static class PartialView
    {
        /// <summary>
        /// Partial view name for the entity form control.
        /// </summary>
        public const string EntityFormControlPartial = "_EntityFormControlPartial";

        /// <summary>
        /// Partial view name for the entity form controls.
        /// </summary>
        public const string EntityFormControlsPartial = "_EnityFormControlsPartial";
    }
}