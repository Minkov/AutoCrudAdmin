namespace AutoCrudAdmin;

public static class Constants
{
    public static class Entity
    {
        public const string SinglePrimaryKeyName = "PK";
    }

    public static class CssClassNames
    {
        public const string FormGroup = "form-group";
        public const string FormControl = "form-control";
        public const string FormCheckbox = "form-check";
        public const string FormCheckboxLabel = "form-check-label";
        public const string FormCheckboxInput = "form-check-input";
        public const string FormCheckboxInline = "form-check-inline";
        public const string Hide = "d-none";
        public const string ExpandableClassName = "expandable";
    }

    public static class TempDataKeys
    {
        public const string SuccessMessage = "SuccessMessage";
        public const string DangerMessage = "DangerMessage";
    }

    public static class Grid
    {
        public const int DefaultColumnStringMaxLength = 40;
    }

    public static class Html
    {
        public const string NewLine = "<br />";
    }

    public static class PartialView
    {
        public const string EntityFormControlPartial = "_EntityFormControlPartial";
    }
}