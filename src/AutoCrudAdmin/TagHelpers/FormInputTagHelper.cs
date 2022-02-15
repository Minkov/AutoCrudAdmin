namespace AutoCrudAdmin.TagHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutoCrudAdmin.Enumerations;
    using AutoCrudAdmin.Extensions;
    using AutoCrudAdmin.ViewModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using static Constants.CssClassNames;

    [HtmlTargetElement("formInput", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class FormInputTagHelper : TagHelper
    {
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

        [HtmlAttributeName("for-name")]
        public string Name { get; set; }

        [HtmlAttributeName("for-type")]
        public Type Type { get; set; }

        [HtmlAttributeName("for-form-control-type")]
        public FormControlType FormControlType { get; set; }

        [HtmlAttributeName("is-hidden")]
        public bool IsHidden { get; set; }

        [HtmlAttributeName("with-label")]
        public string LabelText { get; set; }

        [HtmlAttributeName("with-value")]
        public object Value { get; set; }

        [HtmlAttributeName("with-options")]
        public IEnumerable<object> Options { get; set; }

        [HtmlAttributeName("is-db-set")]
        public bool IsDbSet { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("name", this.Name);

            if (this.Type.IsEnum)
            {
                this.PrepareEnum(output);
                return Task.CompletedTask;
            }

            output.TagName = "input";
            output.Attributes.SetAttribute("value", this.Value);

            if (this.Type == typeof(string))
            {
                output.Attributes.SetAttribute("type", "text");
            }
            else if (this.Type == typeof(DateTime) || this.Type == typeof(DateTime?))
            {
                output.Attributes.SetAttribute("type", "datetime");
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
            else if (this.FormControlType == FormControlType.MultiChoiceCheckbox)
            {
                this.PrepareMultiChoiceCheckbox(output);
            }
            else if (this.IsDbSet)
            {
                this.PrepareDropDownForDbSet(output);
            }
            else
            {
                this.PrepareDropdown(output, this.Options.Cast<DropDownViewModel>().ToList());
            }

            if (this.IsHidden)
            {
                output.Attributes.SetAttribute("type", "hidden");
            }

            return Task.CompletedTask;
        }

        private void PrepareDropDownForDbSet(TagHelperOutput output)
        {
            var values = this.Options
                .Select(x => new DropDownViewModel
                {
                    Name = x.ToString(),
                    Value = x.GetType().UnProxy().GetPrimaryKeyValue(x).First().Value,
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

            var checkboxValues = (IEnumerable<CheckboxFormControlViewModel>)this.Options;

            var checkboxes = checkboxValues.Select(x =>
            {
                var isChecked = x.IsChecked ? "checked='checked'" : string.Empty;

                return $"<label class='d-block'>{x.Name}" +
                       $"<input type='checkbox' class='{FormCheckboxInput}' data-name='{x.Name}' data-value='{x.Value}' {isChecked}/>" +
                       "</label>";
            });

            output.Content.SetHtmlContent(string.Join(string.Empty, checkboxes));
        }
    }
}