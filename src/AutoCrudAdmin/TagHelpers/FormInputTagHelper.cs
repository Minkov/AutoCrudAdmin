namespace AutoCrudAdmin.TagHelpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoCrudAdmin.Enumerations;
using AutoCrudAdmin.Extensions;
using AutoCrudAdmin.Helpers;
using AutoCrudAdmin.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using static Constants.CssClassNames;
using static Constants.Html;
using static Constants.PartialView;

/// <summary>
/// The <see cref="FormInputTagHelper"/> class is a custom ASP.NET Core TagHelper used to generate form inputs based on certain attributes.
/// This includes support for Enums, text areas, multi-choice checkboxes, dropdowns, and more.
/// </summary>
/// <remarks>
/// This tag helper is designed to be used in the Razor view and is targeted at the 'formInput' element.
/// </remarks>
[HtmlTargetElement("formInput", TagStructure = TagStructure.NormalOrSelfClosing)]
public class FormInputTagHelper : TagHelper
{
    private static readonly ISet<Type> NumberTypes = new HashSet<Type>
    {
        typeof(long),
        typeof(int),
        typeof(int?),
        typeof(short),
    };

    private static readonly ISet<Type> FloatingPointTypes = new HashSet<Type>
    {
        typeof(decimal),
        typeof(double),
        typeof(float),
    };

    private readonly IPartialViewHelper partialViewHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormInputTagHelper"/> class.
    /// </summary>
    /// <param name="partialViewHelper">An instance of <see cref="IPartialViewHelper"/> to assist with the rendering of partial views.</param>
    public FormInputTagHelper(IPartialViewHelper partialViewHelper)
        => this.partialViewHelper = partialViewHelper;

