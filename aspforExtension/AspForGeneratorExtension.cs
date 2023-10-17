using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;

namespace Evergrowth.AspForMarkDigExtension;
public class AspForGeneratorExtension : IMarkdownExtension
{
    private readonly AspForGeneratorOptions _options;

    public AspForGeneratorExtension(AspForGeneratorOptions options)
    {
        _options = options;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        OrderedList<InlineParser> parsers = pipeline.InlineParsers;

        if (!parsers.Contains<AspForInlineParser>())
        {
            parsers.Add(new AspForInlineParser(_options));
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        HtmlRenderer htmlRenderer;
        ObjectRendererCollection renderers;

        htmlRenderer = renderer as HtmlRenderer;
        renderers = htmlRenderer?.ObjectRenderers;

        if (renderers != null && !renderers.Contains<AspForRenderer>())
        {
            renderers.Add(new AspForRenderer(_options));
        }
    }


}


