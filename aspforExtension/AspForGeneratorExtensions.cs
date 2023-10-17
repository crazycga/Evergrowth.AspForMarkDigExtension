using Markdig;
using Markdig.Helpers;

namespace Evergrowth.AspForMarkDigExtension;

public static class AspForGeneratorExtensions
{
    public static MarkdownPipelineBuilder UseAspForGenerator(this MarkdownPipelineBuilder pipeline, AspForGeneratorOptions options)
    {
        OrderedList<IMarkdownExtension> extensions = pipeline.Extensions;

        if (!extensions.Contains<AspForGeneratorExtension>())
        {
            extensions.Add(new AspForGeneratorExtension(options));
        }

        return pipeline;
    }
}