    /// <summary>
    /// Gets or sets the name of the form input.
    /// </summary>
    [HtmlAttributeName("for-name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the type of the data bound to the form input.
    /// </summary>
    [HtmlAttributeName("for-type")]
    public Type Type { get; set; } = default!;

    /// <summary>
    /// Gets or sets the type of the form control.
    /// </summary>
    [HtmlAttributeName("for-form-control-type")]
    public FormControlType FormControlType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the form input is hidden.
    /// </summary>
    [HtmlAttributeName("is-hidden")]
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the form input is read-only.
    /// </summary>
    [HtmlAttributeName("is-readonly")]
    public bool IsReadonly { get; set; }

    /// <summary>
    /// Gets or sets the label text associated with the form input.
    /// </summary>
    [HtmlAttributeName("with-label")]
    public string? LabelText { get; set; }

    /// <summary>
    /// Gets or sets the value of the form input.
    /// </summary>
    [HtmlAttributeName("with-value")]
    public object? Value { get; set; }

    /// <summary>
    /// Gets or sets the options for the form input. This is used for dropdowns and checkboxes.
    /// </summary>
    [HtmlAttributeName("with-options")]
    public IEnumerable<object> Options { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value indicating whether the form input is a DbSet.
    /// </summary>
    [HtmlAttributeName("is-db-set")]
    public bool IsDbSet { get; set; }

    /// <summary>
    /// Gets or sets the HttpContext for the current request.
    /// </summary>
    [HtmlAttributeName("http-context")]
    public HttpContext HttpContext { get; set; } = default!;

    /// <summary>
    /// Processes the form input tag helper and writes the output.
    /// </summary>
    /// <param name="context">Contains information associated with a <see cref="FormInputTagHelper"/>.</param>
    /// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.SetAttribute("name", this.Name);

        if (this.Type.IsEnum)
        {
            this.PrepareEnum(output);
            return Task.CompletedTask;
        }

        if (this.FormControlType == FormControlType.TextArea)
        {
            this.PrepareTextArea(output);
        }
        else if (this.FormControlType == FormControlType.MultiChoiceCheckbox)
        {
            this.PrepareMultiChoiceCheckbox(output);
        }
        else if (this.FormControlType == FormControlType.ExpandableMultiChoiceCheckBox)
        {
            this.PrepareExpandableMultiChoiceCheckBox(output);
        }
        else if (this.FormControlType == FormControlType.Autocomplete)
        {
            this.PrepareAutocompleteDropdown(output);
        }
        else if (this.IsDbSet)
        {
            this.PrepareDropDownForDbSet(output);
        }
        else if (this.Options.Any())
        {
            this.PrepareDropdown(output, this.Options.Cast<DropDownViewModel>().ToList());
        }
        else
        {
            this.PrepareGenericInput(output);
        }

        if (this.IsHidden)
        {
            output.Attributes.SetAttribute("type", "hidden");
        }

        return Task.CompletedTask;
    }

    private static string GenerateCheckboxHtml(CheckboxFormControlViewModel checkboxFromControl, string expandedValuePrefix = "")
    {
        var isChecked = checkboxFromControl.IsChecked ? "checked='checked'" : string.Empty;
        var showExpandable = string.IsNullOrWhiteSpace(expandedValuePrefix);

        var expandableClass = showExpandable ? string.Empty : ExpandableClassName;

        var expandableAttribute = showExpandable
            ? string.Empty
            : $"expand='{expandedValuePrefix}'";

        return $"<div class='{FormCheckbox} {FormCheckboxInline}'>" +
               $"<input type='checkbox' class='{FormCheckboxInput} {expandableClass}' data-name='{checkboxFromControl.Name}' data-value='{checkboxFromControl.Value}' {isChecked} {expandableAttribute} />" +
               $"<label class='{FormCheckboxLabel}'>{checkboxFromControl.DisplayName}</label>" +
               "</div>";
    }

    private static string WrapExpandableComponent(ExpandableMultiChoiceCheckBoxFormControlViewModel x, string result)
        => $"<div id={x.ExpandedValuePrefix} class='{(x.IsChecked ? string.Empty : Hide)}'>" + result + "</div>";

    private static void PrepareDatetimePicker(TagHelperOutput output)
    {
        output.Attributes.SetAttribute("type", "datetime");
        output.AddClass("datetimepicker", HtmlEncoder.Default);
    }

    private void PrepareAutocompleteDropdown(TagHelperOutput output)
    {
        output.TagName = "input";
        output.Attributes.SetAttribute("id", this.Name + "Id");
        output.Attributes.SetAttribute("autocomplete", "off");
    }

    private void PrepareDropDownForDbSet(TagHelperOutput output)
    {
        var values = this.Options
            .Select(x => new DropDownViewModel
            {
                Name = x.ToString() !,
                Value = ReflectionHelper.GetEntityTypeUnproxied(x).GetPrimaryKeyValue(x).First().Value,
            })
            .ToList();

        this.PrepareDropdown(output, values, this.Name + "Id");
    }

    private void PrepareDropdown(
        TagHelperOutput output,
        IList<DropDownViewModel> options,
        string? name = null)
    {
        output.TagName = "select";
        output.Attributes.SetAttribute("name", name ?? this.Name);

        if (this.IsReadonly)
        {
            output.Attributes.SetAttribute("disabled", true);
        }

        var values = options
            .Select(x => x.Value)
            .ToList();
        var names = options
            .Select(x => x.Name)
            .ToList();

        var optionsResult = values
            .Select((t, i) => new { Text = names[i], Value = values[i], })
            .Select(x =>
                x.Value.ToString() == this.Value?.ToString()
                    ? $"<option value='{x.Value}' selected>{x.Text}</option>"
                    : $"<option value='{x.Value}'>{x.Text}</option>")
            .ToList();

        output.Content.SetHtmlContent(string.Join(string.Empty, optionsResult));
    }

    private void PrepareEnum(TagHelperOutput output)
    {
        output.TagName = "select";

        var values = Enum.GetValues(this.Type);
        var names = Enum.GetNames(this.Type);
        var options =
            values
                .Cast<object>()
                .Select((t, i) => new { Text = names[i], Value = values.GetValue(i)?.ToString(), })
                .Select(x =>
                    x.Value == this.Value?.ToString()
                        ? $"<option value='{x.Value}' selected>{x.Text}</option>"
                        : $"<option value='{x.Value}'>{x.Text}</option>")
                .ToList();
        output.Content.SetHtmlContent(
            string.Join(string.Empty, options));
    }

    private void PrepareMultiChoiceCheckbox(TagHelperOutput output)
    {
        output.TagName = "fieldset";
        output.RemoveClass(FormControl, HtmlEncoder.Default);

        var checkboxValues = (IEnumerable<CheckboxFormControlViewModel>)this.Options;

        var checkboxes = checkboxValues.Select(x => GenerateCheckboxHtml(x));

        output.Content.SetHtmlContent(string.Join(string.Empty, checkboxes));
    }

    private void PrepareTextArea(TagHelperOutput output)
    {
        output.TagName = "textarea";
        output.Content.SetContent(this.Value?.ToString() ?? string.Empty);

        // TODO: make height auto adjustable
        output.Attributes.SetAttribute("rows", 10);
    }

    private void PrepareGenericInput(TagHelperOutput output)
    {
        output.TagName = "input";
        output.Attributes.SetAttribute("value", this.Value);

        if (this.Type == typeof(string))
        {
            output.Attributes.SetAttribute("type", "text");
        }
        else if (this.Type == typeof(DateTime) || this.Type == typeof(DateTime?))
        {
            PrepareDatetimePicker(output);
        }
        else if (this.Type == typeof(TimeSpan) || this.Type == typeof(TimeSpan?))
        {
            output.Attributes.SetAttribute("type", "timespan");
        }
        else if (NumberTypes.Contains(this.Type))
        {
            output.Attributes.SetAttribute("type", "number");
        }
        else if (FloatingPointTypes.Contains(this.Type))
        {
            output.Attributes.SetAttribute("type", "number");
            output.Attributes.SetAttribute("step", "0.5");
        }
        else if (this.Type == typeof(bool))
        {
            output.Attributes.SetAttribute("type", "checkbox");

            if (this.Value is true)
            {
                output.Attributes.SetAttribute("checked", "checked");
            }
        }
        else if (this.Type == typeof(IFormFile) || this.Type == typeof(IFormFileCollection))
        {
            output.Attributes.SetAttribute("type", "file");

            if (this.Type == typeof(IFormFileCollection))
            {
                output.Attributes.SetAttribute("multiple", "multiple");
            }
        }
    }

    private void PrepareExpandableMultiChoiceCheckBox(TagHelperOutput output)
    {
        output.TagName = "fieldset";
        output.RemoveClass(FormControl, HtmlEncoder.Default);

        var checkboxValues = (IEnumerable<ExpandableMultiChoiceCheckBoxFormControlViewModel>)this.Options;

        var checkboxes = checkboxValues.Select(x =>
        {
            var result = this.partialViewHelper.GetViewResult(this.HttpContext, x, EntityFormControlPartial);

            result = WrapExpandableComponent(x, result);

            return GenerateCheckboxHtml(x, x.ExpandedValuePrefix) + result;
        });

        output.Content.SetHtmlContent(string.Join(NewLine, checkboxes));
    }
}