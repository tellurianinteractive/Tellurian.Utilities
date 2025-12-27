using Markdig;

namespace Tellurian.Utilities.Web;

static class MarkdownExtensions
{
    /// <summary>
    /// Provides a default instance of the Markdown processing pipeline with standard configuration.
    /// </summary>
    /// <remarks>Use this pipeline for typical Markdown parsing scenarios where no custom configuration is
    /// required. The default pipeline includes commonly used extensions and settings suitable for general-purpose
    /// Markdown processing.</remarks>
    public static MarkdownPipeline DefaultPipeline = CreatePipeline();

    private static MarkdownPipeline CreatePipeline()
    {
        var builder = new MarkdownPipelineBuilder();
        builder.UseAdvancedExtensions();
        return builder.Build();
    }

    extension(string? markdown)
    {
        /// <summary>
        /// Converts the current Markdown content to its HTML representation.
        /// </summary>
        /// <returns>A string containing the HTML generated from the Markdown content. Returns an empty string if the Markdown
        /// content is null, empty, or consists only of white-space characters.</returns>
        public string HtmlFromMarkdown()
        {
            if (markdown.IsEmpty) return string.Empty;
            return Markdown.ToHtml(markdown, DefaultPipeline);
        }
    }
}
