using Markdig.Helpers;
using Markdig.Parsers;

namespace Evergrowth.AspForMarkDigExtension;
public class AspForInlineParser : InlineParser
{
    /// <summary>
    /// Full sample to see what the intended use-case is.
    /// </summary>
    private static readonly string sample = "!ASP-FOR[myname]";

    /// <summary>
    /// Sample header against which the parser checks.
    /// </summary>
    private static readonly string sample_header = "!ASP-FOR[";

    /// <summary>
    /// The triggering character that will start the process to kick-start the parser.
    /// </summary>
    private static readonly char[] _openingCharacters = { '!' };

    private readonly AspForGeneratorOptions _options;

    public AspForInlineParser(AspForGeneratorOptions options)
    {
        this._options = options;
        this.OpeningCharacters = _openingCharacters;
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        bool matchFound = false;
        char previous;
        string checkHeader = string.Empty;

        previous = slice.PeekCharExtra(-1);

        if (previous.IsWhiteSpaceOrZero())          // this requires the ! to be preceeded by a space.  This helps prevent the parser from examining the proper use of exclamation points.
        {
            char current;
            int start;
            int end;

            current = slice.CurrentChar;
            start = slice.Start;
            end = start;

            checkHeader = slice.CurrentChar.ToString();

            // determine if next characters conform to sample
            for (int i = 0; i < sample_header.Length - 1; i++)
            {
                checkHeader += slice.NextChar();
            }

            if (checkHeader != sample_header)
            {
                return false;
            }

            int counter = 0;

            do
            {
                current = slice.NextChar();
                end = slice.Start;
                counter++;
            } while ((!current.Equals(']')) && counter < _options.MaxReferenceLength) ;

            current = slice.NextChar();

            int inlineStart;

            inlineStart = processor.GetSourcePosition(slice.Start, out int line, out int column);

            StringSlice InputString = new StringSlice(slice.Text, start, end);

            processor.Inline = new AspForGenerator
            {
                Span =
                    {
                        Start = inlineStart,
                        End = inlineStart + (end - start) + 1
                    },
                Line = line,
                Column = column,
                InputString = InputString
            };

            matchFound = true;
        }
        return matchFound;
    }
}
