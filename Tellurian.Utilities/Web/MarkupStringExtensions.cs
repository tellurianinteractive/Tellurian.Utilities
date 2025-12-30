namespace Tellurian.Utilities.Web;

public static class MarkupStringExtensions
{
    extension(string? content)
    {
        public string Span(string cssClass = "") =>
            content.IsWhite ? string.Empty :
            $"""
            <span class="{cssClass}">{content}</span>
            """;

        public string Div(string cssClass = "") =>
            content.IsWhite ? string.Empty :
            $"""
            <div class="{cssClass}">{content}</dov>
            """;

    }
}
