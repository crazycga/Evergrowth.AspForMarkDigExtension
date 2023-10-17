using Markdig.Helpers;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace Evergrowth.AspForMarkDigExtension;

[DebuggerDisplay("!ASPFOR[" + nameof(InputString) + "]")]
public class AspForGenerator : LeafInline
{
    public StringSlice InputString { get; set; }

}
