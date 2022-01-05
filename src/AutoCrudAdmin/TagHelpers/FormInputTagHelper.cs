namespace AutoCrudAdmin.TagHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoCrudAdmin.Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("formInput", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class FormInputTagHelper : TagHelper
    {
        private static ISet<Type> NumberTypes => new HashSet<Type>
        {
            typeof(long),
            typeof(int),
            typeof(short),

            typeof(decimal),
            typeof(double),
            typeof(float),
        };

        [HtmlAttributeName("for-name")]
        public string Name { get; set; }

        [HtmlAttributeName("for-type")]
        public Type Type { get; set; }

        [HtmlAttributeName("is-hidden")]
        public bool IsHidden { get; set; }

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
            else
            {
                this.PrepareComplex(output);
            }

            if (this.IsHidden)
            {
                output.Attributes.SetAttribute("type", "hidden");
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
                .Select(x => x.GetType().UnProxy().GetPrimaryKeyValue(x).FirstOrDefault())
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