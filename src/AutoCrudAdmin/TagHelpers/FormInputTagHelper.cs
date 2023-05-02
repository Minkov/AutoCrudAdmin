namespace AutoCrudAdmin.TagHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

    /// <summary>
    /// A class for processing the elements of a form control.
    /// </summary>
    [HtmlTargetElement("formInput", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class FormInputTagHelper : TagHelper
    {
        /// <summary>
        /// Gets or sets the name of the form control form input.
        /// </summary>
        [HtmlAttributeName("for-name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the type of data the form control holds..
        /// </summary>
        [HtmlAttributeName("for-type")]
        public Type Type { get; set; } = null!;

        /// <summary>
        /// Gets or sets the type of the form control.
        /// </summary>
        [HtmlAttributeName("for-form-control-type")]
        public FormControlType FormControlType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the form control is hidden.
        /// </summary>
        [HtmlAttributeName("is-hidden")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the form control is read only.
        /// </summary>
        [HtmlAttributeName("is-readonly")]
        public bool IsReadonly { get; set; }

        /// <summary>
        /// Gets or sets the text of the label of the form control.
        /// </summary>
        [HtmlAttributeName("with-label")]
        public string LabelText { get; set; } = null!;

        /// <summary>
        /// Gets or sets the value of the form control.
        /// </summary>
        [HtmlAttributeName("with-value")]
        public object Value { get; set; } = null!;

        /// <summary>
        /// Gets or sets the options of the form control.
        /// </summary>
        [HtmlAttributeName("with-options")]
        public IEnumerable<object> Options { get; set; } = null!;

        /// <summary>
        /// Gets or sets a value indicating whether the form control has a database set.
        /// </summary>
        [HtmlAttributeName("is-db-set")]
        public bool IsDbSet { get; set; }

        private static ISet<Type> NumberTypes => new HashSet<Type>
        {
            typeof(long),
            typeof(int),
            typeof(int?),
            typeof(short),

            typeof(decimal),
            typeof(double),
            typeof(float),
        };

        /// <summary>
        /// Processes the attributes of the form control.
        /// </summary>
        /// <param name="context">Contains information related to the execution of ITagHelpers.</param>
        /// <param name="output">Represents the output of an ITagHelper.</param>
        /// <returns>The successfully completed task.</returns>
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

        private void PrepareAutocompleteDropdown(TagHelperOutput output)
        {
            output.TagName = "input";
            output.Attributes.SetAttribute("id", this.Name + "Id");
            output.Attributes.SetAttribute("autocomplete", this.Name + "off");
            output.Attributes.SetAttribute("list", this.Name + "Autocomplete");
            var options = this.Options
                .Select(x => new DropDownViewModel
                {
                    Name = x.ToString(),
                    Value = ReflectionHelper.GetEntityTypeUnproxied(x).GetPrimaryKeyValue(x).First().Value,
                })
                .Take(20)
                .ToList();

            var values = options
                .Select(x => x.Value!)
                .ToList();
            var names = options
                .Select(x => x.Name)
                .ToList();

            var optionsResult = values
                .Select((t, i) => new { Text = names[i], Value = values[i], })
                .Select(x => $"<option value='{x.Text}'>{x.Text}</option>")
                .ToList();

            var sb = new StringBuilder();
            optionsResult.ForEach(x => sb.AppendLine(x));
            var result = $"<datalist id={this.Name}Autocomplete>" + sb + "</datalist>";

            output.Content.SetHtmlContent(string.Join(string.Empty, result));
        }

        private void PrepareDropDownForDbSet(TagHelperOutput output)
        {
            var values = this.Options
                .Select(x => new DropDownViewModel
                {
                    Name = x.ToString(),
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
                .Select(x => x.Value!)
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
                        x.Value == this.Value.ToString()
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

            var checkboxes = checkboxValues.Select(x =>
            {
                var isChecked = x.IsChecked ? "checked='checked'" : string.Empty;

                return $"<div class='{FormCheckbox} {FormCheckboxInline}'>" +
                    $"<input type='checkbox' class='{FormCheckboxInput}' data-name='{x.Name}' data-value='{x.Value}' {isChecked}/>" +
                    $"<label class='{FormCheckboxLabel}'>{x.DisplayName}</label>" +
                    "</div>";
            });

            output.Content.SetHtmlContent(string.Join(string.Empty, checkboxes));
        }

        private void PrepareTextArea(TagHelperOutput output)
        {
            output.TagName = "textarea";
            output.Content.SetContent(this.Value.ToString());

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
                this.PrepareDatetimePicker(output);
            }
            else if (this.Type == typeof(TimeSpan) || this.Type == typeof(TimeSpan?))
            {
                output.Attributes.SetAttribute("type", "timespan");
            }
            else if (NumberTypes.Contains(this.Type))
            {
                output.Attributes.SetAttribute("type", "number");
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

        private void PrepareDatetimePicker(TagHelperOutput output)
        {
            output.Attributes.SetAttribute("type", "datetime");
            output.AddClass("datetimepicker", HtmlEncoder.Default);
        }
    }
}
