using System.Net;

namespace Tellurian.Utilities.Web;

/// <summary>
/// Extensions that wrap text in HTML elements.
/// </summary>
public static class MarkupStringExtensions
{
    extension(string? content)
    {
        /// <summary>
        /// The text in a <c>span</c> element with the CSS class; an empty string for empty text. The text and the class are
        /// HTML-encoded, so the text is shown as written.
        /// </summary>
        /// <param name="cssClass">The CSS class of the element, or none.</param>
        public string Span(string cssClass = "") => content.InElement("span", cssClass);

        /// <summary>
        /// The text in a <c>div</c> element with the CSS class; an empty string for empty text. The text and the class are
        /// HTML-encoded, so the text is shown as written.
        /// </summary>
        /// <param name="cssClass">The CSS class of the element, or none.</param>
        public string Div(string cssClass = "") => content.InElement("div", cssClass);

        private string InElement(string elementName, string? cssClass) =>
            content.IsEmpty ? string.Empty :
            string.IsNullOrWhiteSpace(cssClass) ? $"<{elementName}>{WebUtility.HtmlEncode(content)}</{elementName}>" :
            $"""<{elementName} class="{WebUtility.HtmlEncode(cssClass)}">{WebUtility.HtmlEncode(content)}</{elementName}>""";
    }
}
