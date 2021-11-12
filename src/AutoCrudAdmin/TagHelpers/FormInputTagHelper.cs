namespace AutoCrudAdmin.TagHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoCrudAdmin.Extensions;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("formInput", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class FormInputTagHelper : TagHelper
    {
        [HtmlAttributeName("for-name")]
        public string Name { get; set; }

        [HtmlAttributeName("for-type")]
        public Type Type { get; set; }

        [HtmlAttributeName("with-label")]
        public string LabelText { get; set; }

        [HtmlAttributeName("with-value")]
        public object Value { get; set; }

        [HtmlAttributeName("with-options")]
        public IEnumerable<object> Options { get; set; }

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
            else if (this.Type == typeof(DateTime))
            {
                output.Attributes.SetAttribute("type", "datetime");
            }
            else if (this.Type == typeof(int))
            {
                output.Attributes.SetAttribute("type", "number");
            }
            else if (this.Type == typeof(bool))
            {
                output.Attributes.SetAttribute("type", "checkbox");
                output.Attributes.SetAttribute("value", "true");

                if (this.Value is not true)
                {
                    return Task.CompletedTask;
                }

                output.Attributes.SetAttribute("checked", "checked");
            }
            else
            {
                this.PrepareComplex(output);
            }

            return Task.CompletedTask;
        }

        private void PrepareComplex(TagHelperOutput output)
        {
            output.TagName = "select";

            output.Attributes.SetAttribute("name", this.Name + "Id");
            var valuesList = this.Options
                !.ToList();
            var values = valuesList
                .Select(x => x.GetType().GetPrimaryKeyValue(x).FirstOrDefault())
                .Select(x => x.Value)
                .ToList();
            var names = valuesList
                .Select(x => x.ToString())
                .ToList();

            var value = this.Value?.ToString();

            var options = values
                .Select((t, i) => new { Text = names[i], Value = values[i], })
                .Select(x =>
                    x.Value.ToString() == value
                        ? $"<option value='{x.Value}' selected>{x.Text}</option>"
                        : $"<option value='{x.Value}'>{x.Text}</option>")
                .ToList();
            output.Content.SetHtmlContent(
                string.Join(string.Empty, options));
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
    }
}