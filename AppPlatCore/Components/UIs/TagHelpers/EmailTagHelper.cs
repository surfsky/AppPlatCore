using Microsoft.AspNetCore.Razor.TagHelpers;

namespace App.UIs.TagHelpers
{
    /// <summary>
    /// Email Tag. &lg;email mail-to="..." &gl; Contact Me &lt;/email&gl;
    /// to:
    /// &lg;a href="mailto:..." /&gl;
    /// </summary>
    /// <example>
    /// Pages/_ViewStart.cshtml
    /// @addTagHelper "*, App"
    /// </example>
    public class EmailTagHelper : TagHelper
    {
        private const string EmailDomain = "contoso.com";
        public string MailTo { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            var address = MailTo + "@" + EmailDomain; 
            output.Attributes.SetAttribute("href", "mailto:" + address);
            output.Content.SetContent(address);
        }
    }
}
