namespace UI.SyntaxBox;

/// <summary>
/// Extension methods dealing with text.
/// </summary>
public static class Text
{
    static readonly AhoCorasickSearch _search = new(
        new List<string> { "\r\n", "\n" },
        false);


    /// <summary>
    /// Gets a TextLine representing the line at a specific char position 
    /// in Text.
    /// </summary>
    /// <param name="text">The text to inspect.</param>
    /// <param name="position"></param>
    /// <returns>A TextLine or null.</returns>
    public static TextLine GetLineAtPosition(this string text, int position)
    {
        if (text == null || position < 0 || position > text.Length)
            return (null);
        string nlstr = Environment.NewLine.Last().ToString();
        int nlen = nlstr.Length;
        char nl = nlstr[0];

        int start = -1, end = -1;

        //Search left
        for (int i = position - 1; i > nlen; i--)
        {
            if (text[i - nlen] == nl && (nlen == 1 || text.Substring(i - nlen, nlen) == nlstr))
            {
                start = i;
                break;
            }
        }
        if (start < 0)
            start = 0;

        // Search right
        for (int i = position; i < text.Length; i++)
        {
            if (text[i] == nl && (nlen == 1 || text.Length > i + 1 && text[i + 1] == nlstr[1]))
            {
                end = i + nlen;
                break;
            }
        }
        if (end < 0)
            end = text.Length;

        TextLine line = new TextLine
        {
            Text = text.Substring(start, end - start),
            StartIndex = start,
            LineNumber = -1
        };

        return line;
    }


    /// <summary>
    /// Parses the text returning a chunk of lines as TextLines starting at
    /// First line and ending on Last line or at the end of the text.
    /// This can be done using built-in TextBox functions, but this is >5x faster.
    /// </summary>
    /// <param name="text">The text to parse.</param>
    /// <param name="first">The first line to include in the chunk.</param>
    /// <param name="last">The last line to include in the chunk.</param>
    /// <returns></returns>
    public static List<TextLine> GetLines(this string text, int first, int last, out int totalLines)
    {
        string nlstr = Environment.NewLine;
        int nlen = nlstr.Length;
        char nl = nlstr[nlen - 1];

        List<TextLine> lines = new List<TextLine>(Math.Min(1000, last - first));
        int start = 0;
        int foundNewlines = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == nl)
            {
                if (foundNewlines >= first && foundNewlines <= last)
                {
                    TextLine line = new TextLine
                    {
                        Text = text.Substring(start, i + 1 - start),
                        StartIndex = start,
                        LineNumber = foundNewlines
                    };
                    lines.Add(line);
                }

                foundNewlines++;
                start = i + 1;
            }
        }

        if (start <= text.Length && foundNewlines >= first && foundNewlines <= last)
        {
            TextLine tailLine = new TextLine
            {
                Text = text.Substring(start),
                StartIndex = start,
                LineNumber = foundNewlines
            };
            lines.Add(tailLine);
        }

        totalLines = foundNewlines + 1;

        return lines;
    }

    /// <summary>
    /// Parses the text returning a chunk of lines as TextLines starting at
    /// First line and ending on Last line or at the end of the text.
    /// This is the same as GetLines, but uses the Aho-Corasick algorithm,
    /// which makes it a bit slower.
    /// </summary>
    /// <param name="text">The text to parse.</param>
    /// <param name="first">The first line to include in the chunk.</param>
    /// <param name="last">The last line to include in the chunk.</param>
    /// <returns></returns>
    public static List<TextLine> GetLines2(this string text, int first, int last, out int totalLines)
    {
        string nlstr = Environment.NewLine;
        int nlen = nlstr.Length;
        char nl = nlstr[0];

        List<TextLine> lines = new List<TextLine>(Math.Min(1000, last - first));
        int start = 0;
        int foundNewlines = 0;

        var matches = _search.FindAll(text)
            .OrderBy((x) => x.Position)
            .ToList();

        foreach (var nwln in matches)
        {
            if (foundNewlines >= first && foundNewlines <= last)
            {
                TextLine line = new TextLine
                {
                    Text = text.Substring(start, nwln.Position + nwln.Length - start),
                    StartIndex = start,
                    LineNumber = foundNewlines
                };
                lines.Add(line);
            }

            foundNewlines++;
            start = nwln.Position + nwln.Length;
        }

        if (start <= text.Length && foundNewlines >= first && foundNewlines <= last)
        {
            TextLine tailLine = new TextLine
            {
                Text = text.Substring(start),
                StartIndex = start,
                LineNumber = foundNewlines
            };
            lines.Add(tailLine);
        }

        totalLines = foundNewlines + 1;
        return lines;
    }


    /// <summary>
    /// Determines if the Position in Text is is on a start-of-word-boundary,
    /// i.e. that the position is at the beginning of the string or that 
    /// the prefix character is a non-word character. It assumes the position is
    /// already on a word character.
    /// </summary>
    /// <param name="text">The text to inspect</param>
    /// <param name="position">The a position in Text pointing at a word character.</param>
    /// <returns></returns>
    public static bool IsStartWordBoundary(this string text, int position)
    {
        return text is null
            ? throw new ArgumentNullException(nameof(text))
            : position == 0 || !char.IsLetterOrDigit(text, position - 1) && text[position - 1] != '_';
    }

    /// <summary>
    /// Determines if the Position in Text is is on a start-of-word-boundary,
    /// i.e. that the position is at the end of the string or that 
    /// the postfix character is a non-word character. It assumes the position is
    /// already on a word character.
    /// </summary>
    /// <param name="text">The text to inspect</param>
    /// <param name="position">The a position in Text pointing at a word character.</param>
    /// <returns></returns>
    public static bool IsEndWordBoundary(this string text, int position)
    {
        return text is null
            ? throw new ArgumentNullException(nameof(text))
            : position >= text.Length - 1 || !char.IsLetterOrDigit(text, position + 1) && text[position + 1] != '_';
    }


    public static void AddRange(this HashSet<int> target, IEnumerable<int> Items)
    {
        foreach (var item in Items)
            target.Add(item);
    }
}